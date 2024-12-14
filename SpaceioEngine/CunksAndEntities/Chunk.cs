using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    public class Chunk
    {
        public Engine Engine { get; private set; }
        public Vec2i Index { get; private set; }

        internal Dictionary<UID, GameObject> _gameObjects = new Dictionary<UID, GameObject>();
        public ReadOnlyDictionary<UID, GameObject> gameObjects { get; private set; }

        internal List<GameObject>[] _gameObjectRenderLists;
        public ReadOnlyCollection<ReadOnlyCollection<GameObject>> gameObjectRenderLists { get; private set; }


        public DateTime LastUnloaded { get; internal set; }
        public Chunk(Vec2i index, Engine engine)
        {
            Engine = engine;

            gameObjects = new ReadOnlyDictionary<UID, GameObject>(_gameObjects);
            
            LastUnloaded = DateTime.MinValue;

            _gameObjectRenderLists = new List<GameObject>[Engine.Settings.spriteLevelCount];
            for (var i = 0; i < _gameObjectRenderLists.Length; i++) _gameObjectRenderLists[i] = new List<GameObject>();

            gameObjectRenderLists = new ReadOnlyCollection<ReadOnlyCollection<GameObject>>(
            Array.ConvertAll(_gameObjectRenderLists, list => new ReadOnlyCollection<GameObject>(list)));

            Index = index;
        }

        internal Chunk(Engine engine, Vec2i index, ChunkSaveData saveData)
        {
            Engine = engine;
            Index = index;

            _gameObjectRenderLists = new List<GameObject>[Engine.Settings.spriteLevelCount];
            for (var i = 0; i < _gameObjectRenderLists.Length; i++) _gameObjectRenderLists[i] = new List<GameObject>();
            gameObjects = new ReadOnlyDictionary<UID, GameObject>(_gameObjects);
            gameObjectRenderLists = new ReadOnlyCollection<ReadOnlyCollection<GameObject>>(
            Array.ConvertAll(_gameObjectRenderLists, list => new ReadOnlyCollection<GameObject>(list)));

            LastUnloaded = saveData.lastUnloaded;
            foreach (var gameObjectSaveData in saveData.gameObjects)
            {
                var go = new GameObject(Engine, Index, gameObjectSaveData);
                _gameObjects.Add(go.UID, go);
                _gameObjectRenderLists[go.SpriteLevel].Add(go);
            }
        }

        internal ChunkSaveData GetSaveData()
        {
            var sd = new ChunkSaveData();
            sd.lastUnloaded = LastUnloaded;
            sd.gameObjects = new List<GameObjectSaveData>();
            foreach (var go in _gameObjects.Values)
            {
                var goSd = go.GetSaveData();
                sd.gameObjects.Add(goSd);
            }

            return sd;
        }
        
        // Gets called when the chunk gets loaded.
        internal void ChunkWasLoaded()
        {

        }
    }
}
