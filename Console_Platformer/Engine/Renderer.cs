using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Platformer.Engine
{
    class Renderer
    {
        private Engine engine;
        private char[,] screenBuffer;

        public Renderer(Engine engine)
        {
            this.engine = engine;

            // Initialize the screenBuffer
            screenBuffer = new char[engine.Camera.Size.X, engine.Camera.Size.Y];

            // Fill screenBuffer with backgroud pixels initializing all pixels
            ClearBuffer(screenBuffer);
        }

        // Writes all gameobjects to the framebuffer and then draws the framebuffer
        public void Render()
        {
            ClearBuffer(screenBuffer);

            DrawGameObjectsToFrameBuffer();
            DrawDebugLines();

            Draw();
        }

        // Fills the screen buffer with blank spaces
        private void ClearBuffer(char[,] buffer)
        {
            for (var x = 0; x < buffer.GetLength(0); x++)
            {
                for (var y = 0; y < buffer.GetLength(1); y++)
                {
                    buffer[x, y] = engine.backgroudPixel;
                }
            }
        }

        // Draws all gameobjects to the screenBuffer 
        private void DrawGameObjectsToFrameBuffer() 
        {
            for (var x = 0; x < engine.chunkCountX; x++)
            {
                for (var y = 0; y < engine.chunkCountY; y++)
                {
                    if (engine.IsChunkLoaded(new Vec2i(x, y)))
                    {
                        for (var level = engine.spriteLevelCount - 1; level >= 0; level--)
                        {
                            foreach (var gameObject in engine.chunks[x, y].gameObjectRenderLists[level])
                            {
                                WriteSpritesToScreenBuffer(gameObject);
                            }
                        }
                    }
                }
            }
        }

        private void DrawDebugLines()
        {
            // Draws the debug window with debug lines
            for (var i = 0; i < engine.debugLinesCount; i++)
            {
                for (var x = 0; x < engine.debugLines[i].Length; x++)
                {
                    screenBuffer[x + engine.Camera.Size.X - engine.debugLinesLength,
                        engine.Camera.Size.Y - i * 2 - 1] = engine.debugLines[i][x];
                }
            }
        }

        // Draws the contents of the screenbuffer to the console
        private void Draw()
        {
            Console.SetCursorPosition(0, 0);

            var finalString = string.Empty;
            for (int y = engine.Camera.Size.Y - 1; y >= 0; y--)
            {
                var line = "";
                for (int x = 0; x < engine.Camera.Size.X; x++)
                {
                    line += screenBuffer[x, y].ToString();
                    line += engine.pixelSpacingCharacters;
                }

                finalString += line + "\n";
            }

            Console.WriteLine(finalString);
        }

        

        // Writes the gameobject's sprite into the specified framebuffer
        public void WriteSpritesToScreenBuffer(GameObject gameObject) 
        {
            foreach (var sprite in gameObject.Sprites)
            {
                if (sprite != null)
                {
                    var realPosX = gameObject.Position.X + sprite.AttachmentPos.X - engine.Camera.Position.X; 
                    var realPosY = gameObject.Position.Y + sprite.AttachmentPos.Y - engine.Camera.Position.Y;

                    var beginX = (realPosX >= 0) ? 0 : -realPosX; 
                    var endX = (realPosX + sprite.Bitmap.Size.X <= engine.Camera.Size.X) 
                        ? sprite.Bitmap.Size.X - beginX : engine.Camera.Size.X - realPosX;

                    var beginY = (realPosY >= 0) ? 0 : -realPosY;
                    var endY = (realPosY + sprite.Bitmap.Size.Y <= engine.Camera.Size.Y)
                        ? sprite.Bitmap.Size.Y - beginY : engine.Camera.Size.Y - realPosY;

                    for (var x = 0; x < endX; x++)
                    {
                        for (var y = 0; y < endY; y++)
                        {
                            screenBuffer[realPosX + beginX + x, realPosY + beginY + y] = sprite.Bitmap.Data[beginX + x, beginY + y];  
                        }
                    }
                } 
            }
        }
    }
}
