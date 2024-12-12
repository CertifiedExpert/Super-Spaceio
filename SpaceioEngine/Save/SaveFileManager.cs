using ConsoleEngine.Save.SaveData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ConsoleEngine
{
    public class SaveFileManager
    {
        internal readonly string ChunksHeaderPath = ""; // TODO: actually set the path
        internal readonly string ChunksPath = "";

        private Dictionary<Vec2i, int> chunkInHeaderIndexes = new Dictionary<Vec2i, int>();
        private List<HeaderEntry> headerEntries = new List<HeaderEntry>();

        private readonly int bytesForSegment = 100;
        internal void SaveChunkXML(string xml, Vec2i index)
        {
            var numberOfBytes = xml.Length * sizeof(char);
            int occupiedSectors = (numberOfBytes + bytesForSegment - 1) / bytesForSegment; // divides numberOfBytes by bytesForSegment and rounds up

            if (chunkInHeaderIndexes.ContainsKey(index))
            {

            }
            else
            {
                
            }
        }

        internal string LoadChunkXML(Vec2i index)
        {
            var he = headerEntries[chunkInHeaderIndexes[index]];

            using (FileStream fs = new FileStream(ChunksPath, FileMode.Open, FileAccess.Read))
            using (BinaryReader binaryReader = new BinaryReader(fs))
            {
                fs.Seek(he.Offset, SeekOrigin.Begin);
                var bytes = binaryReader.ReadBytes(he.Length);
                return Encoding.UTF8.GetString(bytes);
            }
        }

        private HeaderEntry StringToHeaderEntry(string line)
        {
            var info = line.Split(',');
            return new HeaderEntry(int.Parse(info[0]), int.Parse(info[1]), long.Parse(info[2]), int.Parse(info[3]));
        }
        private string HeaderEntryToString(HeaderEntry entry)
        {
            return string.Concat(entry.ChunkX, ",", entry.ChunkY, ",", entry.Offset, ",", entry.Length);
        }

        private void AddHeaderEntry(Vec2i index, long offset, int length)
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
        internal void LoadChunksHeader(string path)
        {
            headerEntries.Clear();
            chunkInHeaderIndexes.Clear();

            var lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                var he = StringToHeaderEntry(lines[i]);
                headerEntries.Add(he);
                chunkInHeaderIndexes.Add(new Vec2i(he.ChunkX, he.ChunkY), i);
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
