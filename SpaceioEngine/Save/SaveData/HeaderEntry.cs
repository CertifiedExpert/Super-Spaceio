using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine.Save.SaveData
{
    internal struct HeaderEntry
    {
        public int ChunkX { get; set; }
        public int ChunkY { get; set; }
        public long Offset {  get; set; }
        public int Length { get; set; }

        public HeaderEntry(int chunkX, int chunkY, long offset, int length)
        {
            ChunkX = chunkX;
            ChunkY = chunkY;
            Offset = offset;
            Length = length;
        }
    }
}
