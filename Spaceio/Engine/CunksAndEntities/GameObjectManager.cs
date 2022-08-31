using System.Runtime.Serialization;

namespace Spaceio.Engine
{
    [DataContract]
    class GameObjectManager
    {
        private Engine Engine { get; set; }

        public GameObjectManager(Engine engine)
        {
            Engine = engine;
        }
        
        public void Update()
        {
            foreach (var chunk in Engine.ChunkManager.loadedChunks)
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
            var chunkX = gameObject.Position.X / Engine.Settings.chunkSize;
            var chunkY = gameObject.Position.Y / Engine.Settings.chunkSize;

            gameObject.Chunk = Engine.chunks[chunkX, chunkY];
            if (Engine.ChunkManager.IsChunkLoaded(chunkX, chunkY))
                Engine.chunks[chunkX, chunkY].gameObjectsToAdd.Add(gameObject);
            else
                Engine.unloadedChunkTransitionAddGameObjects[chunkX, chunkY].Add(gameObject);
        }

        public void RemoveGameObject(GameObject gameObject)
        {
            if (Engine.ChunkManager.IsChunkLoaded(gameObject.Chunk)) gameObject.Chunk.gameObjectsToRemove.Add(gameObject);
            else
                Engine.unloadedChunkTransitionRemoveGameObjects[gameObject.Chunk.Index.X, gameObject.Chunk.Index.Y].Add(gameObject);
        }

        public void CompleteDataAfterDeserialization(Engine engine)
        {
            Engine = engine;
        }
    }
}