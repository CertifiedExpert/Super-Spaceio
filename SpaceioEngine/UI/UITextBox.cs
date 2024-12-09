using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace ConsoleEngine
{
    public class UITextBox : UIComponent
    {
        public string Text { get; set; }
        public char? Outline { get; set; } // Null means no outline.
        public bool SpacedLetters { get; set; }
        public Vec2i TextPosition { get; set; }
        public UITextBox(Vec2i position, Vec2i size, Vec2i textPosition, string text, char? outline = null, bool? spacedLetters = null) : base(position, size)
        {
            Text = text;
            TextPosition = textPosition;
            Outline = outline;
            SpacedLetters = spacedLetters ?? false;
        }
        
        public override void Update()
        {
            
        }

        public override void DrawComponentToBitmap(Bitmap bitmap)
        {
            base.DrawComponentToBitmap(bitmap);
            
            if (Outline != null)
            {
                bitmap.DrawRectangleOutline(Position, Size, Outline.Value);
            }

            if (SpacedLetters)
            {
                var limit = (Size.X - TextPosition.X + 1) / 2;
                limit = Math.Min(limit, Text.Length);
                for (var i = 0; i < limit; i++)
                {
                    bitmap.Data[TextPosition.X + 2 * i, TextPosition.Y] = Text[i];
                }
            }
            else
            {
                var limit = Size.X - TextPosition.X;
                limit = Math.Min(limit, Text.Length);
                for (var i = 0; i < limit; i++)
                {
                    bitmap.Data[TextPosition.X + i, TextPosition.Y] = Text[i];
                }
            }
        }
    }
}
