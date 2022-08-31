using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Spaceio.Engine
{
    [DataContract]
    class ChunkManager
    {
        private Engine engine { get; set; }

        private List<Chunk> _loadedChunks;
        public ReadOnlyCollection<Chunk> loadedChunks { get; private set; } // A list of all chunks which are loaded. Can be used instead of chunks[,] during interation for the conveniance of not checking if the chunk is loaded.
        private List<Chunk> chunksToBeAddedToLoadedChunks = new List<Chunk>();
        private List<Chunk> chunksToBeRemovedFromLoadedChunks = new List<Chunk>();
        private List<Chunk> chunksToBeUnloaded = new List<Chunk>(); // List of chunks which have been scheduled to be unloaded.

        public ChunkManager(Engine engine)
        {
            this.engine = engine;

            _loadedChunks = new List<Chunk>();
            loadedChunks = new ReadOnlyCollection<Chunk>(_loadedChunks);
        }
        public void Update()
        {
            // Update the loadedChunks list.
            foreach (var chunk in chunksToBeAddedToLoadedChunks) _loadedChunks.Add(chunk);
            chunksToBeAddedToLoadedChunks.Clear();
            foreach (var chunk in chunksToBeRemovedFromLoadedChunks) _loadedChunks.Remove(chunk);
            chunksToBeRemovedFromLoadedChunks.Clear();

            // Lazy unloading Chunks.
            foreach (var chunk in chunksToBeUnloaded)
            {
                UnloadChunk(chunk.Index.X, chunk.Index.Y);
            }
            chunksToBeUnloaded.Clear();
        }

        // Loads the Chunk at specified index from file into the engine.
        public virtual void LoadChunk(int x, int y)
        {
            engine.chunks[x, y] = engine.Serializer.FromFile<Chunk>($"{engine.pathCurrentLoadedSave}\\Chunks\\chunk{x}_{y}");
            
            // Fill misssing data
            engine.chunks[x, y].CompleteDataAfterSerialization(engine, new Vec2i(x, y));

            chunksToBeAddedToLoadedChunks.Add(engine.chunks[x, y]);
            engine.chunks[x, y].OnChunkLoaded();

            // Integrate traverse GameObjects into the chunk and inform them of the newly loaded state (or delete them)
            foreach (var gameObject in engine.unloadedChunkTransitionRemoveGameObjects[x, y])
            {
                engine.chunks[x, y].gameObjectsToRemove.Add(gameObject);
            }
            foreach (var gameObject in engine.unloadedChunkTransitionAddGameObjects[x, y])
            {
                engine.chunks[x, y].gameObjectsToAdd.Add(gameObject);
            }
            foreach (var gameObject in engine.unloadedChunkTransitionAddGameObjects[x, y])
            {
                gameObject.OnUnloadedChunkAwake(x, y);
            }
            engine.unloadedChunkTransitionRemoveGameObjects[x, y].Clear();
            engine.unloadedChunkTransitionAddGameObjects[x, y].Clear();
        }
        // Scheudles the Chunk to be unloaded at the at the beginning of the next frame.
        public void ScheduleUnloadChunk(int x, int y)
        {
            if (IsChunkLoaded(engine.chunks[x, y]) && !chunksToBeUnloaded.Contains(engine.chunks[x, y]))
            {
                chunksToBeUnloaded.Add(engine.chunks[x, y]);
                chunksToBeRemovedFromLoadedChunks.Add(engine.chunks[x, y]);
                engine.chunks[x, y].OnChunkUnLoaded();
            }
        }
        public void ForceUnloadChunk(int x, int y)
        {
            UnloadChunk(x, y);
        }
        // Unloads the Chunk at specified index to file and deletes it from the engine.
        private void UnloadChunk(int x, int y)
        {
            foreach (var gameObject in engine.chunks[x, y].gameObjects)
            {
                gameObject.PrepareForSerialization();
            }

            engine.Serializer.ToFile(engine.chunks[x, y], $"{engine.pathCurrentLoadedSave}\\Chunks\\chunk{engine.chunks[x, y].Index.X}_{engine.chunks[x, y].Index.Y}");
            engine.chunks[engine.chunks[x, y].Index.X, engine.chunks[x, y].Index.Y] = null;
        }

        // Checks if the chunk is loaded and returns the result as a bool.
        public bool IsChunkLoaded(int x, int y)
        {
            if (engine.chunks[x, y] != null) return true;
            else return false;
        }
        // Checks if the chunk is loaded and returns the result as a bool.
        public bool IsChunkLoaded(Chunk chunk)
        {
            if (chunk != null) return true;
            else return false;
        }


        public void CompleteDataAfterDeserialization(Engine engine)
        {
            this.engine = engine;

            _loadedChunks = new List<Chunk>();
            loadedChunks = new ReadOnlyCollection<Chunk>(_loadedChunks);
            
            chunksToBeAddedToLoadedChunks = new List<Chunk>();
            chunksToBeRemovedFromLoadedChunks = new List<Chunk>();
            chunksToBeUnloaded = new List<Chunk>();
        }
    }
}
