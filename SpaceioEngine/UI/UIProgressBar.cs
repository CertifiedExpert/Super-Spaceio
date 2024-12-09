using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public class UIProgressBar : UIComponent
    {
        public char? Outline { get; set; } // Null means no outline.
        public Vec2i BarPosition { get; set; }
        public Vec2i BarSize { get; set; }
        public char BarBackground { get; set; }

        private float _progress = 0f;
        public float Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                if (_progress < 0)
                    _progress = 0;
                if (_progress > 1)
                    _progress = 1;
            }
        } // 0.0f to 1.0f
        
        public UIProgressBar(Vec2i position, Vec2i size, Vec2i barPosition, Vec2i barSize, char barBackground, char? outline = null) : base(position, size)
        {
            BarPosition = barPosition;
            BarSize = barSize;
            BarBackground = barBackground;
            Outline = outline;
        }

        public override void Update()
        {
    
        }

        public override void DrawComponentToBitmap(Bitmap bitmap)
        {
            base.DrawComponentToBitmap(bitmap);

            if (Outline != null)
            {
                bitmap.DrawRectangleOutline(Position, Size - new Vec2i(1, 1), Outline.Value);
            }

            var size = new Vec2i((int)(BarSize.X * Progress), (int)(BarSize.Y * Progress)); //TODO: round this not cast
            bitmap.DrawFilledRectangle(Position + BarPosition, size, BarBackground);
        }
    }
}
