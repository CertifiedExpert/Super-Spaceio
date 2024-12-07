using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace Spaceio.Engine
{
    [DataContract(IsReference = true)]
    abstract class Engine
    {
        // Systems. TODO: change ObservableCollection to ReadOnlyCollection
        [DataMember] public Settings Settings { get; private set; } // The settings of the engine.
        [DataMember] public Renderer Renderer { get; set; } // The renderer of the game.
        [DataMember] public InputManager InputManager { get; private set; } // The input renderer of the game.
        [DataMember] public Serializer Serializer { get; private set; } // The serializer of the game.
        [DataMember] public ChunkManager ChunkManager { get; private set; } // The chunk manager of the game.
        [DataMember] public GameObjectManager GameObjectManager { get; private set; } // The game object manager of the game.
        [DataMember] public Camera Camera { get; private set; } // The camera used in the engine. 
        [DataMember] public UIManager UIManager { get; private set; } // The ui manager used in the engine.


        [DataMember] public bool[][] wasChunkLoadedMap_serialize { get; private set; } // A temporary variable used to save a map of chunks which were loaded during the saving of the game.
        [DataMember] public List<GameObject>[][] unloadedChunkTransitionAddGameObjects_serialize { get; private set; } // A temporary variable used to serialize unloadedChunkTransitionAddGameObjects.
        [DataMember] public List<GameObject>[][] unloadedChunkTransitionRemoveGameObject_serialize { get; private set; } // A temporary variable used to serialize unloadedChunkTransitionRemoveGameObjects.

        public bool gameShouldClose = false; // Flag whether the game is set to close.
        public int deltaTime = 0; // Milliseconds which passed since last frame.
        private Vec2i _worldSize;
        public ReadOnlyVec2i worldSize { get; private set; } // The total size of the world. (Number of chunks * chunk size)
        public Chunk[,] chunks { get; private set; } // A 2d-array of chunks in the engine. If the chunk is null then it is unloaded, otherwise is loaded.
        public List<GameObject>[,] unloadedChunkTransitionAddGameObjects { get; private set; }  // A 2d-array of lists of GameObject which need to be added to the chunk with index corresponding to the position in the array after the chunk gets loaded. (It may be added to this when a GameObject is added to an unloaded chunk.)
        public List<GameObject>[,] unloadedChunkTransitionRemoveGameObjects { get; private set; } // A 2d-array of lists of GameObject which need to be removed from the chunk with index corresponding to the position in the array after the chunk gets loaded. (It may be added to this when a GameObject is removed from an unload chunk.)

        private DateTime lastFrame; // The time of last frame.

        // Debug.
        public readonly int debugLinesCount = 10;
        public readonly int debugLinesLength = 40;
        public string[] debugLines = new string[10];

        // Paths.
        public string pathRootFolder; // The root folder of the executable.
        public string pathSavesFolder;
        public string pathCurrentLoadedSave;

        // Possible configurations.
        // "  " <and> font 20 | width 70 | height 48 <or> font 10 | width 126 | height 90 <or> font 5 | width 316 | height 203
        // " " <and> font 10 | width 189 | height 99
        
        // Application loop.
        protected Engine()
        {
            // Loading. 
            OnEngineLoad();
            OnLoad();

            while (!gameShouldClose)
            {
                if (deltaTime > Settings.milisecondsForNextFrame)
                {

                    debugLines[1] = $"DeltaTime:  {deltaTime}";
                    debugLines[2] = $"FPS: {1000 / deltaTime}";

                    lastFrame = DateTime.Now;

                    // Updating.
                    EngineUpdate();
                    Update();

                    // Rendering.
                    Renderer.Render();
                }

                deltaTime = (int)(DateTime.Now - lastFrame).TotalMilliseconds;
            }

            SaveGame();
        }

        // Called once on engine load. Initializes the engine.
        private void OnEngineLoad()
        {
            pathRootFolder = Util.GetAssemblyDirectory();
            pathSavesFolder = $"{pathRootFolder}\\Saves";

            SetupEngineWithSettings($"{pathRootFolder}\\SettingsTemplates\\defaultSettings");

            // Console settings
            Console.CursorVisible = false;
            Console.Title = Settings.title;
            Console.OutputEncoding = Encoding.Unicode;

            for (var i = 0; i < debugLines.Length; i++) debugLines[i] = "";
            lastFrame = DateTime.Now;
        }

        // Called once every frame
        private void EngineUpdate()
        {
            // Registers input.
            InputManager.UpdateInput(this);

            UIManager.Update();

            GameObjectManager.Update();

            ChunkManager.Update();
        }

        // Adds GameObject to the engine.
        public void AddGameObject(GameObject gameObject)
        {
            GameObjectManager.AddGameObject(gameObject);
        }
        // Removes GameObject from the engine.
        public void RemoveGameObject(GameObject gameObject)
        {
            GameObjectManager.RemoveGameObject(gameObject);
        }
                

        #region FILES/LOADING/UNLOADING
        // Saves the current state of the game including settings and chunks in their own folder.
        protected virtual void SaveGame()
        {
            Serializer.SaveKnownTypes($"{pathCurrentLoadedSave}\\knownTypes");

            var wasChunkLoadedMap2d = new bool[Settings.chunkCountX, Settings.chunkCountY];
            for (var x = 0; x < Settings.chunkCountX; x++)
            {
                for (var y = 0; y < Settings.chunkCountY; y++)
                {
                    if (ChunkManager.IsChunkLoaded(chunks[x, y]))
                    {
                        ChunkManager.ForceUnloadChunk(x, y);
                        wasChunkLoadedMap2d[x, y] = true;
                    }
                    else
                    {
                        wasChunkLoadedMap2d[x, y] = false;
                    }
                }
            }

            wasChunkLoadedMap_serialize = Util.Jaggedize2dArray(wasChunkLoadedMap2d);

            for (var x = 0; x < Settings.chunkCountX; x++)
            {
                for (var y = 0; y < Settings.chunkCountY; y++)
                {
                    foreach (var gameObject in unloadedChunkTransitionAddGameObjects[x, y]) gameObject.PrepareForSerialization();
                }
            }
            unloadedChunkTransitionAddGameObjects_serialize = Util.Jaggedize2dArray(unloadedChunkTransitionAddGameObjects);
            unloadedChunkTransitionRemoveGameObject_serialize = Util.Jaggedize2dArray(unloadedChunkTransitionRemoveGameObjects);
            Serializer.ToFile(this, $"{pathCurrentLoadedSave}\\gameState");
        }
        // Loads save data from a gameState file. Sets all necessary engine variables based on the file contents.
        protected void LoadSavedData(string saveName)
        {
            pathCurrentLoadedSave = $"{pathSavesFolder}\\{saveName}";

            Serializer.ReadKnownTypes($"{pathCurrentLoadedSave}\\knownTypes");
            var data = Serializer.FromFile<Engine>($"{pathCurrentLoadedSave}\\gameState");
            
            // Systems.
            Settings = data.Settings;
            Renderer = data.Renderer;
            Renderer.CompleteDataAfterDeserialization(this);
            InputManager = data.InputManager;
            data.Serializer.knownTypes = Serializer.knownTypes;
            Serializer = data.Serializer;
            Camera = data.Camera;
            ResourceManager.Init();
            ChunkManager = data.ChunkManager;
            ChunkManager.CompleteDataAfterDeserialization(this);
            GameObjectManager = data.GameObjectManager;
            GameObjectManager.CompleteDataAfterDeserialization(this);
            UIManager = data.UIManager;
            UIManager.CompleteDataAfterDeserialization(this);

            // Initiate all necessary variables other than those depending on deserialization.
            SetupAllVariablesNotRelyingOnSerialization();

            // Initiate variables depending on deserialization.
            unloadedChunkTransitionAddGameObjects = Util.UnJaggedize2dArray(data.unloadedChunkTransitionAddGameObjects_serialize);
            unloadedChunkTransitionRemoveGameObjects = Util.UnJaggedize2dArray(data.unloadedChunkTransitionRemoveGameObject_serialize);
            var wasChunkLoadedMap2d = Util.UnJaggedize2dArray(data.wasChunkLoadedMap_serialize);
            for (var x = 0; x < Settings.chunkCountX; x++)
            {
                for (var y = 0; y < Settings.chunkCountY; y++)
                {
                    foreach (var gameObject in unloadedChunkTransitionAddGameObjects[x, y]) gameObject.CompleteDataAfterDeserialization(this, new Vec2i(x, y)); 
                    foreach (var gameObject in unloadedChunkTransitionRemoveGameObjects[x, y]) gameObject.CompleteDataAfterDeserialization(this, new Vec2i(x, y));
                    if (wasChunkLoadedMap2d[x, y]) ChunkManager.LoadChunk(x, y);
                }
            }

            OnSaveDataLoad(data);
        }
        // Called after the save data has been loaded. Has the contents of the gameState file passed to it as an Engine.
        protected virtual void OnSaveDataLoad(Engine data) { }
        // Sets the engine settings and updates the variables to accomodate those settings. Takes in a path to the settings file or the settings data.
        protected void SetupEngineWithSettings(string settingsFilePath)
        {
            Serializer = new Serializer();
            Settings = Serializer.FromFile<Settings>(settingsFilePath);
            Camera = new Camera(Settings.CameraStartPosition, new Vec2i(Settings.CameraSizeX, Settings.CameraSizeY));
            Renderer = new Renderer(this);
            InputManager = new InputManager();
            ResourceManager.Init();
            ChunkManager = new ChunkManager(this);
            GameObjectManager = new GameObjectManager(this);
            UIManager = new UIManager(this);

            // Variables which have nothing to do with serialization.
            SetupAllVariablesNotRelyingOnSerialization();

            // Variables which can be dependent on serialization.
            unloadedChunkTransitionAddGameObjects = new List<GameObject>[Settings.chunkCountX, Settings.chunkCountX];
            unloadedChunkTransitionRemoveGameObjects = new List<GameObject>[Settings.chunkCountX, Settings.chunkCountX];
            for (var x = 0; x < Settings.chunkCountX; x++)
            {
                for (var y = 0; y < Settings.chunkCountY; y++)
                {
                    unloadedChunkTransitionAddGameObjects[x, y] = new List<GameObject>();
                    unloadedChunkTransitionRemoveGameObjects[x, y] = new List<GameObject>();
                }
            }
        }
        // Sets up all the variables which are not dependent on serialization.
        private void SetupAllVariablesNotRelyingOnSerialization()
        {
            _worldSize = new Vec2i(Settings.chunkCountX * Settings.chunkSize, Settings.chunkCountY * Settings.chunkSize);
            worldSize = new ReadOnlyVec2i(_worldSize);

            chunks = new Chunk[Settings.chunkCountX, Settings.chunkCountY];

            unloadedChunkTransitionAddGameObjects = new List<GameObject>[Settings.chunkCountX, Settings.chunkCountY];
            unloadedChunkTransitionRemoveGameObjects = new List<GameObject>[Settings.chunkCountX, Settings.chunkCountY];
            for (var x = 0; x < chunks.GetLength(0); x++)
            {
                for (var y = 0; y < chunks.GetLength(1); y++)
                {
                    unloadedChunkTransitionAddGameObjects[x, y] = new List<GameObject>();
                    unloadedChunkTransitionRemoveGameObjects[x, y] = new List<GameObject>();
                }
            }
        }
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
            for (var x = 0; x < Settings.chunkCountX; x++)
            {
                for (var y = 0; y < Settings.chunkCountY; y++)
                {
                    using (var fs = File.Create($"{dir}\\Chunks\\chunk{x}_{y}")) { }
                }
            }

            using (var fs = File.Create($"{dir}\\gameState")) { }
            using (var fs = File.Create($"{dir}\\knownTypes")) { }

            pathCurrentLoadedSave = dir;
        }

        // Fills the currently loaded save file with empty chunks.
        protected virtual void FillCurrentSaveWithEmptyChunks()
        {
            for (var x = 0; x < Settings.chunkCountX; x++)
            {
                for (var y = 0; y < Settings.chunkCountY; y++)
                {
                    chunks[x, y] = new Chunk(new Vec2i(x, y), this);
                    ChunkManager.ForceUnloadChunk(x, y);
                    chunks[x, y] = null;
                }
            }
        }
        #endregion


        // Called once after engine load. Initializes the derived engine.
        protected abstract void OnLoad();
        // Called once every frame after the engine has updated.
        protected abstract void Update();
    }
}
