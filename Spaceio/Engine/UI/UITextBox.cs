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
        public char Outline { get; set; }
        public Vec2i TextPosition { get; set; }
        public UITextBox(Vec2i position, Vec2i size, string text) : base(position, size)
        {
            Text = text;
        }
        
        public override void Update()
        {
            
        }

        public override void DrawComponentToBitmap(Bitmap bitmap)
        {
            bitmap.FillWith(Background);
            bitmap.DrawLine(Position, new Vec2i(Position.X + Size.X, Position.Y), Outline);
            bitmap.DrawLine(Position, new Vec2i(Position.X, Position.Y + Size.Y), Outline);
            bitmap.DrawLine(new Vec2i(Position.X, Position.Y + Size.Y), new Vec2i(Position.X + Size.X, Position.Y + Size.Y), Outline);
            bitmap.DrawLine(new Vec2i(Position.X + Size.X, Position.Y), new Vec2i(Position.X + Size.X, Position.Y + Size.Y), Outline);

            for (var i = 0; i < Text.Length; i++)
            {
                bitmap.Data[TextPosition.X + i, TextPosition.Y] = Text[i];
            }
        }
    }
}
