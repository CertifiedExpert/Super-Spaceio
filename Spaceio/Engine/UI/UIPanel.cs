using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Spaceio.Engine.UI;

namespace Spaceio.Engine
{
    abstract class UIPanel
    {
        public Engine Engine { get; private set; }
        public Vec2i Position { get; private set; }
        public Vec2i Size { get; private set; }
        public int Priority { get; private set; }

        private List<UIComponent> _uiComponents;
        public ReadOnlyCollection<UIComponent> UIComponents { get; }
        
        
        private List<UIPanel> _uiPanels;
        public ReadOnlyCollection<UIPanel> UIPanels { get; }
        
        public UIPanel(Engine engine)
        {
            Engine = engine;
            _uiComponents = new List<UIComponent>();
            UIComponents = _uiComponents.AsReadOnly();
            _uiPanels = new List<UIPanel>();
            UIPanels = _uiPanels.AsReadOnly();
        }

        public virtual void Update()
        {
            foreach (var uiPanel in UIPanels) uiPanel.Update();
            foreach (var uiComponent in UIComponents) uiComponent.Update();
        }

        public void AddUIPanel(UIPanel otherUIPanel)
        {
            if (IsUIPanelOutsideOfCamera())
                throw new UIException("The UIPanel being added was partially or fully outside of its parent UIPanel");
            else _uiPanels.Add(otherUIPanel);
        }
        public void RemoveUIPanel(UIPanel otherUIPanel)
        {
            _uiPanels.Remove(otherUIPanel);
        }
        public void AddUIComponent(UIComponent uiComponent)
        {
            if (uiComponent.IsUIComponentOutsideOfPanel(this)) 
                throw new UIException("The UIComponent being added was partially or fully outside of its parent UIPanel");
            else _uiComponents.Add(uiComponent);
        }
        public void RemoveUIComponent(UIComponent uiComponent)
        {
            _uiComponents.Remove(uiComponent);
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

        public Sprite GetParentPanelSprite()
        {
            var bitmap = new Bitmap(new char[Size.X, Size.Y]);

            // Creates a non-transparent background.
            bitmap.FillWith(' ');
            
            // Draw the parent
            DrawPanelToBitmap(bitmap);
            
            // Draw the children (starting with minimal priority)
            var sortedPanels = SortChildrenUIPanelsByPriority(this);
            foreach (var panel in sortedPanels) panel.DrawPanelToBitmap(bitmap);

            return new Sprite(bitmap);
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
