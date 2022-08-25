using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using SpaceGame;

namespace Console_Platformer.Engine
{
    [DataContract(IsReference = true)]
    abstract class Engine
    {//TODO: figure out in which functions the Vec2i parameter is acutally helpful as opposed to passing just 2 int
        //TODO: there are some nested for loops over chunks whereas a foreach loop would possibly work
        //TODO: many times the Engine is called from SpaceGame. Game shoud be called instead
        //TODO: file system. Everything is hardcoded for now.
        public bool gameShouldClose = false;
        public int deltaTime = 0; // Miliseconds since last frame

        #region SERIALIZABLE_VARIABLES
        [DataMember] public int spriteMaxCount { get; private set; } //10 
        [DataMember] public int spriteLevelCount { get; private set; }//10;
        [DataMember] public char backgroudPixel { get; private set; }//' ';
        [DataMember] public string pixelSpacingCharacters { get; private set; }//" ";
        [DataMember] public string title { get; private set; }//"default title";
        [DataMember] public int chunkCountX { get; private set; }//100;              
        [DataMember] public int chunkCountY { get; private set; }//100;
        [DataMember] public int chunkSize { get; private set; }//100;
        [DataMember] public int milisecondsForNextFrame; //= 20;
        private Camera _camera;

        [DataMember] public Camera Camera
        {
            get { return _camera; }
            private set 
            {
                if (_camera != null) Renderer = new Renderer(this); //TODO: this might be a bug inducing code!! Add a concrete check if the camera is first time initialized instead of this (somebody can set camera to null and this will not reset the renderer.)
                _camera = value;
            }
        }
        #endregion

        public Vec2i worldSize { get; private set; }
        public Chunk[,] chunks { get; private set; } 
        public List<GameObject>[,] unloadedChunkTransitionGameObjects { get; private set; } //= new List<GameObject>[chunkCountX, chunkCountY];
        private List<Chunk> chunksToBeUnloaded = new List<Chunk>();

        //"  " <and> font 20 | width 70 | height 48 <or> font 10 | width 126 | height 90 <or> font 5 | width 316 | height 203
        //" " <and> font 10 | width 189 | height 99
        public Renderer Renderer { get; set; }
        public ImputManager ImputManager { get; private set; }
        public Serializer Serializer { get; private set; }
        public Random Random { get; private set; } 

        private DateTime lastFrame;

        //debug 
        public readonly bool allowDebug = true;
        public readonly int debugLinesCount = 10;
        public readonly int debugLinesLength = 40;
        public string[] debugLines;

        protected string pathRootFolder = @"C:\Users\Admin\source\repos\Console_Platformer\Console_Platformer\bin\Debug";
        protected string pathSavesFolder;
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
        private void OnEngineLoad()
        {
            // Set up the Renderer, the Camera, the ImputManager and initialize the static Resourcemanager
            Serializer = new Serializer();
            Serializer.knownTypes.Add(GetType());
            ImputManager = new ImputManager();
            ResourceManager.Init();

            var defaultTemplateData = Serializer.FromFile<Engine>($"{pathRootFolder}\\Templates\\defaultTemplate"); 
            LoadTemplate(defaultTemplateData);
            FinaliseVariableInit();

            pathSavesFolder = $"{pathRootFolder}\\Saves";

            // Console settings
            Console.CursorVisible = false;
            Console.Title = title;
            Console.OutputEncoding = Encoding.Unicode;
        }
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

        #region UTIL_FUNCTIONS
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
            chunks[index.X, index.Y] = Serializer.FromFile<Chunk>($"{pathCurrentLoadedSave}\\Chunks\\chunk{index.X}_{index.Y}");

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

            Serializer.ToFile(chunk, $"{pathCurrentLoadedSave}\\Chunks\\chunk{chunk.Index.X}_{chunk.Index.Y}");
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
        #endregion

        #region FILES/LOADING/UNLOADING
        protected virtual void LoadSavedData(string saveName)
        {
            pathCurrentLoadedSave = $"{pathSavesFolder}\\{saveName}";

            var data = Serializer.FromFile<Engine>($"{pathCurrentLoadedSave}\\gameState");
            LoadTemplate(data);
            Camera = data.Camera;
        }
        protected virtual void AddNewSavedData(string name)
        {
            var dir = $"{pathSavesFolder}\\{name}";
            var existingCopies = 1;
            while (Directory.Exists(dir))
            {
                dir = $"{pathSavesFolder}\\{name}-copy({existingCopies})";
                existingCopies++;
            }
            Directory.CreateDirectory(dir);

            Directory.CreateDirectory($"{dir}\\Chunks");
            for (var x = 0; x < chunkCountX; x++)
            {
                for (var y = 0; y < chunkCountY; y++)
                {
                    using (var fs = File.Create($"{dir}\\Chunks\\chunk{x}_{y}")) { }
                }
            }

            using (var fs = File.Create($"{dir}\\gameState")) { }
        }
        protected virtual void CreateChunks()
        {
            for (var x = 0; x < chunkCountX; x++)
            {
                for (var y = 0; y < chunkCountY; y++)
                {
                    chunks[x, y] = new Chunk(new Vec2i(x, y), this);
                    Serializer.ToFile(chunks[x, y], $"{pathCurrentLoadedSave}\\Chunks\\chunk{x}_{y}");
                    chunks[x, y] = null;
                }
            }
        }
        private void LoadTemplate(Engine data)
        {
            spriteMaxCount = data.spriteMaxCount;
            spriteLevelCount = data.spriteLevelCount;
            backgroudPixel = data.backgroudPixel;
            pixelSpacingCharacters = data.pixelSpacingCharacters;
            title = data.title;
            chunkCountX = data.chunkCountX;
            chunkCountY = data.chunkCountY;
            chunkSize = data.chunkSize;
            milisecondsForNextFrame = data.milisecondsForNextFrame;
            Camera = data.Camera;
        }

        private void FinaliseVariableInit()
        {
            chunks = new Chunk[chunkCountX, chunkCountY];
            worldSize = new Vec2i(chunkCountX * chunkSize, chunkCountY * chunkSize);
            chunksToBeUnloaded = new List<Chunk>();
            Random = new Random();

            // Initialise transition lists
            unloadedChunkTransitionGameObjects = new List<GameObject>[chunkCountX, chunkCountY];
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
        }
        #endregion

        protected abstract void OnLoad();
        protected abstract void Update();
    }
}
