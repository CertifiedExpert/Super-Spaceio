using System.Runtime.Serialization;

namespace Spaceio.Engine
{
    [DataContract]
    class GameObjectManager
    {
        private Engine engine;

        public GameObjectManager(Engine engine)
        {
            this.engine = engine;
        }

        public void Update()
        {
            foreach (var chunk in engine.ChunkManager.loadedChunks)
            {
                foreach (var gameObject in chunk.gameObjectsToAdd)
                {
                    chunk.gameObjects.Add(gameObject);
                    chunk.gameObjectRenderLists[gameObject.SpriteLevel].Add(gameObject);
                }
                foreach (var gameObject in chunk.gameObjectsToRemove)
                {
                    chunk.gameObjects.Remove(gameObject);
                    chunk.gameObjectRenderLists[gameObject.SpriteLevel].Remove(gameObject);
                }

                chunk.gameObjectsToAdd.Clear();
                chunk.gameObjectsToRemove.Clear();

                foreach (var gameObject in chunk.gameObjects)
                {
                    gameObject.Update();
                }
            }
        }

        public void AddGameObject(GameObject gameObject)
        {
            var chunkX = gameObject.Position.X / engine.Settings.chunkSize;
            var chunkY = gameObject.Position.Y / engine.Settings.chunkSize;

            gameObject.Chunk = engine.chunks[chunkX, chunkY];
            if (engine.ChunkManager.IsChunkLoaded(chunkX, chunkY))
                engine.chunks[chunkX, chunkY].gameObjectsToAdd.Add(gameObject);
            else
                engine.unloadedChunkTransitionAddGameObjects[chunkX, chunkY].Add(gameObject);
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            if (engine.ChunkManager.IsChunkLoaded(gameObject.Chunk)) gameObject.Chunk.gameObjectsToRemove.Add(gameObject);
            else
                engine.unloadedChunkTransitionRemoveGameObjects[gameObject.Chunk.Index.X, gameObject.Chunk.Index.Y].Add(gameObject);
        }

        public void CompleteDataAfterDeserialization(Engine engine)
        {
            this.engine = engine;
        }
    }
}