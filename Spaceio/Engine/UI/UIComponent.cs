using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spaceio.Engine.UI
{
    abstract class UIComponent
    {
        public Vec2i Position { get; private set; }
        public Vec2i Size { get; }

        public UIComponent()
        {
            
        }

        public abstract void DrawComponentToBitmap(Bitmap bitmap);
    }
}
