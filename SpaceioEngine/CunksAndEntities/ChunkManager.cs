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

        internal delegate void ChunkLoadedEventHandler(Vec2i chunkIndex);
        internal event ChunkLoadedEventHandler ChunkLoaded; // TODO: add this to LoadChunk() function
        internal event EventHandler ChunkLoadingEnded; 
        public ChunkManager(Engine engine)
        {
            Engine = engine;
            loadedChunks = new ReadOnlyCollection<Chunk>(_loadedChunks);
            chunks = new ReadOnlyDictionary<Vec2i, Chunk>(_chunks);
        }
        internal void Update()
        {
            foreach (var v in chunksToBeUnloaded) UnloadChunk(v.X, v.Y);
            chunksToBeUnloaded.Clear();

            foreach (var v in chunksToBeLoaded) LoadChunk(v.X, v.Y);
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
        private void LoadChunk(int x, int y)
        {
            /*
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

            chunks[new Tuple<int, int>(x, y)].OnChunkLoaded();
            */

            //TODO: IMPLEMENT THIS
            throw new NotImplementedException();
        }

        private void UnloadChunk(int x, int y)
        {
            var c = chunks[new Vec2i(x, y)];
            c.ChunkWasUnloaded();


            // TODO: CHANGE THIS CODE. IMPLEMENT SERIALIZATION!!
            throw new NotImplementedException();

            /*
            foreach (var gameObject in Engine.chunks[x, y].gameObjects)
            {
                gameObject.PrepareForSerialization();
            }

            Engine.Serializer.ToFile(Engine.chunks[x, y], $"{Engine.pathCurrentLoadedSave}\\Chunks\\chunk{Engine.chunks[x, y].Index.X}_{Engine.chunks[x, y].Index.Y}");
            Engine.chunks[Engine.chunks[x, y].Index.X, Engine.chunks[x, y].Index.Y] = null;
            */
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
            UnloadChunk(x, y);
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

        public void CompleteDataAfterDeserialization(Engine engine)
        {
            Engine = engine;

            _loadedChunks = new List<Chunk>();
            loadedChunks = new ReadOnlyCollection<Chunk>(_loadedChunks);
            
            chunksToBeAddedToLoadedChunks = new List<Vec2i>();
            chunksToBeRemovedFromLoadedChunks = new List<Vec2i>();
            chunksToBeUnloaded = new List<Vec2i>();
            chunksToBeLoaded = new List<Vec2i>();
        }
    }
}
