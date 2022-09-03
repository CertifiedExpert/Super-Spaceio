using System;
using System.Configuration;

namespace Spaceio.Engine
{
    class Bitmap
    {
        public Vec2i Size { get; set; }
        public char[,] Data { get; set; }
        public Bitmap(char[,] data)
        {
            Size = new Vec2i(data.GetLength(0), data.GetLength(1));
            Data = data;
        }

        public static Bitmap CreateStaticFillBitmap(Vec2i size, char fillChar)
        {
            var bitmap = new Bitmap(new char[size.X, size.Y]);
            for (var i = 0; i < size.X; i++)
            {
                for (var j = 0; j < size.Y; j++)
                {
                    bitmap.Data[i, j] = fillChar;
                }
            }

            return bitmap;
        }

        public void FillWith(char fillChar)
        {
            for (var x = 0; x < Size.X; x++)
            {
                for (var y = 0; y < Size.Y; y++)
                {
                    Data[x, y] = fillChar;
                }
            }
        }
    }
}
