using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract(IsReference = true)]
    public class Chunk
    {

        [DataMember]
        internal Dictionary<UID, GameObject> _gameObjects = new Dictionary<UID, GameObject>();
        public ReadOnlyDictionary<UID, GameObject> gameObjects { get; private set; }

        [DataMember]
        internal List<GameObject>[] _gameObjectRenderLists;
        public ReadOnlyCollection<ReadOnlyCollection<GameObject>> gameObjectRenderLists { get; private set; }

        [DataMember]
        public DateTime lastUnloaded { get; private set; }
        public Vec2i Index { get; private set; }
        public Engine Engine { get; private set; }
        public Chunk(Vec2i index, Engine engine)
        {
            Engine = engine;

            gameObjects = new ReadOnlyDictionary<UID, GameObject>(_gameObjects);
            
            lastUnloaded = DateTime.MinValue;

            _gameObjectRenderLists = new List<GameObject>[Engine.Settings.spriteLevelCount];
            for (var i = 0; i < _gameObjectRenderLists.Length; i++) _gameObjectRenderLists[i] = new List<GameObject>();

            gameObjectRenderLists = new ReadOnlyCollection<ReadOnlyCollection<GameObject>>(
            Array.ConvertAll(_gameObjectRenderLists, list => new ReadOnlyCollection<GameObject>(list)));

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
        internal void ChunkWasLoaded()
        {

        }
        // Gets called when the chunk gets unloaded.
        internal void ChunkWasUnloaded()
        {
            lastUnloaded = DateTime.Now;
        }
    }
}
