using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;

namespace SuperSpaceio.Engine
{
    [DataContract]
    class ChunkManager
    {
        private Engine Engine { get; set; }

        private Dictionary<Tuple<int, int>, Chunk> chunks = new Dictionary<Tuple<int, int>, Chunk>(); //TODO: change to private and write access functions

        private List<Chunk> _loadedChunks = new List<Chunk>();
        public ReadOnlyCollection<Chunk> loadedChunks { get; private set; } // A list of all chunks which are loaded. Can be used instead of chunk dictionary during interaction for the convenience of not checking if the chunk is loaded.
        private List<Tuple<int, int>> chunksToBeAddedToLoadedChunks = new List<Tuple<int, int>>();
        private List<Tuple<int, int>> chunksToBeRemovedFromLoadedChunks = new List<Tuple<int, int>>();
        private List<Tuple<int, int>> chunksToBeUnloaded = new List<Tuple<int, int>>(); // List of chunks which have been scheduled to be unloaded.
        private List<Tuple<int, int>> chunksToBeLoaded = new List<Tuple<int, int>>();

        public ChunkManager(Engine engine)
        {
            Engine = engine;
            loadedChunks = new ReadOnlyCollection<Chunk>(_loadedChunks);
        }
        public void Update()
        {
            foreach (var index in chunksToBeUnloaded) UnloadChunk(index.Item1, index.Item2);
            chunksToBeUnloaded.Clear();

            foreach (var index in chunksToBeLoaded) LoadChunk(index.Item1, index.Item2);
            chunksToBeLoaded.Clear();

            foreach (var index in chunksToBeAddedToLoadedChunks) _loadedChunks.Add(chunks[index]);
            chunksToBeAddedToLoadedChunks.Clear();
            foreach (var index in chunksToBeRemovedFromLoadedChunks) _loadedChunks.Remove(chunks[index]);
            chunksToBeRemovedFromLoadedChunks.Clear();
        }

        public virtual void GenerateEmptyChunk(int x, int y)
        {
            if (!WasChunkCreated(x, y))
            {
                var c = new Chunk(new Vec2i(x, y), Engine);
                chunks.Add(new Tuple<int, int>(x,y), c);
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
            var c = chunks[new Tuple<int, int>(x, y)];
            c.OnChunkUnloaded();


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
            if (IsChunkLoaded(x, y) && !chunksToBeUnloaded.Contains(new Tuple<int, int>(x, y)))
            {
                chunksToBeUnloaded.Add(new Tuple<int, int>(x, y));
                chunksToBeRemovedFromLoadedChunks.Add(new Tuple<int, int>(x, y));
            }
        }
        public void ScheduleLoadChunk(int x, int y)
        {
            if (WasChunkCreated(x, y) && !IsChunkLoaded(x, y))
            {
                chunksToBeLoaded.Add(new Tuple<int, int>(x, y));
                chunksToBeAddedToLoadedChunks.Add(new Tuple<int, int>(x, y));
            }
        }

        public void ForceUnloadChunk(int x, int y)
        {
            UnloadChunk(x, y);
        }

        // Checks if the chunk is loaded and returns the result as a bool.
        public bool IsChunkLoaded(int x, int y)
        {
            if (WasChunkCreated(x, y) && chunks[new Tuple<int, int>(x, y)] != null) return true;
            return false; //TODO: decide what to do when the chunks has never been created
        }
        public bool WasChunkCreated(int x, int y)
        { 
            if (chunks.ContainsKey(new Tuple<int, int>(x, y))) return true;
            return false;
        }

        public void CompleteDataAfterDeserialization(Engine engine)
        {
            Engine = engine;

            _loadedChunks = new List<Chunk>();
            loadedChunks = new ReadOnlyCollection<Chunk>(_loadedChunks);
            
            chunksToBeAddedToLoadedChunks = new List<Tuple<int, int>>();
            chunksToBeRemovedFromLoadedChunks = new List<Tuple<int, int>>();
            chunksToBeUnloaded = new List<Tuple<int, int>>();
            chunksToBeLoaded = new List<Tuple<int, int>>();
        }
    }
}
