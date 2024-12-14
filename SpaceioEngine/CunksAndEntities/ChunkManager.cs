using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract]
    public class ChunkManager //TODO: replace Tuple<int, int> with Vec2i. It should be more readable. Do so also in GameObject class for the Chunk field
    {
        private Engine Engine { get; set; }

        private Dictionary<Vec2i, Chunk> _chunks = new Dictionary<Vec2i, Chunk>();
        public ReadOnlyDictionary<Vec2i, Chunk> chunks { get; private set; } //TODO: change to private and write access functions

        private List<Chunk> _loadedChunks = new List<Chunk>();
        public ReadOnlyCollection<Chunk> loadedChunks { get; private set; } // A list of all chunks which are loaded. Can be used instead of chunk dictionary during interaction for the convenience of not checking if the chunk is loaded.
        
        private List<Vec2i> chunksToBeAddedToLoadedChunks = new List<Vec2i>();
        private List<Vec2i> chunksToBeRemovedFromLoadedChunks = new List<Vec2i>();
        private List<Vec2i> chunksToBeUnloaded = new List<Vec2i>(); // List of chunks which have been scheduled to be unloaded.
        private List<Vec2i> chunksToBeLoaded = new List<Vec2i>();

        public delegate void ChunkLoadedEventHandler(Vec2i chunkIndex);
        public event ChunkLoadedEventHandler ChunkLoaded; // Subscribe to this if you want to add changes to chunk after it has been loaded (like ex. growing plants by the amount of time which has passed since Chunks.LastUnloaded.)
        internal event EventHandler ChunkLoadingEnded; 
        public ChunkManager(Engine engine)
        {
            Engine = engine;
            loadedChunks = new ReadOnlyCollection<Chunk>(_loadedChunks);
            chunks = new ReadOnlyDictionary<Vec2i, Chunk>(_chunks);
        }

        internal ChunkManager(Engine engine, ChunkManagerSaveData saveData)
        {
            Engine = engine;
            loadedChunks = new ReadOnlyCollection<Chunk>(_loadedChunks);
            chunks = new ReadOnlyDictionary<Vec2i, Chunk>(_chunks);

            foreach (var index in saveData.Indexes)
            {
                _chunks.Add(index, null);
            }
        }
        internal void Update()
        {
            foreach (var v in chunksToBeUnloaded) UnloadChunk(v);
            chunksToBeUnloaded.Clear();

            foreach (var v in chunksToBeLoaded) LoadChunk(v);
            chunksToBeLoaded.Clear();

            foreach (var v in chunksToBeAddedToLoadedChunks) _loadedChunks.Add(chunks[v]);
            chunksToBeAddedToLoadedChunks.Clear();
            foreach (var v in chunksToBeRemovedFromLoadedChunks) _loadedChunks.Remove(chunks[v]);
            chunksToBeRemovedFromLoadedChunks.Clear();

            ChunkLoadingEnded?.Invoke(this, EventArgs.Empty);
        }

        public virtual void GenerateEmptyChunk(int x, int y)
        {
            if (!WasChunkCreated(x, y))
            {
                var c = new Chunk(new Vec2i(x, y), Engine);
                _chunks.Add(new Vec2i(x, y), c);
            }
            else
            {
                //TODO: handle the case when the chunk already exists and is LOADED or is UNLOADED.
            }
        }

        // Loads the Chunk at specified index from file into the engine.
        private void LoadChunk(Vec2i index)
        {
            var bytes = Engine.SaveFileManager.LoadChunkBytes(index);

            var saveData = Serializer.FromXmlBytes<ChunkSaveData>(bytes);
            var chunk = new Chunk(Engine, index, saveData);

            _chunks[index] = chunk;
            _loadedChunks.Add(chunk);

            ChunkLoaded?.Invoke(index);
        }

        private void UnloadChunk(Vec2i index)
        {
            var chunk = chunks[index];
            chunk.LastUnloaded = DateTime.Now;

            var saveData = chunk.GetSaveData();

            var bytes = Serializer.ToXmlBytes(saveData);
            Engine.SaveFileManager.SaveChunkBytes(bytes, index);
        }

        public void ScheduleUnloadChunk(int x, int y)
        {
            if (IsChunkLoaded(x, y) && !chunksToBeUnloaded.Contains(new Vec2i(x, y)))
            {
                chunksToBeUnloaded.Add(new Vec2i(x, y));
                chunksToBeRemovedFromLoadedChunks.Add(new Vec2i(x, y));
            }
        }
        public void ScheduleUnloadChunk(Vec2i v) => ScheduleUnloadChunk(v.X , v.Y);

        public void ScheduleLoadChunk(int x, int y)
        {
            if (WasChunkCreated(x, y) && !IsChunkLoaded(x, y))
            {
                chunksToBeLoaded.Add(new Vec2i(x, y));
                chunksToBeAddedToLoadedChunks.Add(new Vec2i(x, y));
            }
        }
        public void ScheduleLoadChunk(Vec2i v) => ScheduleLoadChunk(v.X , v.Y);

        internal void ForceUnloadChunk(int x, int y)
        {
            UnloadChunk(new Vec2i(x, y));
        }

        // Checks if the chunk is loaded and returns the result as a bool.
        public bool IsChunkLoaded(int x, int y)
        {
            if (WasChunkCreated(x, y) && chunks[new Vec2i(x, y)] != null) return true;
            return false; //TODO: decide what to do when the chunks has never been created
        }
        public bool IsChunkLoaded(Vec2i v) => IsChunkLoaded(v.X, v.Y);

        public bool WasChunkCreated(int x, int y)
        { 
            if (chunks.ContainsKey(new Vec2i(x, y))) return true;
            return false;
        }
        public bool WasChunkCreated(Vec2i v) => WasChunkCreated(v.X, v.Y);

        internal ChunkManagerSaveData GetSaveData()
        {
            var sd = new ChunkManagerSaveData();
            sd.Indexes = new List<Vec2i>();
            foreach (var index in chunks.Keys)
            {
                sd.Indexes.Add(index);
            }
            return sd;
        }
    }
}
