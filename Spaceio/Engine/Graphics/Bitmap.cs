using System;
using System.Configuration;
using System.Data.SqlTypes;

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
            DrawFilledRectangle(new Vec2i(0, 0), Size, fillChar);
        }

        // Draws a line onto its bitmap. Including the startPoint, including the endPoint. NOTE: in large lines the shared end or start points might not match due to imprecision in drawing lines!
        public void DrawLine(Vec2i startPoint, Vec2i endPoint, char fillChar)
        {
            Vec2i line = endPoint + new Vec2i(1, 1) - startPoint;
            int stepCount = Math.Max(Math.Abs(line.X), Math.Abs(line.Y));
            Vec2f step = new Vec2f((float)line.X / stepCount, (float)line.Y / stepCount);

            var leftover = new Vec2f(0, 0);
            var lastMove = startPoint.Copy();
            var currentMove = startPoint.Copy();

            for (var i = 0; i < stepCount; i++)
            {
                leftover += step;
                if (leftover.X >= 1)
                {
                    leftover.X -= 1;
                    currentMove.X += 1;
                }
                else if (leftover.X <= -1)
                {
                    leftover.X += 1;
                    currentMove.X -= 1;
                }
                if (leftover.Y >= 1)
                {
                    leftover.Y -= 1;
                    currentMove.Y += 1;
                }
                else if (leftover.Y <= -1)
                {
                    leftover.Y += 1;
                    currentMove.Y -= 1;
                }

                Data[lastMove.X, lastMove.Y] = fillChar;

                lastMove = currentMove.Copy();
            }
        }
        public void DrawRectangleOutline(Vec2i bottomLeftCorner, Vec2i size, char fillChar)
        {
            DrawLine(bottomLeftCorner, new Vec2i(bottomLeftCorner.X, bottomLeftCorner.Y + size.Y), fillChar);
            DrawLine(bottomLeftCorner, new Vec2i(bottomLeftCorner.X + size.X, bottomLeftCorner.Y), fillChar);
            DrawLine(new Vec2i(bottomLeftCorner.X, bottomLeftCorner.Y + size.Y), new Vec2i(bottomLeftCorner.X + size.X, bottomLeftCorner.Y + size.Y), fillChar);
            DrawLine(new Vec2i(bottomLeftCorner.X + size.X, bottomLeftCorner.Y), new Vec2i(bottomLeftCorner.X + size.X, bottomLeftCorner.Y + size.Y), fillChar);
        }
        public void DrawFilledRectangle(Vec2i bottomLeftCorner, Vec2i size, char fillChar)
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
