using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Spaceio.Engine
{
    [DataContract]
    class ChunkManager
    {
        private Engine Engine { get; set; }

        private List<Chunk> _loadedChunks;
        public ReadOnlyCollection<Chunk> loadedChunks { get; private set; } // A list of all chunks which are loaded. Can be used instead of chunks[,] during interaction for the convenience of not checking if the chunk is loaded.
        private List<Chunk> chunksToBeAddedToLoadedChunks = new List<Chunk>();
        private List<Chunk> chunksToBeRemovedFromLoadedChunks = new List<Chunk>();
        private List<Chunk> chunksToBeUnloaded = new List<Chunk>(); // List of chunks which have been scheduled to be unloaded.

        public ChunkManager(Engine engine)
        {
            this.Engine = engine;

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
            Engine.chunks[x, y] = Engine.Serializer.FromFile<Chunk>($"{Engine.pathCurrentLoadedSave}\\Chunks\\chunk{x}_{y}");
            
            // Fill missing data
            Engine.chunks[x, y].CompleteDataAfterSerialization(Engine, new Vec2i(x, y));

            chunksToBeAddedToLoadedChunks.Add(Engine.chunks[x, y]);
            Engine.chunks[x, y].OnChunkLoaded();

            // Integrate traverse GameObjects into the chunk and inform them of the newly loaded state (or delete them)
            foreach (var gameObject in Engine.unloadedChunkTransitionRemoveGameObjects[x, y])
            {
                Engine.chunks[x, y].gameObjectsToRemove.Add(gameObject);
            }
            foreach (var gameObject in Engine.unloadedChunkTransitionAddGameObjects[x, y])
            {
                Engine.chunks[x, y].gameObjectsToAdd.Add(gameObject);
            }
            foreach (var gameObject in Engine.unloadedChunkTransitionAddGameObjects[x, y])
            {
                gameObject.OnUnloadedChunkAwake(x, y);
            }
            Engine.unloadedChunkTransitionRemoveGameObjects[x, y].Clear();
            Engine.unloadedChunkTransitionAddGameObjects[x, y].Clear();
        }
        // Schedules the Chunk to be unloaded at the at the beginning of the next frame.
        public void ScheduleUnloadChunk(int x, int y)
        {
            if (IsChunkLoaded(Engine.chunks[x, y]) && !chunksToBeUnloaded.Contains(Engine.chunks[x, y]))
            {
                chunksToBeUnloaded.Add(Engine.chunks[x, y]);
                chunksToBeRemovedFromLoadedChunks.Add(Engine.chunks[x, y]);
                Engine.chunks[x, y].OnChunkUnLoaded();
            }
        }
        public void ForceUnloadChunk(int x, int y)
        {
            UnloadChunk(x, y);
        }
        // Unloads the Chunk at specified index to file and deletes it from the engine.
        private void UnloadChunk(int x, int y)
        {
            foreach (var gameObject in Engine.chunks[x, y].gameObjects)
            {
                gameObject.PrepareForSerialization();
            }

            Engine.Serializer.ToFile(Engine.chunks[x, y], $"{Engine.pathCurrentLoadedSave}\\Chunks\\chunk{Engine.chunks[x, y].Index.X}_{Engine.chunks[x, y].Index.Y}");
            Engine.chunks[Engine.chunks[x, y].Index.X, Engine.chunks[x, y].Index.Y] = null;
        }

        // Checks if the chunk is loaded and returns the result as a bool.
        public bool IsChunkLoaded(int x, int y)
        {
            if (Engine.chunks[x, y] != null) return true;
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
            Engine = engine;

            _loadedChunks = new List<Chunk>();
            loadedChunks = new ReadOnlyCollection<Chunk>(_loadedChunks);
            
            chunksToBeAddedToLoadedChunks = new List<Chunk>();
            chunksToBeRemovedFromLoadedChunks = new List<Chunk>();
            chunksToBeUnloaded = new List<Chunk>();
        }
    }
}
