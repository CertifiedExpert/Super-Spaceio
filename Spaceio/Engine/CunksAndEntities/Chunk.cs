using Spaceio;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SuperSpaceio.Engine
{
    [DataContract(IsReference = true)]
    class Chunk
    {
        [DataMember]
        public List<GameObject>[] gameObjectRenderLists;
        [DataMember]
        public Dictionary<UID, GameObject> gameObjects = new Dictionary<UID, GameObject>();

        [DataMember]
        public DateTime lastUnloaded = DateTime.MinValue;
        public Vec2i Index { get; set; }
        public Engine Engine { get; set; }
        public Chunk(Vec2i index, Engine engine)
        {
            Engine = engine;

            gameObjectRenderLists = new List<GameObject>[Engine.Settings.spriteLevelCount];
            for (var i = 0; i < gameObjectRenderLists.Length; i++)
            {
                gameObjectRenderLists[i] = new List<GameObject>();
            }

            Index = index;
        }
        // Completes missing data in the chunk after serialization and calls to complete data in all GameObjects residing in it.
        public void CompleteDataAfterSerialization(Engine engine, Vec2i index)
        {
            Engine = engine;
            Index = index;
            gameObjectsToAdd = new List<GameObject>();
            gameObjectsToRemove = new List<GameObject>();

            foreach (var gameObject in gameObjects)
            {
                gameObject.CompleteDataAfterDeserialization(engine, index);
            }
        }

        // Gets called when the chunk gets loaded.
        public void OnChunkLoaded()
        {

        }
        // Gets called when the chunk gets unloaded.
        public void OnChunkUnloaded()
        {
            lastUnloaded = DateTime.Now;
        }
    }
}
