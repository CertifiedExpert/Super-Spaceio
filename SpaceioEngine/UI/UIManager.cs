using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    //TODO: serialization (if necessary)
    //TODO: exception and error handling
    [DataContract]
    public class UIManager 
    {
        public Engine Engine { get; private set; }

        //[DataMember]
        List<UIPanel> _parentUIPanels;
        public ReadOnlyCollection<UIPanel> ParentUIPanels { get; private set; }

        public UIManager(Engine engine)
        {
            Engine = engine;

            _parentUIPanels = new List<UIPanel>();
            ParentUIPanels = _parentUIPanels.AsReadOnly();
        }
        public void Update()
        {
            foreach (var parentUIPanel in ParentUIPanels) parentUIPanel.Update();
        }

        public void AddParentUIPanel(UIPanel uiPanel)
        {
            uiPanel.Validate();
            _parentUIPanels.Add(uiPanel);
        }
        public void RemoveParentUIPanel(UIPanel uiPanel)
        {
            _parentUIPanels.Remove(uiPanel);
        }
        
        public void CompleteDataAfterDeserialization(Engine engine)
        {
            Engine = engine;
            //ParentUIPanels = _parentUIPanels.AsReadOnly();
        }
    }
}
