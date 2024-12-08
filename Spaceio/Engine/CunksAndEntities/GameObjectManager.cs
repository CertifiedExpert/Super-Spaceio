using Spaceio;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using SpaceGame;
using System.Xml;
using Microsoft.Win32;

namespace SuperSpaceio.Engine
{
    [DataContract]
    class GameObjectManager
    {
        private Engine Engine { get; set; }

        private List<Tuple<Vec2i, GameObject>> gameObjectsToAddSchedule = new List<Tuple<Vec2i, GameObject>>(); // Vec2i stores to which chunk the gameObject should be added
        private List<GameObject> gameObjectsToRemoveSchedule = new List<GameObject>(); 
        private Dictionary<Vec2i, List<GameObject>> unloadedChunkGOsToAdd = new Dictionary<Vec2i, List<GameObject>>();
        private List<GameObject> movedGameObjects = new List<GameObject>();

        public GameObjectManager(Engine engine)
        {
            Engine = engine;
        }

        /// <summary>
        /// NEW IDEA. DISREGARD CHUNK MOVEMENT DURING UPDATE LOOPS OF GAME OBJECTS. ONLY AFTER IT ENDED CHECK THE POSITIONS. 
        /// (YOU CAN OPTIMIZE BY DOING CHECKING DURING UPDATE LOOPS AND LISTING SUSPECTS. THEN GO OVER THEM AND CHECK POSITIONS, MOVE)
        /// 
        /// THE SAME THING GOES FOR REMOVDING AND ADDING ENTITIES. LIST ADD/REMOVECALLS, LET UPDATE LOOP GO AS NORMAL. ADD AFTER)
        /// </summary>
        
        public void Update()
        {
            // The sequence of foreach loops is vital.

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
                    Engine.ChunkManager.chunks[go.Chunk].gameObjects.Remove(go.UID);
                    Engine.ChunkManager.chunks[go.Chunk].gameObjectRenderLists[go.SpriteLevel].Remove(go);

                    Engine.ChunkManager.chunks[new Vec2i(newChunkX, newChunkY)].gameObjects.Add(go.UID, go);
                    Engine.ChunkManager.chunks[new Vec2i(newChunkX, newChunkY)].gameObjectRenderLists[go.SpriteLevel].Add(go);

                    go.OnChunkTraverse();
                }
            }
            movedGameObjects.Clear();

            foreach (var go in gameObjectsToRemoveSchedule) // Works deespite duplicate calls
            {
                var goExisted = Engine.ChunkManager.chunks[go.Chunk].gameObjects.Remove(go.UID);
                Engine.ChunkManager.chunks[go.Chunk].gameObjectRenderLists[go.SpriteLevel].Remove(go);

                // Retires the UID of the gameObject but ignores duplicate retire calls.
                if (goExisted) Engine.UIDManager.RetireUID(go.UID);
            }
            gameObjectsToRemoveSchedule.Clear();

            foreach (var tpl in gameObjectsToAddSchedule) 
            {
                Engine.ChunkManager.chunks[tpl.Item1].gameObjects.Add(tpl.Item2.UID, tpl.Item2);
                Engine.ChunkManager.chunks[tpl.Item1].gameObjectRenderLists[tpl.Item2.SpriteLevel].Add(tpl.Item2);

                // Updates the Chunk field for objects which were not just added to the engine but moved from other chunks
                tpl.Item2.Chunk = tpl.Item1;
            }
            gameObjectsToAddSchedule.Clear();
        }

        public void AddToMovedGameObjects(GameObject gameObject) => movedGameObjects.Add(gameObject);

        public UID AddGameObject(GameObject gameObject)
        {
            // Generates UID for the gameObject and returns it for use. It is done here during scheduling so that UID can be returned
            var UID = Engine.UIDManager.GenerateUID();
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

        public void RemoveGameObject(UID uID)
        {
            var go = Engine.Find(uID);
            if (go != null)
            {
                gameObjectsToRemoveSchedule.Add(go);
                
                // The UID of the gameObject is retired the frame when it actually is destroyed, not here during scheduling, to prevent duplication
            }
        }

        public void FinishInit() // TODO: when the Engine has created instances of all subsystems, call this function to finish init
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
                    Engine.ChunkManager.chunks[chunkIndex].gameObjects.Add(go.UID, go);
                    Engine.ChunkManager.chunks[chunkIndex].gameObjectRenderLists[go.SpriteLevel].Add(go);
                }
            }
        }

        public void CompleteDataAfterDeserialization(Engine engine)
        {
            Engine = engine;
        }
    }
}