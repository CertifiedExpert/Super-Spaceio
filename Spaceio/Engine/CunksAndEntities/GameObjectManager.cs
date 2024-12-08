using Spaceio;
using System.Runtime.Serialization;
using System;
using System.Collections.Generic;
using SpaceGame;

namespace SuperSpaceio.Engine
{
    [DataContract]
    class GameObjectManager
    {
        private Engine Engine { get; set; }

        private List<Tuple<Vec2i, GameObject>> gameObjectsToRemove = new List<Tuple<Vec2i, GameObject>>(); // Vec2i stores from which chunk the gameObject should be removed
        private List<Tuple<Vec2i, GameObject>> gameObjectsToAdd = new List<Tuple<Vec2i, GameObject>>(); // Vec2i stores to which chunk the gameObject should be added

        public GameObjectManager(Engine engine)
        {
            Engine = engine;
        }
        
        public void Update()
        {
            foreach (var chunk in Engine.ChunkManager.loadedChunks)
            {
                foreach (var go in chunk.gameObjects.Values) go.Update();
            }

            foreach (var tpl in gameObjectsToAdd)
            {
                Engine.ChunkManager.chunks[tpl.Item1].gameObjects.Add(tpl.Item2.UID, tpl.Item2);
                Engine.ChunkManager.chunks[tpl.Item1].gameObjectRenderLists[tpl.Item2.SpriteLevel].Add(tpl.Item2);
            }
            gameObjectsToAdd.Clear();

            foreach (var tpl in gameObjectsToRemove)
            {
                Engine.ChunkManager.chunks[tpl.Item1].gameObjects.Remove(tpl.Item2.UID);
                Engine.ChunkManager.chunks[tpl.Item1].gameObjectRenderLists[tpl.Item2.SpriteLevel].Remove(tpl.Item2);

                // Retires the UID of the gameObject from the engine
                Engine.UIDManager.RetireUID(tpl.Item2.UID);
            }
            gameObjectsToRemove.Clear();
        }

        public void MoveGameObjectToChunk(GameObject go, int x, int y)
        {
            gameObjectsToRemove.Add(new Tuple<Vec2i, GameObject>(go.Chunk, go));
            go.Chunk = new Vec2i(x, y);

            if (Engine.ChunkManager.IsChunkLoaded(x, y))
            {
                gameObjectsToAdd.Add(new Tuple<Vec2i, GameObject>(new Vec2i(x, y), go));
            }
            else
            {
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
                gameObjectsToAdd.Add(new Tuple<Vec2i, GameObject>(new Vec2i(chunkX, chunkY), gameObject));
            else
                Engine.unloadedChunkTransitionAddGameObjects[chunkX, chunkY].Add(gameObject); //TODO: handle this

            return UID;
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            if (Engine.ChunkManager.IsChunkLoaded(gameObject.Chunk)) 
                gameObjectsToRemove.Add(new Tuple<Vec2i, GameObject>(gameObject.Chunk, gameObject));
            else
                Engine.unloadedChunkTransitionRemoveGameObjects[gameObject.Chunk.Index.X, gameObject.Chunk.Index.Y].Add(gameObject); //TODO: handle this

            // The UID of the gameObject is retired the frame when it actually is destroyed, not here during scheduling, to prevent duplication
        }

        public void CompleteDataAfterDeserialization(Engine engine)
        {
            Engine = engine;
        }
    }
}