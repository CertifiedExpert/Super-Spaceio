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

        private List<Tuple<Vec2i, GameObject>> gameObjectsToRemoveSchedule = new List<Tuple<Vec2i, GameObject>>(); // Vec2i stores from which chunk the gameObject should be removed
        private List<Tuple<Vec2i, GameObject>> gameObjectsToAddSchedule = new List<Tuple<Vec2i, GameObject>>(); // Vec2i stores to which chunk the gameObject should be added
        private Dictionary<Vec2i, List<GameObject>> unloadedChunkGOsToAdd = new Dictionary<Vec2i, List<GameObject>>();

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
            foreach (var chunk in Engine.ChunkManager.loadedChunks)
            {
                foreach (var go in chunk.gameObjects.Values) go.Update();
            }

            foreach (var tpl in gameObjectsToAddSchedule) 
            {
                Engine.ChunkManager.chunks[tpl.Item1].gameObjects.Add(tpl.Item2.UID, tpl.Item2);
                Engine.ChunkManager.chunks[tpl.Item1].gameObjectRenderLists[tpl.Item2.SpriteLevel].Add(tpl.Item2);
            }
            gameObjectsToAddSchedule.Clear();

            foreach (var tpl in gameObjectsToRemoveSchedule) // TODO: Check and remove duplicate calls
            {
                Engine.ChunkManager.chunks[tpl.Item1].gameObjects.Remove(tpl.Item2.UID);
                Engine.ChunkManager.chunks[tpl.Item1].gameObjectRenderLists[tpl.Item2.SpriteLevel].Remove(tpl.Item2);

                // Retires the UID of the gameObject from the engine
                Engine.UIDManager.RetireUID(tpl.Item2.UID);
            }
            gameObjectsToRemoveSchedule.Clear();
        }

        public void MoveGameObjectToChunk(GameObject go, int x, int y)
        {
            gameObjectsToRemoveSchedule.Add(new Tuple<Vec2i, GameObject>(go.Chunk, go));
            go.Chunk = new Vec2i(x, y);

            if (Engine.ChunkManager.IsChunkLoaded(x, y))
            {
                gameObjectsToAddSchedule.Add(new Tuple<Vec2i, GameObject>(new Vec2i(x, y), go));
            }
            else
            {
                unloadedChunkGOsToAdd.Add()

                Engine.unloadedChunkTransitionAddGameObjects[newChunkX, newChunkY].Add(this); //TODO: handle this
            }
        }
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
                Engine.unloadedChunkTransitionAddGameObjects[chunkX, chunkY].Add(gameObject); //TODO: handle this

            return UID;
        }

        public void RemoveGameObject(UID uID)
        {
            var go = Engine.Find(uID);
            if (go != null)
            {
                // prevent double deletion

                // if GO went entered an unloaded or inexistent chunk, still delete this GO and prevent it from being added to the chunk

                if (Engine.ChunkManager.IsChunkLoaded(go.Chunk))
                    gameObjectsToRemoveSchedule.Add(new Tuple<Vec2i, GameObject>(go.Chunk, go));
                else
                {

                }
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