using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;

namespace Console_Platformer.Engine
{
    abstract class Engine
    {//TODO: figure out in which functions the Vec2i parameter is acutally helpful as opposed to passing just 2 int
        //TODO: there are some nested for loops over chunks whereas a foreach loop would possibly work
        //TODO: many times the Engine is called from SpaceGame. Game shoud be called instead
        //TODO: file system. Everything is hardcoded for now.
        // Public variables 
        public bool gameShouldClose = false;
        public int deltaTime = 0; // Miliseconds since last frame
        public string title = "default title";
        public readonly Vec2i worldSize = new Vec2i(chunkCountX * chunkSize, chunkCountY * chunkSize); 
        public const string pixelSpacingCharacters = " ";
        public const char backgroudPixel = ' ';
        public const int spriteLevelCount = 10;
        public const int spriteMaxCount = 10;

        public const int chunkCountX = 100;
        public const int chunkCountY = 100;
        public const int chunkSize = 100;
        public const int chunkLoadRadius = 3; //TODO: move this const to Game.cs
        public readonly Chunk[,] chunks = new Chunk[chunkCountX, chunkCountY];
        public readonly List<GameObject>[,] unloadedChunkTransitionGameObjects = new List<GameObject>[chunkCountX, chunkCountY];
        private readonly List<Chunk> chunksToBeUnloaded = new List<Chunk>();
        public static Random gRandom = new Random();

        //"  " <and> font 20 | width 70 | height 48 <or> font 10 | width 126 | height 90 <or> font 5 | width 316 | height 203
        //" " <and> font 10 | width 189 | height 99
        public Camera Camera { get; private set; } 
        protected Renderer Renderer { get; private set; }
        public ImputManager ImputManager { get; private set; }
        public Serializer serializer { get; private set; }

        private DateTime lastFrame;
        private readonly int milisecondsForNextFrame = 20;

        //debug 
        public readonly bool allowDebug = true;
        public readonly int debugLinesCount = 10;
        public readonly int debugLinesLength = 40;
        public string[] debugLines;

        protected string pathSavesFolder = @"C:\Users\Admin\source\repos\Console_Platformer\Console_Platformer\bin\Debug\Saves";
        protected string pathCurrentLoadedSave;


        // Applicaton loop
        public Engine()
        {
            // Loading 
            OnEngineLoad();
            OnLoad();

            while (!gameShouldClose)
            {
                if (deltaTime > milisecondsForNextFrame)
                {

                    debugLines[1] = $"DeltaTime:  {deltaTime}";
                    debugLines[2] = $"FPS: {1000 / deltaTime}";

                    lastFrame = DateTime.Now;

                    // Updating
                    EngineUpdate();
                    Update();

                    // Rendering
                    Renderer.Render();
                }

                deltaTime = (int)(DateTime.Now - lastFrame).TotalMilliseconds;
            }
        }

        // Functions required to run each frame by the engine
        private void EngineUpdate()
        {
            // Registers imput
            ImputManager.UpdateImput(this);

            // Updates all GameObject
            foreach(var chunk in chunks)
            {
                if (IsChunkLoaded(chunk))
                {
                    foreach (var gameObject in chunk.gameObjects)
                    {
                        gameObject.Update();
                    } 
                }
            }

            // Lazy removing game objects. 
            foreach (var chunk in chunks)
            {
                if (IsChunkLoaded(chunk))
                {
                    foreach (var gameObject in chunk.gameObjectsToRemove)
                    {
                        chunk.gameObjects.Remove(gameObject);
                    }
                    chunk.gameObjectsToRemove.Clear(); 
                }
            }

            // Lazy adding GameObjects
            foreach (var chunk in chunks)
            {
                if (IsChunkLoaded(chunk))
                {
                    foreach (var gameObject in chunk.gameObjectsToAdd)
                    {
                        chunk.gameObjects.Add(gameObject);
                    }
                    chunk.gameObjectsToAdd.Clear(); 
                }
            }

            // Lazy unloading Chunks
            foreach (var chunk in chunksToBeUnloaded)
            {
                UnloadChunk(chunk);
            }
            chunksToBeUnloaded.Clear();
        }


        private void OnEngineLoad()
        {
            // Console settings
            Console.CursorVisible = false;
            Console.Title = title;
            Console.OutputEncoding = Encoding.Unicode;

            // Set up the Renderer, the Camera, the ImputManager and initialize the static Resourcemanager
            serializer = new Serializer();
            Camera = new Camera(new Vec2i(0, 0), new Vec2i(189, 99));
            Renderer = new Renderer(this);
            ImputManager = new ImputManager();
            ResourceManager.Init();

            // Initialise the file system

            //var dt = DateTime.Now; 
            //AddNewSaveData($"{dt.Day}-{dt.Month}-{dt.Year}_{dt.Hour}-{dt.Minute}-{dt.Second}");

            LoadSaveData("debug");

            // Initialise transition lists
            for (var x = 0; x < chunks.GetLength(0); x++)
            {
                for (var y = 0; y < chunks.GetLength(1); y++)
                {
                    unloadedChunkTransitionGameObjects[x, y] = new List<GameObject>();
                }
            }

            // Debug
            debugLines = new string[debugLinesCount];
            for (var i = 0; i < debugLines.Length; i++)
            {
                debugLines[i] = "";
            }

            // Set up frame timers
            lastFrame = DateTime.Now;
            //deltaTime = TimeSpan.Zero;
        }


        // Adds and deletes gameobjects
        public void AddGameObject(GameObject gameObject)
        {
            var chunkX = gameObject.Position.X / chunkSize;
            var chunkY = gameObject.Position.Y / chunkSize;

            gameObject.Chunk = chunks[chunkX, chunkY];
            if (IsChunkLoaded(new Vec2i(chunkX, chunkY))) chunks[chunkX, chunkY].InsertGameObject(gameObject);
            else unloadedChunkTransitionGameObjects[chunkX, chunkY].Add(gameObject);
        }
        public void RemoveGameObject(GameObject gameObject)
        {
            gameObject.Chunk.UnInsertGameObject(gameObject);
        }
        public virtual void LoadChunk(Vec2i index)
        {
            chunks[index.X, index.Y] = serializer.FromFile<Chunk>($"{pathCurrentLoadedSave}\\Chunks\\chunk{index.X}_{index.Y}");

            // Fill misssing data
            chunks[index.X, index.Y].CompleteDataAfterSerialization(this, index);

            // Integrate traverse GameObjects into the chunk and inform them of the newly loaded state
            foreach (var gameObject in unloadedChunkTransitionGameObjects[index.X, index.Y])
            {
                chunks[index.X, index.Y].InsertGameObject(gameObject);
            }
            foreach (var gameObject in unloadedChunkTransitionGameObjects[index.X, index.Y])
            {
                gameObject.OnUnloadedChunkAwake(index.X, index.Y);
            }
            unloadedChunkTransitionGameObjects[index.X, index.Y].Clear();
        }
        private void UnloadChunk(Chunk chunk)
        {
            foreach (var gameObject in chunk.gameObjects)
            {
                gameObject.PrepareForDeserialization();
            }

            serializer.ToFile(chunk, $"{pathCurrentLoadedSave}\\Chunks\\chunk{chunk.Index.X}_{chunk.Index.Y}");
            chunks[chunk.Index.X, chunk.Index.Y] = null;
        }
        public void ScheduleUnloadChunk(Vec2i index)
        {
            chunksToBeUnloaded.Add(chunks[index.X, index.Y]);
        }
        public bool IsChunkLoaded(Vec2i index)
        {
            if (chunks[index.X, index.Y] != null) return true;
            else return false;
        }
        public bool IsChunkLoaded(Chunk chunk)
        {
            if (chunk != null) return true;
            else return false;
        }

        protected void LoadSaveData(string name)
        {
            pathCurrentLoadedSave = $"{pathSavesFolder}\\{name}";
        }
        protected void AddNewSaveData(string name)
        {
            var dir = $"{pathSavesFolder}\\{name}";
            var existingCopies = 1;
            while (Directory.Exists(dir))
            {
                dir = $"{pathSavesFolder}\\{name}-copy({existingCopies})";
                existingCopies++;
            }
            Directory.CreateDirectory(dir);

            pathCurrentLoadedSave = dir;

            Directory.CreateDirectory($"{dir}\\Chunks");
            for (var x = 0; x < chunkCountX; x++)
            {
                for (var y = 0; y < chunkCountY; y++)
                {
                    using (var fs = File.Create($"{pathCurrentLoadedSave}\\Chunks\\chunk{x}_{y}")) { }
                }
            }

            CreateChunks();
        }
        protected virtual void CreateChunks()
        {
            for (var x = 0; x < chunkCountX; x++)
            {
                for (var y = 0; y < chunkCountY; y++)
                {
                    chunks[x, y] = new Chunk(new Vec2i(x, y), this);
                    serializer.ToFile(chunks[x, y], $"{pathCurrentLoadedSave}\\Chunks\\chunk{x}_{y}");
                    chunks[x, y] = null;
                }
            }
        }
        protected abstract void OnLoad();
        protected abstract void Update();
    }
}
