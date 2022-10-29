using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Spaceio.Engine
{
    class UITextBox : UIComponent
    {
        public string Text { get; set; }
        public char Background { get; set; }
        public char? Outline { get; set; } // Null means no outline.
        public bool SpacedLetters { get; set; }
        public Vec2i TextPosition { get; set; }
        public UITextBox(Vec2i position, Vec2i size, string text, char? outline = null, bool? spacedLetters = null) : base(position, size)
        {
            Text = text;
            Outline = outline;
            SpacedLetters = spacedLetters ?? false;
        }
        
        public override void Update()
        {
            
        }

        public override void DrawComponentToBitmap(Bitmap bitmap)
        {
            if (Outline != null)
            {
                bitmap.FillWith(Background);
                bitmap.DrawRectangleOutline(Position, Size, Outline.Value);
            }

            if (SpacedLetters)
            { 
                for (var i = 0; i < Text.Length; i++)
                {
                    bitmap.Data[TextPosition.X + 2 * i, TextPosition.Y] = Text[i];
                }
            }
            else
            {
                for (var i = 0; i < Text.Length; i++)
                {
                    bitmap.Data[TextPosition.X + i, TextPosition.Y] = Text[i];
                }
            }
        }
    }
}
