using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public class UIImageBox : UIComponent
    {
        public char? Outline { get; set; }
        public Bitmap Image { get; private set; }
        public Vec2i ImagePosition { get; private set; }
        public UIImageBox(Vec2i position, Vec2i size) : base(position, size) { }

        public override void Update() { }
        
        public void SetImage(Bitmap image, Vec2i imagePosition)
        {
            Image = image;
            ImagePosition = imagePosition;
        }
        
        public override void DrawComponentToBitmap(Bitmap bitmap)
        {
            base.DrawComponentToBitmap(bitmap);

            if (Outline != null) bitmap.DrawRectangleOutline(Position, Size, Outline.Value);

            if (Image != null) bitmap.DrawBitmap(Image, Position + ImagePosition, Size);
        }
    }
}
