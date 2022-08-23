using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Platformer.Engine
{
    class Bitmap
    {
        public Vec2i Size { get; set; }
        public char[,] Data { get; set; }
        //TODO: delete size from constructor. It can be deduced inside from data
        public Bitmap(Vec2i size, char[,] data)
        {
            Size = size;
            Data = data;
        }

        public static Bitmap CreateStaticFillBitmap(Vec2i size, char fillChar)
        {
            var bitmap = new Bitmap(size, new char[size.X, size.Y]);
            for (var i = 0; i < size.X; i++)
            {
                for (var j = 0; j < size.Y; j++)
                {
                    bitmap.Data[i, j] = fillChar;
                }
            }

            return bitmap;
        }
    }
}
