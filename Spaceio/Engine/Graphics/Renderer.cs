using System;
using System.Runtime.Serialization;

namespace Spaceio.Engine
{
    [DataContract(IsReference = true)]
    class Renderer
    {
        private Engine Engine { get; set; }
        
        private char[,] screenBuffer;

        public Renderer(Engine engine)
        {
            Engine = engine;

            // Initialize the screenBuffer.
            screenBuffer = new char[engine.Camera.Size.X, engine.Camera.Size.Y];

            // Fill screenBuffer with background pixels initializing all pixels.
            ClearBuffer(screenBuffer);
        }

        // Writes all GameObjects to the frame buffer and then draws the frame buffer.
        public void Render()
        {
            ClearBuffer(screenBuffer);

            DrawGameObjectsToFrameBuffer();
            DrawDebugLines();

            Draw();
        }

        // Fills the screen buffer with background pixels.
        private void ClearBuffer(char[,] buffer)
        {
            for (var x = 0; x < buffer.GetLength(0); x++)
            {
                for (var y = 0; y < buffer.GetLength(1); y++)
                {
                    buffer[x, y] = Engine.Settings.backgroudPixel;
                }
            }
        }

        // Draws all GameObjects to the screenBuffer.
        private void DrawGameObjectsToFrameBuffer() 
        {
            foreach (var chunk in Engine.ChunkManager.loadedChunks)
            {
                for (var level = Engine.Settings.spriteLevelCount - 1; level >= 0; level--)
                {
                    foreach (var gameObject in chunk.gameObjectRenderLists[level])
                    {
                        WriteSpritesToScreenBuffer(gameObject);
                    }
                }
            }
        }
        // Draws debug lines to the top right portion of the screen.
        private void DrawDebugLines()
        {
            // Draws the debug window with debug lines.
            for (var i = 0; i < Engine.debugLinesCount; i++)
            {
                for (var x = 0; x < Engine.debugLines[i].Length; x++)
                {
                    screenBuffer[x + Engine.Camera.Size.X - Engine.debugLinesLength,
                        Engine.Camera.Size.Y - i * 2 - 1] = Engine.debugLines[i][x];
                }
            }
        }

        // Draws the contents of the screenBuffer to the console with pixelSpacingCharacters in between them to account for the unequal ration of height and width of unicode characters.
        private void Draw()
        {
            Console.SetCursorPosition(0, 0);

            var finalString = string.Empty;
            for (int y = Engine.Camera.Size.Y - 1; y >= 0; y--)
            {
                var line = "";
                for (int x = 0; x < Engine.Camera.Size.X; x++)
                {
                    line += screenBuffer[x, y].ToString();
                    line += Engine.Settings.pixelSpacingCharacters;
                }

                finalString += line + "\n";
            }

            Console.WriteLine(finalString);
        }

        // Writes the GameObject's Sprites into the screenBuffer.
        public void WriteSpritesToScreenBuffer(GameObject gameObject) 
        {
            foreach (var sprite in gameObject.Sprites)
            {
                if (sprite != null)
                {
                    var realPosX = gameObject.Position.X + sprite.AttachmentPos.X - Engine.Camera.Position.X; 
                    var realPosY = gameObject.Position.Y + sprite.AttachmentPos.Y - Engine.Camera.Position.Y;

                    var beginX = (realPosX >= 0) ? 0 : -realPosX; 
                    var endX = (realPosX + sprite.Bitmap.Size.X <= Engine.Camera.Size.X) 
                        ? sprite.Bitmap.Size.X - beginX : Engine.Camera.Size.X - realPosX;

                    var beginY = (realPosY >= 0) ? 0 : -realPosY;
                    var endY = (realPosY + sprite.Bitmap.Size.Y <= Engine.Camera.Size.Y)
                        ? sprite.Bitmap.Size.Y - beginY : Engine.Camera.Size.Y - realPosY;

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

        public void CompleteDataAfterDeserialization(Engine engine)
        {
            Engine = engine;
            screenBuffer = new char[engine.Settings.CameraSizeX, engine.Settings.CameraSizeY];
        }
    }
}
