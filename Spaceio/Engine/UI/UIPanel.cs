using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Spaceio.Engine.UI;

namespace Spaceio.Engine
{
    abstract class UIPanel
    {
        public Vec2i Position { get; private set; }
        public Vec2i Size { get; private set; }
        public int Priority { get; private set; }

        private List<UIComponent> _uiComponents;
        public ObservableCollection<UIComponent> UIComponents { get; }
        
        
        private List<UIPanel> _uiPanels;
        public ObservableCollection<UIPanel> UIPanels { get; }
        

        public UIPanel()
        {
            _uiComponents = new List<UIComponent>();
            UIComponents = new ObservableCollection<UIComponent>(_uiComponents);
            _uiPanels = new List<UIPanel>();
            UIPanels = new ObservableCollection<UIPanel>(_uiPanels);
        }

        public Sprite GetDrawnPanelSprite()
        {
            var bitmap = new Bitmap(new char[Size.X, Size.Y]);

            // Draw the parent
            AdditionalDrawPanelToBitmapInstructions(bitmap);
            foreach (var uiComponent in UIComponents) uiComponent.DrawComponentToBitmap(bitmap);
            
            // Draw the children (starting with minimal priority)
            var sortedPanels = SortChildrenUIPanelsByPriority(this);
            foreach (var panel in sortedPanels)
            {
                panel.AdditionalDrawPanelToBitmapInstructions(bitmap);
                foreach (var uiComponent in panel.UIComponents) uiComponent.DrawComponentToBitmap(bitmap);
            }

            return new Sprite(bitmap);
        }
        protected static IEnumerable<UIPanel> SortChildrenUIPanelsByPriority(UIPanel uiPanel)
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

        protected virtual void AdditionalDrawPanelToBitmapInstructions(Bitmap bitmap) { }

    }
}
