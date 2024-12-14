using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace ConsoleEngine
{
    public class UIPanel
    {
        public Engine Engine { get; private set; }
        public Vec2i Position { get; private set; }
        public Vec2i Size { get; private set; }
        public int Priority { get; private set; }
        public char Background { get; set; }

        private List<UIComponent> _uiComponents;
        public ReadOnlyCollection<UIComponent> UIComponents { get; }
        
        
        private List<UIPanel> _uiPanels;
        public ReadOnlyCollection<UIPanel> UIPanels { get; }
        
        public UIPanel(Vec2i position, Vec2i size, int priority, Engine engine)
        {
            Engine = engine;
            _uiComponents = new List<UIComponent>();
            UIComponents = _uiComponents.AsReadOnly();
            _uiPanels = new List<UIPanel>();
            UIPanels = _uiPanels.AsReadOnly();

            Position = position;
            Size = size;
            Priority = priority;
        }

        public virtual void Update()
        {
            foreach (var uiPanel in UIPanels) uiPanel.Update();
            foreach (var uiComponent in UIComponents) uiComponent.Update();
        }

        public void AddUIPanel(UIPanel childUIPanel)
        {
            childUIPanel.Validate(this);
            _uiPanels.Add(childUIPanel);
        }
        public void RemoveUIPanel(UIPanel childUIPanel)
        {
            _uiPanels.Remove(childUIPanel);
        }
        public void AddUIComponent(UIComponent uiComponent)
        {
            uiComponent.Validate(this);
            _uiComponents.Add(uiComponent);
        }
        public void RemoveUIComponent(UIComponent uiComponent)
        {
            _uiComponents.Remove(uiComponent);
        }

        public void Validate(UIPanel parent = null)
        {
            if (IsUIPanelOutsideOfCamera())
                throw new UIException("The UIPanel being added was partially or fully outside of the engine camera");
            
            if (parent != null)
            {
                if (IsUIPanelOutsideOfParentPanel(parent))
                    throw new UIException("The UIPanel being added was partially or fully outside of the parent UIPanel");
            }
        }
        public bool IsUIPanelOutsideOfCamera()
        {
            if (Position.X < 0 ||
                Position.X + Size.X > Engine.Settings.CameraSizeX ||
                Position.Y < 0 ||
                Position.Y + Size.Y > Engine.Settings.CameraSizeY)
            {
                return true;
            }

            return false;
        }
        public bool IsUIPanelOutsideOfParentPanel(UIPanel parent)
        {
            if (Position.X < parent.Position.X ||
                Position.X + Size.X > parent.Position.X + parent.Size.X ||
                Position.Y < parent.Position.X ||
                Position.Y + Size.Y > parent.Position.Y + parent.Size.Y)
            {
                return true;
            }
            return false;
        }

        
        public Sprite GetParentPanelSprite()
        {
            var bitmap = new Bitmap(new char[Size.X, Size.Y]);

            // Creates a non-transparent background.
            bitmap.FillWith(Background);
            
            // Draw the parent
            DrawPanelToBitmap(bitmap);
            
            // Draw the children (starting with minimal priority)
            var sortedPanels = SortChildrenUIPanelsByPriority(this);
            foreach (var panel in sortedPanels) panel.DrawPanelToBitmap(bitmap);

            //return new Sprite(bitmap);
            return null;
        }
        protected virtual void AdditionalDrawPanelToBitmapInstructions(Bitmap bitmap) { }
        private static IEnumerable<UIPanel> SortChildrenUIPanelsByPriority(UIPanel uiPanel)
        {
            var allUIPanels = new List<UIPanel>();

            var firstUIPanel = uiPanel.UIPanels.FirstOrDefault();
            if (firstUIPanel != null)
            {
                var panelStack = new Stack<UIPanel>();
                panelStack.Push(firstUIPanel);
                
                while (panelStack.Count > 0)
                {
                    var currentPanel = panelStack.Pop();
                    allUIPanels.Add(currentPanel);
                    var childrenRightToLeft = currentPanel.UIPanels.AsEnumerable().Reverse();
                    foreach (var child in childrenRightToLeft)
                    {
                        panelStack.Push(child);
                    }
                } 
            }

            return allUIPanels.OrderByDescending(p => p.Priority);
        }
        private void DrawPanelToBitmap(Bitmap bitmap)
        {
            AdditionalDrawPanelToBitmapInstructions(bitmap);
            foreach (var uiComponent in UIComponents) uiComponent.DrawComponentToBitmap(bitmap);
        }
    }
}
