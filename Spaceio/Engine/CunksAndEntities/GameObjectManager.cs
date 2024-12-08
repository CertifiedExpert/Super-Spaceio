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

        private List<GameObject> gameObjectsToRemove = new List<GameObject>();
        private List<GameObject> gameObjectsToAdd = new List<GameObject>();

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

            foreach (var go in gameObjectsToAdd)
            {
                Engine.ChunkManager.chunks[go.Chunk].gameObjects.Add(go.UID, go);
                Engine.ChunkManager.chunks[go.Chunk].gameObjectRenderLists[go.SpriteLevel].Add(go);
            }
            gameObjectsToAdd.Clear();

            foreach (var go in gameObjectsToRemove)
            {
                Engine.ChunkManager.chunks[go.Chunk].gameObjects.Remove(go.UID);
                Engine.ChunkManager.chunks[go.Chunk].gameObjectRenderLists[go.SpriteLevel].Remove(go);

                // Retires the UID of the gameObject from the engine
                Engine.UIDManager.RetireUID(go.UID);
            }
            gameObjectsToRemove.Clear();
        }

        public UID AddGameObject(GameObject gameObject)
        {
            // Generates UID for the gameObject and returns it for use. It is done here during scheduling so that UID can be returned
            var UID = Engine.UIDManager.GenerateUID();
            gameObject.SetUID(UID);

            var chunkX = gameObject.Position.X / Engine.Settings.chunkSize; //TODO: make it so that the player at <0,0> starts in the middle of a start chunk, not between 4 chunks
            var chunkY = gameObject.Position.Y / Engine.Settings.chunkSize;

            gameObject.Chunk = new Tuple<int, int>(chunkX, chunkY);
            if (Engine.ChunkManager.IsChunkLoaded(chunkX, chunkY))
                gameObjectsToAdd.Add(gameObject);
            else
                Engine.unloadedChunkTransitionAddGameObjects[chunkX, chunkY].Add(gameObject); //TODO: handle this

            return UID;
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            if (Engine.ChunkManager.IsChunkLoaded(gameObject.Chunk.Item1, gameObject.Chunk.Item2)) 
                gameObjectsToRemove.Add(gameObject);
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