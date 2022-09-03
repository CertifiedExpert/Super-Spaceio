using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Spaceio.Engine
{
    [DataContract]
    class UIManager
    {
        public Engine Engine { get; private set; }

        [DataMember]
        List<UIPanel> _parentUIPanels;
        public ObservableCollection<UIPanel> ParentUIPanels { get; private set; }

        public UIManager(Engine engine)
        {
            Engine = engine;

            _parentUIPanels = new List<UIPanel>();
            ParentUIPanels = new ObservableCollection<UIPanel>(_parentUIPanels);
        }

        public void CompleteDataAfterDeserialization(Engine engine)
        {
            Engine = engine;
            ParentUIPanels = new ObservableCollection<UIPanel>(_parentUIPanels);
        }
    }
}
