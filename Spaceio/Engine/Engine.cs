using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.IO;
using SpaceGame;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Spaceio.Engine
{
    [DataContract(IsReference = true)]
    abstract class Engine
    {
        public bool gameShouldClose = false; // Flag whether the game is set to close.
        public int deltaTime = 0; // Miliseconds which passed since last frame.

        #region SERIALIZABLE_VARIABLES
        [DataMember] public int spriteMaxCount { get; private set; } // The maximum number of sprites a GameObject may have.
        [DataMember] public int spriteLevelCount { get; private set; } // The maximum number of renderer levels a Sprite can have.
        [DataMember] public char backgroudPixel { get; private set; } // The pixel which is used as the backgroud.
        [DataMember] public string pixelSpacingCharacters { get; private set; } // Unchanbable pixels put inbetween two changable pixels to account for difference in width and height of unicode characters.
        [DataMember] public string title { get; private set; } // The title of the console window.
        [DataMember] public int chunkCountX { get; private set; } // The number of chunks in the X-axis.
        [DataMember] public int chunkCountY { get; private set; } // The number of chunks in the Y-axis.
        [DataMember] public int chunkSize { get; private set; } // The size of each chunk (both in X- and Y- axis as the chunk is a square.
        [DataMember] public bool[][] wasChunkLoadedMap_serialize { get; private set; } // A temporary variable used to save a map of chunks which were loaded during the saving of the game.
        [DataMember] public List<GameObject>[][] unloadedChunkTransitionAddGameObjects_serialize { get; private set; } // TODO: make private and deserialize through reflection. A temporary variable used to save a 2d array of GameObject which needs to be added to a chunk after it gets loaded. (The position in the array corresponds to the chunk)
        [DataMember] public List<GameObject>[][] unloadedChunkTransitionRemoveGameObject_serialize { get; private set; } // TODO: same here .A temporary variable used to save a 2d array of GameObject which needs to be removed from a chunk after it gets loaded. (The position in the array corresponds to the chunk)
        [DataMember] public int milisecondsForNextFrame; // Minimum number of miliseconds which needs to pass for the next frame to 
        private Camera _camera;

        [DataMember]
        public Camera Camera // The camera used in the engine. Each time the camera is changed and its size is different from the current camera create a new renderer. (Renderer is critically dependent on the cammera size because it uses it as a viewport.)
        {
            get { return _camera; }
            private set
            {
                if (_camera != null && _camera.Size.X == value.Size.X && _camera.Size.Y == value.Size.Y) Renderer = new Renderer(this); // Only changes the Render if _camera is different from null because if it is null it means that the engine was just created and is being initialised, meaning that the Renderer wil be added later.
                _camera = value;
            }
        }
        #endregion

        #region OTHER_VARIABLES
        private Vec2i _worldSize;
        public ReadOnlyVec2i worldSize { get; private set; } // The total size of the world. (Number of chunks * chunk size)
        public Chunk[,] chunks { get; private set; } // A 2d-array of chunks in the engine. If the chunk is null then it is unloaded, otherwise is loaded.
        private List<Chunk> _loadedChunks;
        public ReadOnlyCollection<Chunk> loadedChunks { get; private set; } // A list of all chunks which are loaded. Can be used instead of chunks[,] during interation for the conveniance of not checking if the chunk is loaded.
        public List<GameObject>[,] unloadedChunkTransitionAddGameObjects { get; private set; }  // A 2d-array of lists of GameObject which need to be added to the chunk with index corresponding to the position in the array after the chunk gets loaded. (It may be added to this when a GameObject is added to an unloaded chunk.)
        public List<GameObject>[,] unloadedChunkTransitionRemoveGameObjects { get; private set; } // A 2d-array of lists of GameObject which need to be removed from the chunk with index corresponding to the position in the array after the chunk gets loaded. (It may be added to this when a GameObject is removed from an unload chunk.)
        private List<Chunk> chunksToBeUnloaded = new List<Chunk>(); // List of chunks which have been scheduled to be unloaded.
        private List<Chunk> chunksToBeAddedToLoadedChunks = new List<Chunk>();
        private List<Chunk> chunksToBeRemovedFromLoadedChunks = new List<Chunk>();

        // Possible configurations.
        // "  " <and> font 20 | width 70 | height 48 <or> font 10 | width 126 | height 90 <or> font 5 | width 316 | height 203
        // " " <and> font 10 | width 189 | height 99

        public Renderer Renderer { get; set; } // The current renderer of the game.
        public ImputManager ImputManager { get; private set; } // The current imput renderer of the game.
        public Serializer Serializer { get; private set; } // The current serializer of the game.
        public Random Random { get; private set; } // The current serializer of the game.

        private DateTime lastFrame; // The time of last frame.

        // Debug
        public readonly bool allowDebug = true;
        public readonly int debugLinesCount = 10;
        public readonly int debugLinesLength = 40;
        public string[] debugLines;

        protected string pathRootFolder; // The root folder of the executable.
        protected string pathSavesFolder;
        protected string pathCurrentLoadedSave;
        #endregion

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

            SaveGame();
        }

        // Called once on engine load. Initializes the engine.
        private void OnEngineLoad()
        {
            // Set up the Renderer, the Camera, the ImputManager and initialize the static Resourcemanager
            Serializer = new Serializer();
            Serializer.knownTypes.Add(GetType());
            ImputManager = new ImputManager();
            ResourceManager.Init();

            pathRootFolder = GetAssemblyDirectory();
            pathSavesFolder = $"{pathRootFolder}\\Saves";
            SetupEngineWithSettings($"{pathRootFolder}\\SettingsTemplates\\defaultSettings");

            // Console settings
            Console.CursorVisible = false;
            Console.Title = title;
            Console.OutputEncoding = Encoding.Unicode;
        }

        // Called once every frame
        private void EngineUpdate()
        {
            // Registers imput.
            ImputManager.UpdateImput(this);

            // Lazy removing game objects. 
            foreach (var chunk in loadedChunks)
            {
                foreach (var gameObject in chunk.gameObjectsToRemove)
                {
                    chunk.gameObjects.Remove(gameObject);
                }
                chunk.gameObjectsToRemove.Clear(); 
            }

            // Lazy adding GameObjects.
            foreach (var chunk in loadedChunks)
            {
                foreach (var gameObject in chunk.gameObjectsToAdd)
                {
                    chunk.gameObjects.Add(gameObject);
                }
                chunk.gameObjectsToAdd.Clear(); 
            }

            // Updates all GameObject.
            foreach (var chunk in loadedChunks)
            {
                foreach (var gameObject in chunk.gameObjects)
                {
                    gameObject.Update();
                }
            }

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

        #region UTIL_FUNCTIONS
        // Adds GameObject to the engine.
        public void AddGameObject(GameObject gameObject)
        {
            var chunkX = gameObject.Position.X / chunkSize;
            var chunkY = gameObject.Position.Y / chunkSize;

            gameObject.Chunk = chunks[chunkX, chunkY];
            if (IsChunkLoaded(chunkX, chunkY)) chunks[chunkX, chunkY].InsertGameObject(gameObject);
            else unloadedChunkTransitionAddGameObjects[chunkX, chunkY].Add(gameObject);
        }
        // Removes GameObject from the engine.
        public void RemoveGameObject(GameObject gameObject)
        {
            if (IsChunkLoaded(gameObject.Chunk)) gameObject.Chunk.UnInsertGameObject(gameObject);
            else unloadedChunkTransitionRemoveGameObjects[gameObject.Chunk.Index.X, gameObject.Chunk.Index.Y].Add(gameObject);
        }
        // Loads the Chunk at specified index from file into the engine.
        public virtual void LoadChunk(int x, int y)
        {
            chunks[x, y] = Serializer.FromFile<Chunk>($"{pathCurrentLoadedSave}\\Chunks\\chunk{x}_{y}");

            // Fill misssing data
            chunks[x, y].CompleteDataAfterSerialization(this, new Vec2i(x, y));

            chunksToBeAddedToLoadedChunks.Add(chunks[x, y]);
            chunks[x, y].OnChunkLoaded();

            // Integrate traverse GameObjects into the chunk and inform them of the newly loaded state (or delete them)
            foreach (var gameObject in unloadedChunkTransitionRemoveGameObjects[x, y])
            {
                chunks[x, y].UnInsertGameObject(gameObject);
            }
            foreach (var gameObject in unloadedChunkTransitionAddGameObjects[x, y])
            {
                chunks[x, y].InsertGameObject(gameObject);
            }
            foreach (var gameObject in unloadedChunkTransitionAddGameObjects[x, y])
            {
                gameObject.OnUnloadedChunkAwake(x, y);
            }
            unloadedChunkTransitionRemoveGameObjects[x, y].Clear();
            unloadedChunkTransitionAddGameObjects[x, y].Clear();
        }
        // Scheudles the Chunk to be unloaded at the at the beginning of the next frame.
        public void ScheduleUnloadChunk(int x, int y)
        {
            if (IsChunkLoaded(chunks[x, y]) && !chunksToBeUnloaded.Contains(chunks[x, y])) chunksToBeUnloaded.Add(chunks[x, y]);

            chunksToBeRemovedFromLoadedChunks.Add(chunks[x, y]);
            chunks[x, y].OnChunkUnLoaded();
        }
        // Checks if the chunk is loaded and returns the result as a bool.
        public bool IsChunkLoaded(int x, int y)
        {
            if (chunks[x, y] != null) return true;
            else return false;
        }
        // Checks if the chunk is loaded and returns the result as a bool.
        public bool IsChunkLoaded(Chunk chunk)
        {
            if (chunk != null) return true;
            else return false;
        }
        // Unloads the Chunk at specified index to file and deletes it from the engine.
        private void UnloadChunk(int x, int y)
        {
            foreach (var gameObject in chunks[x, y].gameObjects)
            {
                gameObject.PrepareForDeserialization();
            }

            Serializer.ToFile(chunks[x, y], $"{pathCurrentLoadedSave}\\Chunks\\chunk{chunks[x, y].Index.X}_{chunks[x, y].Index.Y}");
            chunks[chunks[x, y].Index.X, chunks[x, y].Index.Y] = null;
        }
        #endregion

        #region FILES/LOADING/UNLOADING
        // Loads save data from a gameState file. Sets all necessary engine variables based on the file contents.
        protected void LoadSavedData(string saveName)
        {
            pathCurrentLoadedSave = $"{pathSavesFolder}\\{saveName}";

            var data = Serializer.FromFile<Engine>($"{pathCurrentLoadedSave}\\gameState");
            SetupEngineWithSettings(data);

            unloadedChunkTransitionAddGameObjects = Util.UnJaggedize2dArray(data.unloadedChunkTransitionAddGameObjects_serialize);
            unloadedChunkTransitionRemoveGameObjects = Util.UnJaggedize2dArray(data.unloadedChunkTransitionRemoveGameObject_serialize);
            var wasChunkLoadedMap2d = Util.UnJaggedize2dArray(data.wasChunkLoadedMap_serialize);
            for (var x = 0; x < chunkCountX; x++)
            {
                for (var y = 0; y < chunkCountY; y++)
                {
                    foreach (var gameObject in unloadedChunkTransitionAddGameObjects[x, y]) gameObject.CompleteDataAfterSerialization(this, new Vec2i(x, y)); 
                    foreach (var gameObject in unloadedChunkTransitionRemoveGameObjects[x, y]) gameObject.CompleteDataAfterSerialization(this, new Vec2i(x, y));
                    if (wasChunkLoadedMap2d[x, y]) LoadChunk(x, y);
                }
            }

            OnSaveDataLoad(data);
        }
        // Called after the save data has been loaded. Has the contents of the gameState file passed to it as an Engine.
        protected virtual void OnSaveDataLoad(Engine data) { }
        // Add a new save folder with save information in it and sets the pathCurrentLoadedSave to it. (Does not save any actual information, just makes place for it)
        protected virtual void AddNewSaveFiles(string saveName)
        {
            var dir = $"{pathSavesFolder}\\{saveName}";
            var existingCopies = 1;
            while (Directory.Exists(dir))
            {
                dir = $"{pathSavesFolder}\\{saveName}-copy({existingCopies})";
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

            pathCurrentLoadedSave = dir;
        }
        // Fills the save file with empty chunks.
        protected virtual void CreateChunks()
        {
            for (var x = 0; x < chunkCountX; x++)
            {
                for (var y = 0; y < chunkCountY; y++)
                {
                    chunks[x, y] = new Chunk(new Vec2i(x, y), this);
                    UnloadChunk(x, y);
                    chunks[x, y] = null;
                }
            }
        }
        // Saves the currnet state of the game including chunks and settings.
        protected virtual void SaveGame()
        {
            var wasChunkLoadedMap2d = new bool[chunkCountX, chunkCountY];
            for (var x = 0; x < chunkCountX; x++)
            {
                for (var y = 0; y < chunkCountY; y++)
                {
                    if (IsChunkLoaded(chunks[x, y]))
                    {
                        UnloadChunk(x, y);
                        wasChunkLoadedMap2d[x, y] = true;
                    }
                    else
                    {
                        wasChunkLoadedMap2d[x, y] = false;
                    }
                }
            }

            wasChunkLoadedMap_serialize = Util.Jaggedize2dArray(wasChunkLoadedMap2d);

            for (var x = 0; x < chunkCountX; x++)
            {
                for (var y = 0; y < chunkCountY; y++)
                {
                    foreach (var gameObject in unloadedChunkTransitionAddGameObjects[x, y]) gameObject.PrepareForDeserialization();
                }
            }
            unloadedChunkTransitionAddGameObjects_serialize = Util.Jaggedize2dArray(unloadedChunkTransitionAddGameObjects);
            unloadedChunkTransitionRemoveGameObject_serialize = Util.Jaggedize2dArray(unloadedChunkTransitionRemoveGameObjects);
            Serializer.ToFile(this, $"{pathCurrentLoadedSave}\\gameState");
        }


        // Sets the engine settings and updates the variables to accomodate those settings. Takes in a path to the settings file or the settings data.
        protected void SetupEngineWithSettings(string settingsFilePath)
        {
            var settingsData = Serializer.FromFile<Engine>(settingsFilePath);
            SetupEngineWithSettingsInternal(settingsData);
        }
        protected void SetupEngineWithSettings(Engine settingsData)
        {
            SetupEngineWithSettingsInternal(settingsData);
        }
        private void SetupEngineWithSettingsInternal(Engine settingsData)
        {
            LoadSettingsData(settingsData);
            FinaliseVariableInit();
        }

        // Loads setting variables from an instance of Engine.
        private void LoadSettingsData(Engine data)
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
        // Basing on settings variables, initializes the variables of an engine which do not require additional information for initialization.
        private void FinaliseVariableInit()
        {
            chunks = new Chunk[chunkCountX, chunkCountY];
            _worldSize = new Vec2i(chunkCountX * chunkSize, chunkCountY * chunkSize);
            worldSize = new ReadOnlyVec2i(_worldSize);
            chunksToBeUnloaded = new List<Chunk>();
            _loadedChunks = new List<Chunk>();
            loadedChunks = new ReadOnlyCollection<Chunk>(_loadedChunks);
            chunksToBeAddedToLoadedChunks = new List<Chunk>();
            chunksToBeRemovedFromLoadedChunks = new List<Chunk>();

            Random = new Random();

            // Initialise transition lists
            unloadedChunkTransitionAddGameObjects = new List<GameObject>[chunkCountX, chunkCountY];
            unloadedChunkTransitionRemoveGameObjects = new List<GameObject>[chunkCountX, chunkCountY];
            for (var x = 0; x < chunks.GetLength(0); x++)
            {
                for (var y = 0; y < chunks.GetLength(1); y++)
                {
                    unloadedChunkTransitionAddGameObjects[x, y] = new List<GameObject>();
                    unloadedChunkTransitionRemoveGameObjects[x, y] = new List<GameObject>();
                }
            }

            // Debug
            debugLines = new string[debugLinesCount];
            for (var i = 0; i < debugLines.Length; i++)
            {
                debugLines[i] = "";
            }

            Renderer = new Renderer(this);

            // Set up frame timers
            lastFrame = DateTime.Now;
        }
        // Gets the assembly directory of the executable
        public string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
        #endregion

        // Called once after engine load. Initializes the derived engine.
        protected abstract void OnLoad();
        // Called once every frame after the engine has updated.
        protected abstract void Update();
    }
}
