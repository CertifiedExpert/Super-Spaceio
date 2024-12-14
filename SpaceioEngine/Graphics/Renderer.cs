using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleEngine
{
    [DataContract(IsReference = true)]
    public class Renderer
    {
        private Engine Engine { get; set; }
        
        private char[,] screenBuffer; // TODO: need to eventually convert this to 1-dimensional array :(

        public Renderer(Engine engine)
        {
            Engine = engine;
        }

        internal void Init()
        {
            // Initialize the screenBuffer.
            screenBuffer = new char[Engine.Camera.Size.X, Engine.Camera.Size.Y];

            // Fill screenBuffer with background pixels initializing all pixels.
            ClearBuffer(screenBuffer);
        }

        // Writes all GameObjects to the frame buffer and then draws the frame buffer.
        internal void Render()
        {
            ClearBuffer(screenBuffer);

            DrawGameObjectsToFrameBuffer();
            DrawDebugLines();
            DrawUI();

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
            foreach (var chunk in Engine.ChunkManager.loadedChunks) // TODO: check if the chunk is even visible
            {
                for (var level = Engine.Settings.spriteLevelCount - 1; level >= 0; level--)
                {
                    foreach (var gameObject in chunk.gameObjectRenderLists[level])
                    {
                        WriteGameObjectSpritesToScreenBuffer(gameObject);
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
        // Draws the UI of the engine
        private void DrawUI()
        {
            // Order parentUIPanels by descending priority.
            var orderedPanels = Engine.UIManager.ParentUIPanels.OrderByDescending(p => p.Priority);

            foreach (var panel in orderedPanels) WriteParentPanelToScreenBuffer(panel);
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

        // Writes the GameObject's Sprites into the screenBuffer. //TODO: might wanna check if the transparency is preserved because it seems like it is not + set a universal transparency color in Settings.cs for the entire engine
        private void WriteGameObjectSpritesToScreenBuffer(GameObject gameObject) 
        {
            foreach (var sprite in gameObject.Sprites)
            {
                if (sprite != null)
                {
                    var bitmap = Engine.ResourceManager.Bitmaps[sprite.BitmapID]; // TODO: check if the bitmap was retrieved succesfully
                    var realPosX = gameObject.Position.X + sprite.AttachmentPos.X - Engine.Camera.Position.X; 
                    var realPosY = gameObject.Position.Y + sprite.AttachmentPos.Y - Engine.Camera.Position.Y;

                    var beginX = (realPosX >= 0) ? 0 : -realPosX; 
                    var endX = (realPosX + bitmap.Size.X <= Engine.Camera.Size.X) 
                        ? bitmap.Size.X - beginX : Engine.Camera.Size.X - realPosX;

                    var beginY = (realPosY >= 0) ? 0 : -realPosY;
                    var endY = (realPosY + bitmap.Size.Y <= Engine.Camera.Size.Y)
                        ? bitmap.Size.Y - beginY : Engine.Camera.Size.Y - realPosY;

                    for (var x = 0; x < endX; x++)
                    {
                        for (var y = 0; y < endY; y++)
                        {
                            screenBuffer[realPosX + beginX + x, realPosY + beginY + y] = 
                                sprite.Shader.ShaderMethod(beginX + x, beginY + y, bitmap, sprite.Shader.Args);
                        }
                    }
                } 
            }
        }
        // Renders the Sprite of the provided parent UIPanel to the screenBuffer. 
        private void WriteParentPanelToScreenBuffer(UIPanel uiPanel)
        {
            /*
            var sprite = uiPanel.GetParentPanelSprite();
            for (var x = 0; x < sprite.BitmapID.Size.X; x++)
            {
                for (var y = 0; y < sprite.BitmapID.Size.Y; y++)
                {
                    screenBuffer[uiPanel.Position.X + x, uiPanel.Position.Y + y] = sprite.BitmapID.Data[x, y];
                }
            }
            */
        }
    }
}
