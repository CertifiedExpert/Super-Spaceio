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
            DrawFilledSquare(new Vec2i(0, 0), Size, fillChar);
        }
        public void DrawLine(Vec2i startPoint, Vec2i endPoint, char fillChar)
        {

        }
        public void DrawSquareOutline(Vec2i bottomLeftCorner, Vec2i size, char fillChar)
        {
            DrawLine(bottomLeftCorner, new Vec2i(bottomLeftCorner.X, bottomLeftCorner.Y + size.Y), fillChar);
            DrawLine(bottomLeftCorner, new Vec2i(bottomLeftCorner.X + size.X, bottomLeftCorner.Y), fillChar);
            DrawLine(new Vec2i(bottomLeftCorner.X, bottomLeftCorner.Y + size.Y), new Vec2i(bottomLeftCorner.X + size.X, bottomLeftCorner.Y + size.Y), fillChar);
            DrawLine(new Vec2i(bottomLeftCorner.X + size.X, bottomLeftCorner.Y), new Vec2i(bottomLeftCorner.X + size.X, bottomLeftCorner.Y + size.Y), fillChar);
        }
        public void DrawFilledSquare(Vec2i bottomLeftCorner, Vec2i size, char fillChar)
        {
            var endX = bottomLeftCorner.X + size.X;
            var endY = bottomLeftCorner.Y + size.Y;
            for (var x = bottomLeftCorner.X; x < endX; x++)
            {
                for (var y = bottomLeftCorner.Y; y < endY; y++)
                {
                    Data[x, y] = fillChar;
                }
            }
        }
    }
}
