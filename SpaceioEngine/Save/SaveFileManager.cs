using ConsoleEngine.Save.SaveData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ConsoleEngine
{
    public class SaveFileManager // TODO: this REALLY needs to be bug checked...
    {
        private Engine Engine { get; set; }
        internal string ChunksHeaderPath { get; private set; }
        internal string ChunksDataPath { get; private set; }

        private Dictionary<Vec2i, int> chunkInHeaderIndexes = new Dictionary<Vec2i, int>();
        private List<HeaderEntry> headerEntries = new List<HeaderEntry>();

        private const int bittArrayDefaultSize = 100;
        private BitArray segmentOccupancy = new BitArray(bittArrayDefaultSize);

        private readonly int bytesForSegment = 100;

        public SaveFileManager(Engine engine) 
        {
            Engine = engine;
        }

        internal void Init()
        {
            ChunksHeaderPath = $"{Engine.pathWorldFolder}\\ChunkHeader.txt";
            ChunksDataPath = $"{Engine.pathWorldFolder}\\ChunkData.txt";
        }
        internal void SaveChunkBytes(byte[] bytes, Vec2i index)
        {
            var numberOfBytes = bytes.Length;
            int occupiedSectors = (numberOfBytes + bytesForSegment - 1) / bytesForSegment; // divides numberOfBytes by bytesForSegment and rounds up

            if (chunkInHeaderIndexes.ContainsKey(index)) // if chunk exists in save file
            {
                var currentHeaderIndex = chunkInHeaderIndexes[index];

                if (occupiedSectors < headerEntries[currentHeaderIndex].Length) // if new chunks smaller in size
                {
                    var oldLength = headerEntries[currentHeaderIndex].Length;
                    headerEntries[currentHeaderIndex] =
                        new HeaderEntry(index.X, index.Y, headerEntries[currentHeaderIndex].Offset, occupiedSectors);
                    for (var i = headerEntries[currentHeaderIndex].Offset + occupiedSectors;
                        i < headerEntries[currentHeaderIndex].Offset + oldLength; i++)
                    {
                        segmentOccupancy[i] = false;
                    }

                    WriteChunkDataToFile(bytes, headerEntries[currentHeaderIndex].Offset);
                }
                else if (occupiedSectors > headerEntries[currentHeaderIndex].Length) // if new chunk larger in size
                {
                    // Checks if segmentOccupancy can accommodate new date. Extends it if can't
                    ExtendSegmentOccupancyIfNeeded(headerEntries[currentHeaderIndex].Offset + occupiedSectors);    
                
                    // Tries to allocate same origin. If impossible allocates new origin
                    var newOffset = headerEntries[currentHeaderIndex].Offset; 
                    for (var i = headerEntries[currentHeaderIndex].Offset + headerEntries[currentHeaderIndex].Length;
                        i < headerEntries[currentHeaderIndex].Offset + occupiedSectors; i++)
                    {
                        if (segmentOccupancy[i] == true)
                        {
                            newOffset = FindFreeSpace(bytes);
                            break;
                        }
                    }

                    if (newOffset != headerEntries[currentHeaderIndex].Offset) // if different origin has been allocated
                    {
                        for (var i = headerEntries[currentHeaderIndex].Offset;
                                        i < headerEntries[currentHeaderIndex].Offset + headerEntries[currentHeaderIndex].Length; i++)
                        {
                            segmentOccupancy[i] = false;
                        }
                        for (var i = newOffset; i < newOffset + occupiedSectors; i++)
                        {
                            segmentOccupancy[i] = true;
                        }

                        headerEntries[currentHeaderIndex] = new HeaderEntry(index.X, index.Y, newOffset, occupiedSectors);
                        WriteChunkDataToFile(bytes, newOffset);
                    }
                    else // if same origin has been allocated
                    {
                        for (var i = headerEntries[currentHeaderIndex].Offset + headerEntries[currentHeaderIndex].Length; 
                            i < headerEntries[currentHeaderIndex].Offset + occupiedSectors; i++)
                        {
                            segmentOccupancy[i] = true;
                        }
                        headerEntries[currentHeaderIndex] = new HeaderEntry(index.X, index.Y, newOffset, occupiedSectors);
                        WriteChunkDataToFile(bytes, newOffset);
                    }
                }
                else // if new chunk same size
                {
                    WriteChunkDataToFile(bytes, headerEntries[currentHeaderIndex].Offset);
                }
            }
            else // if chunks does not exist in save file
            {
                var offset = FindFreeSpace(bytes);
                AddHeaderEntry(index, offset, occupiedSectors);
                var headerIndex = chunkInHeaderIndexes[index];
                for (var i = offset; i < offset + occupiedSectors; ++i)
                {
                    segmentOccupancy[i] = true;
                }
                WriteChunkDataToFile(bytes, offset);
            }
        }
        internal byte[] LoadChunkBytes(Vec2i index)
        {
            var he = headerEntries[chunkInHeaderIndexes[index]];

            using (FileStream fs = new FileStream(ChunksDataPath, FileMode.Open, FileAccess.Read))
            using (BinaryReader binaryReader = new BinaryReader(fs))
            {
                fs.Seek(he.Offset * bytesForSegment, SeekOrigin.Begin);
                return binaryReader.ReadBytes(he.Length);
            }
        }

        // Returns the segment index where data should begin to be written
        private int FindFreeSpace(byte[] bytes)
        {
            var offset = 0;

            var successfullyAllocated = false;
            var freeSegmentsInARow = 0;
            for (var i = 0; i < segmentOccupancy.Length; i++)
            {
                if (segmentOccupancy[i] == false)
                {
                    freeSegmentsInARow++;
                }
                else
                {
                    freeSegmentsInARow = 0;
                    offset = i;
                }

                if (freeSegmentsInARow == bytes.Length)
                {
                    successfullyAllocated = true;
                    break;
                }
            }

            if (successfullyAllocated == false)
            {
                var oldSegmentOccupancyLength = segmentOccupancy.Length;
                ExtendSegmentOccupancyIfNeeded(segmentOccupancy.Length + (bytes.Length + bytesForSegment - 1) / bytesForSegment);

                freeSegmentsInARow = 0;
                offset = oldSegmentOccupancyLength;
                for (var i = oldSegmentOccupancyLength; i < segmentOccupancy.Length; i++)
                {
                    if (segmentOccupancy[i] == false)
                    {
                        freeSegmentsInARow++;
                    }
                    else
                    {
                        freeSegmentsInARow = 0;
                        offset = i;
                    }

                    if (freeSegmentsInARow == bytes.Length)
                    {
                        break;
                    }
                }
            }

            return offset;
        }
        private void ExtendSegmentOccupancyIfNeeded(int newNecessarySize)
        {
            if (newNecessarySize > segmentOccupancy.Length)
            {
                var newSegmentOccupancyLength = newNecessarySize - segmentOccupancy.Length + bittArrayDefaultSize;
                var newArr = new BitArray(newSegmentOccupancyLength);
                for (var i = 0; i < segmentOccupancy.Length; i++)
                {
                    newArr[i] = segmentOccupancy[i];
                }
            }
        }
        private void WriteChunkDataToFile(byte[] bytes, int offset)
        {
            using (FileStream fs = new FileStream(ChunksDataPath, FileMode.Open, FileAccess.Write))
            {
                fs.Seek((long)offset * bytesForSegment, SeekOrigin.Begin);
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        private HeaderEntry StringToHeaderEntry(string line)
        {
            var info = line.Split(',');
            return new HeaderEntry(int.Parse(info[0]), int.Parse(info[1]), int.Parse(info[2]), int.Parse(info[3]));
        }
        private string HeaderEntryToString(HeaderEntry entry)
        {
            return string.Concat(entry.ChunkX, ",", entry.ChunkY, ",", entry.Offset, ",", entry.Length);
        }

        private void AddHeaderEntry(Vec2i index, int offset, int length)
        {
            headerEntries.Add(new HeaderEntry(index.X, index.Y, offset, length));
            chunkInHeaderIndexes.Add(index, headerEntries.Count - 1);
        }
        private void RemoveHeaderEntry(int headerIndex)
        {
            chunkInHeaderIndexes.Remove(new Vec2i(headerEntries[headerIndex].ChunkX, headerEntries[headerIndex].ChunkY));

            headerEntries[headerIndex] = headerEntries[headerEntries.Count - 1];
            headerEntries.RemoveAt(headerEntries.Count - 1);

            chunkInHeaderIndexes.Add(new Vec2i(headerEntries[headerIndex].ChunkX, headerEntries[headerIndex].ChunkY), headerIndex);
        }
        internal void LoadChunkHeader(string path)
        {
            headerEntries.Clear();
            chunkInHeaderIndexes.Clear();

            var lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                var he = StringToHeaderEntry(lines[i]);
                headerEntries.Add(he);
                chunkInHeaderIndexes.Add(new Vec2i(he.ChunkX, he.ChunkY), i);

                for (var j = 0; j < he.Length; j++)
                {
                    segmentOccupancy[he.Offset + j] = true;
                }
            }
        }
        internal void WriteHeaderToFile(string path)
        {
            var lines = new string[headerEntries.Count];
            for (int i = 0; i < headerEntries.Count; i++)
            {
                lines[i] = HeaderEntryToString(headerEntries[i]);
            }

            File.WriteAllLines(path, lines);
        }
    }
}
