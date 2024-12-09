using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public abstract class UIComponent // TODO: clean up access modifyers if needed
    {
        public Vec2i Position { get; private set; }
        public Vec2i Size { get; private set; }
        public char Background { get; set; }

        public abstract void Update();

        public virtual void DrawComponentToBitmap(Bitmap bitmap) { }

        public UIComponent(Vec2i position, Vec2i size)
        {
            Position = position;
            Size = size;
        }

        public void Validate(UIPanel parent)
        {
            if (IsUIComponentOutsideOfPanel(parent))
                throw new UIException("The UIComponent being added was partially or fully outside of its parent UIPanel");
        }
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
