using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spaceio.Engine.UI
{
    abstract class UIComponent
    {
        public Vec2i Position { get; private set; }
        public Vec2i Size { get; }

        public abstract void Update();
        public abstract void DrawComponentToBitmap(Bitmap bitmap);

        public bool IsUIComponentOutsideOfPanel(UIPanel uiPanel)
        {
            if (Position.X < uiPanel.Position.X ||
                Position.X + Size.X > uiPanel.Position.X + uiPanel.Size.X ||
                Position.Y < uiPanel.Position.Y ||
                Position.Y + Size.Y > uiPanel.Position.Y + uiPanel.Size.Y)
            {
                return true;
            }

            return false;
        }
    }
}
