using System.Runtime.Serialization;

namespace Spaceio.Engine
{
    /// <summary>
    /// GoBind is a way for tracking specific GameObject instances in the engine. Due to the serializer returning new identical 
    /// instances of GameObjects instead of returning references to previous GameObjects after deserialization, the GameObject 
    /// fields stored are not bound to their GameObject instances in the engnie. Without using GoBind, after deserializatoin, an example
    /// field "private GameObject gameObject = ..." would hold the reference to an identical, separate instance of GameObject 
    /// than the one which is stored in the engine, causing any changes to the "gameObject" not to be reflected in the engine. 
    /// Instead, what can be done is to create a GoBind field ex. "private GoBind gameObject = ..." and access its "Val" property
    /// to get the always correct GameObject reference.
    /// 
    /// NOTE: before accessing "Val" property of GoBind, it must always be made sure that the GoBind is active by checking 
    /// if IsActive equals to true. A GoBind is not active when the referenced GameObject is unloaded, preventing making changes
    /// to an unloaded GameObject.
    /// </summary>
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
        // Sets the stored field to the stored value
        private void RebindProperty()
        {
            if (varType == VarType.Field) engine.GetType().GetField(name).SetValue(engine, this);
            else engine.GetType().GetProperty(name).SetValue(engine, this);
        }
        // Called when its GameObject is deserialized and needs to have its references rebound.
        public void OnDeserialized(GameObject gameObject)
        {
            engine = gameObject.Engine;
            Val = gameObject;

            IsActive = true;
            RebindProperty();
        }
        // Removes the GoBind and sets its values to null to signal it has been deleted.
        public void Remove()
        {
            Val.Binds.Remove(this);
            Val = null;
            IsActive = false;
            engine = null;
            name = null;
        }
    }

    public enum VarType
    {
        Field,
        Property
    }
}
