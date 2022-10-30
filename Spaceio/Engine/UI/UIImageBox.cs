using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spaceio.Engine
{
    class UIImageBox : UIComponent
    {
        public char? Outline { get; set; }
        public Bitmap Image { get; set; }
        public Vec2i ImagePosition { get; set; }
        public UIImageBox(Vec2i position, Vec2i size) : base(position, size)
        {
            
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

            
        }
    }
}
