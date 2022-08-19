using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Engine
{
    class Renderer
    {
        private Engine engine;
        private char[,] screenBuffer;
        private List<GameObject>[] gameObjectRenderLists;

        public Renderer(Engine engine, List<GameObject>[] gameObjectRenderLists)
        {
            this.engine = engine;
            this.gameObjectRenderLists = gameObjectRenderLists;

            // Initialize the screenBuffer
            screenBuffer = new char[engine.Camera.Size.X, engine.Camera.Size.Y];

            // Fill screenBuffer with backgroud pixels initializing all pixels
            ClearBuffer(screenBuffer);
        }

        // Writtes all gameobjects to the framebuffer and then draws the framebuffer
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
            for (var level = engine.spriteLevelCount - 1; level >= 0; level--)
            {
                var gameObjectList = gameObjectRenderLists[level];
                foreach (var gameObject in gameObjectList)
                {
                    WriteSpritesToScreenBuffer(gameObject);
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
                    for (var x = 0; x < sprite.Bitmap.Size.X; x++)
                    {
                        for (var y = 0; y < sprite.Bitmap.Size.Y; y++)
                        {
                            // if pixel is not outside of screenBuffer then add it to the screenbuffer
                            var finalX = gameObject.Position.X + sprite.AttachmentPos.X + x;
                            var finalY = gameObject.Position.Y + sprite.AttachmentPos.Y + y;

                            if (finalX < screenBuffer.GetLength(0) + engine.Camera.Position.X
                                && finalX >= 0 + engine.Camera.Position.X
                                && finalY < screenBuffer.GetLength(1) + engine.Camera.Position.Y
                                && finalY >= 0 + engine.Camera.Position.Y)
                            {
                                if (sprite.Bitmap.Data[x, y] != ' ')
                                    screenBuffer[finalX - engine.Camera.Position.X,
                                    finalY - engine.Camera.Position.Y] = sprite.Bitmap.Data[x, y];
                            }
                        }
                    } 
                } 
            }
        }
    }
}
