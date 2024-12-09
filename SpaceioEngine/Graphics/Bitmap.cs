using System;
using System.Configuration;
using System.Data.SqlTypes;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleEngine
{
    public class Bitmap
    {
        public ReadOnlyVec2i Size { get; private set; }
        public char[,] Data { get; set; } // TODO: consider changing this to a 1-dimensional array. Then you can make it read only.
        public Bitmap(char[,] data)
        {
            Size = new ReadOnlyVec2i(data.GetLength(0), data.GetLength(1));
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
            var lastMove = startPoint;
            var currentMove = startPoint;

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

                lastMove = currentMove;
            }
        }
        public void DrawRectangleOutline(Vec2i bottomLeftCorner, Vec2i size, char fillChar)
        {
            DrawLine(bottomLeftCorner, new Vec2i(bottomLeftCorner.X, bottomLeftCorner.Y + size.Y - 1), fillChar);
            DrawLine(bottomLeftCorner, new Vec2i(bottomLeftCorner.X + size.X - 1, bottomLeftCorner.Y), fillChar);
            DrawLine(new Vec2i(bottomLeftCorner.X, bottomLeftCorner.Y + size.Y - 1), new Vec2i(bottomLeftCorner.X + size.X - 1, bottomLeftCorner.Y + size.Y - 1), fillChar);
            DrawLine(new Vec2i(bottomLeftCorner.X + size.X - 1, bottomLeftCorner.Y), new Vec2i(bottomLeftCorner.X + size.X - 1, bottomLeftCorner.Y + size.Y - 1), fillChar);
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
        public void DrawBitmap(Bitmap bitmap, Vec2i position)
        {
            var endX = position.X + bitmap.Size.X;
            var endY = position.Y + bitmap.Size.Y;
            if (endX > Size.X) endX = Size.X;
            if (endY > Size.Y) endY = Size.Y;
            
            for (var x = position.X; x < endX; x++)
            {
                for (var y = position.Y; y < endY; y++)
                {
                    Data[x, y] = bitmap.Data[x - position.X, y - position.Y];
                }
            }
        }

        // drawableSize is the size of the drawable area. If the bitmap is larger than the drawable area, it will be cropped. If the bitmap is smaller than the drawable area, it will be drawn anchored to the bottom left corner.
        public void DrawBitmap(Bitmap bitmap, Vec2i position, Vec2i drawableSize)
        {
            var endX = position.X + drawableSize.X;
            var endY = position.Y + drawableSize.Y;
            if (endX > Size.X) endX = Size.X;
            if (endY > Size.Y) endY = Size.Y;
            if (endX > position.X + bitmap.Size.X) endX = position.X + bitmap.Size.X;
            if (endY > position.Y + bitmap.Size.Y) endY = position.Y + bitmap.Size.Y;

            for (var x = position.X; x < endX; x++)
            {
                for (var y = position.Y; y < endY; y++)
                {
                    Data[x, y] = bitmap.Data[x - position.X, y - position.Y];
                }
            }
        }

        public void DrawText(string text, Vec2i position, string spacingCharacters = null)
        {
            if (spacingCharacters != null)
            {
                float fLimit = (Size.X - position.X) / (spacingCharacters.Length + 1);
                int limit;
                if (fLimit > (int)fLimit) limit = (int)fLimit + 1;
                else limit = (int)fLimit;

                limit = Math.Min(limit, text.Length);

                for (var i = 0; i < limit; i++)
                {
                    Data[position.X + i * (spacingCharacters.Length + 1), position.Y] = text[i];
                }
            }
            else
            {
                var limit = Math.Min(text.Length, Size.X - position.X);
                for (var i = 0; i < limit; i++)
                {
                    Data[position.X + i, position.Y] = text[i];
                }
            }
        }
    }
}
