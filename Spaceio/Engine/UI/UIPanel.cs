using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spaceio.Engine.UI;

namespace Spaceio.Engine
{
    class UIPanel
    {
        public Vec2i Position { get; private set; }
        public Vec2i Size { get; private set; }
        
        private ObservableCollection<UIComponent> _uiComponents;
        public List<UIComponent> UIComponents { get; private set; }

        public UIPanel()
        {
            _uiComponents = new ObservableCollection<UIComponent>(UIComponents);
        }
    }
}
