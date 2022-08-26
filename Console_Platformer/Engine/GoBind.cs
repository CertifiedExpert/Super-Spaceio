using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;

namespace Console_Platformer.Engine
{
    [DataContract]
    class GoBind
    {
        public bool IsActive { get; set; }
        public GameObject Val { get; private set; }
        private Engine engine;
        [DataMember]
        private VarType varType;
        [DataMember]
        private string name;
        public GoBind(GameObject gameObject, string name, Engine engine, VarType varType = VarType.Field)
        {
            Val = gameObject;
            IsActive = true;
            this.engine = engine;
            this.varType = varType;
            this.name = name;

            Val.Binds.Add(this);
        }

        public void RebindProperty()
        {
            if (varType == VarType.Field) engine.GetType().GetField(name).SetValue(engine, this);
            else engine.GetType().GetProperty(name).SetValue(engine, this);
        }

        public void OnDeserialized(GameObject gameObject)
        {
            engine = gameObject.Engine;
            Val = gameObject;

            IsActive = true;
            RebindProperty();
        }
        public void Remove()
        {
            Val.Binds.Remove(this);
        }
    }

    public enum VarType
    {
        Field,
        Property
    }
}
