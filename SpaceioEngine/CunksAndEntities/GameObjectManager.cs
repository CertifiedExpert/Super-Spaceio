using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Win32;

namespace ConsoleEngine
{
    [DataContract]
    public class GameObjectManager
    {
        private Engine Engine { get; set; }
        private UIDManager UIDManager { get; set; }

        private List<Tuple<Vec2i, GameObject>> gameObjectsToAddSchedule = new List<Tuple<Vec2i, GameObject>>(); // Vec2i stores to which chunk the gameObject should be added
        private List<GameObject> gameObjectsToRemoveSchedule = new List<GameObject>(); 
        private Dictionary<Vec2i, List<GameObject>> unloadedChunkGOsToAdd = new Dictionary<Vec2i, List<GameObject>>();
        private List<GameObject> movedGameObjects = new List<GameObject>();

        public GameObjectManager(Engine engine, UIDManager uIDManager)
        {
            Engine = engine;
            UIDManager = uIDManager;
        }
        
        internal GameObjectManager(Engine engine, GameObjectManagerSaveData saveData)
        {
            Engine = engine;
            UIDManager = new UIDManager(saveData.UIDManager);
        }

        internal void Update()
        {
            // The sequence of each foreach loop is vital.

            foreach (var chunk in Engine.ChunkManager.loadedChunks)
            {
                foreach (var go in chunk.gameObjects.Values) go.Update();
            }

            foreach (var go in movedGameObjects)
            {
                var newChunkX = go.Position.X / Engine.Settings.chunkSize;
                var newChunkY = go.Position.Y / Engine.Settings.chunkSize;
                if (go.Chunk != new Vec2i(newChunkX, newChunkY))
                {
                    Engine.ChunkManager.chunks[go.Chunk]._gameObjects.Remove(go.UID);
                    Engine.ChunkManager.chunks[go.Chunk]._gameObjectRenderLists[go.SpriteLevel].Remove(go);

                    Engine.ChunkManager.chunks[new Vec2i(newChunkX, newChunkY)]._gameObjects.Add(go.UID, go);
                    Engine.ChunkManager.chunks[new Vec2i(newChunkX, newChunkY)]._gameObjectRenderLists[go.SpriteLevel].Add(go);

                    go.OnChunkTraverse();
                }
            }
            movedGameObjects.Clear();

            foreach (var go in gameObjectsToRemoveSchedule) // Works deespite duplicate calls
            {
                var goExisted = Engine.ChunkManager.chunks[go.Chunk]._gameObjects.Remove(go.UID);
                Engine.ChunkManager.chunks[go.Chunk]._gameObjectRenderLists[go.SpriteLevel].Remove(go);

                // Retires the UID of the gameObject but ignores duplicate retire calls.
                if (goExisted) UIDManager.RetireUID(go.UID);
            }
            gameObjectsToRemoveSchedule.Clear();

            foreach (var tpl in gameObjectsToAddSchedule) 
            {
                Engine.ChunkManager.chunks[tpl.Item1]._gameObjects.Add(tpl.Item2.UID, tpl.Item2);
                Engine.ChunkManager.chunks[tpl.Item1]._gameObjectRenderLists[tpl.Item2.SpriteLevel].Add(tpl.Item2);

                // Updates the Chunk field for objects which were not just added to the engine but moved from other chunks
                tpl.Item2.Chunk = tpl.Item1;
            }
            gameObjectsToAddSchedule.Clear();
        }

        internal void AddToMovedGameObjects(GameObject gameObject) => movedGameObjects.Add(gameObject);

        internal UID AddGameObject(GameObject gameObject)
        {
            // Generates UID for the gameObject and returns it for use. It is done here during scheduling so that UID can be returned
            var UID = UIDManager.GenerateUID();
            gameObject.SetUID(UID);

            var chunkX = gameObject.Position.X / Engine.Settings.chunkSize; //TODO: make it so that the player at <0,0> starts in the middle of a start chunk, not between 4 chunks
            var chunkY = gameObject.Position.Y / Engine.Settings.chunkSize;

            gameObject.Chunk = new Vec2i(chunkX, chunkY);
            if (Engine.ChunkManager.IsChunkLoaded(chunkX, chunkY))
                gameObjectsToAddSchedule.Add(new Tuple<Vec2i, GameObject>(new Vec2i(chunkX, chunkY), gameObject));
            else
            {
                if (unloadedChunkGOsToAdd.ContainsKey(new Vec2i(chunkX, chunkY))) 
                    unloadedChunkGOsToAdd[new Vec2i(chunkX, chunkY)].Add(gameObject); 
                else
                    unloadedChunkGOsToAdd.Add(new Vec2i(chunkX, chunkY), new List<GameObject> { gameObject });
            }

            return UID;
        }

        internal void RemoveGameObject(UID uID)
        {
            var go = Engine.Find(uID);
            if (go != null)
            {
                gameObjectsToRemoveSchedule.Add(go);
                
                // The UID of the gameObject is retired the frame when it actually is destroyed, not here during scheduling, to prevent duplication
            }
        }

        internal void Init() // TODO: when the Engine has created instances of all subsystems, call this function to finish init
        {
            Engine.ChunkManager.ChunkLoaded += ChunkManager_ChunkLoaded;
            Engine.ChunkManager.ChunkLoadingEnded += ChunkManager_ChunkLoadingEnded;
        }

        private void ChunkManager_ChunkLoadingEnded(object sender, EventArgs e)
        {
            unloadedChunkGOsToAdd.Clear();
        }

        // Triggers when ChunkManager raises a ChunkLoaded event.
        private void ChunkManager_ChunkLoaded(Vec2i chunkIndex)
        {
            if (unloadedChunkGOsToAdd.ContainsKey(chunkIndex))
            {
                foreach (var go in unloadedChunkGOsToAdd[chunkIndex])
                {
                    Engine.ChunkManager.chunks[chunkIndex]._gameObjects.Add(go.UID, go);
                    Engine.ChunkManager.chunks[chunkIndex]._gameObjectRenderLists[go.SpriteLevel].Add(go);
                }
            }
        }

        internal GameObjectManagerSaveData GetSaveData()
        {
            var sd = new GameObjectManagerSaveData();
            sd.UIDManager = UIDManager.GetSaveData();
            return sd;
        }
    }
}