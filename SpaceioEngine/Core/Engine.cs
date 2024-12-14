using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.IO;

namespace ConsoleEngine
{
    public abstract class Engine
    {
        // Systems. TODO: change ObservableCollection to ReadOnlyCollection
        [DataMember] public Settings Settings { get; private set; } // The settings of the engine.
        [DataMember] public Renderer Renderer { get; private set; } // The renderer of the game.
        [DataMember] public InputManager InputManager { get; private set; } // The input renderer of the game.
        [DataMember] public ChunkManager ChunkManager { get; private set; } // The chunk manager of the game.
        [DataMember] public GameObjectManager GameObjectManager { get; private set; } // The game object manager of the game.
        [DataMember] public Camera Camera { get; private set; } // The camera used in the engine. 
        [DataMember] public UIManager UIManager { get; private set; } // TODO: in the next branch implement this
        public ResourceManager ResourceManager { get; private set; }
        public SaveFileManager SaveFileManager { get; private set; }


        private bool gameShouldClose = false; // Flag whether the game is set to close.
        public int deltaTime { get; private set; } // Milliseconds which passed since last frame.
        private DateTime lastFrame; // The time of last frame.

        // Debug.
        public readonly int debugLinesCount = 10;
        public readonly int debugLinesLength = 40;
        public string[] debugLines = new string[10];

        // Paths.
        public string pathRootFolder = Util.GetAssemblyDirectory();
        public string pathSaveFolder { get; private set; }
        public string pathWorldFolder { get; private set; }
        public string pathGameState { get; private set; }

        // Possible configurations.
        // "  " <and> font 20 | width 70 | height 48 <or> font 10 | width 126 | height 90 <or> font 5 | width 316 | height 203
        // " " <and> font 10 | width 189 | height 99

        // Application loop.
        protected Engine() { }
        public void Run()
        {
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

            OnSave();
            Save();
        }
        public void LoadFromSave(string name)
        {
            pathSaveFolder = $"{pathRootFolder}\\Saves\\{name}";
            pathWorldFolder = $"{pathSaveFolder}\\World";
            pathGameState = $"{pathSaveFolder}\\GameState";

            Settings = Serializer.FromFile<Settings>($"{pathGameState}\\Settings.txt");
            ChunkManager = new ChunkManager(this, Serializer.FromFile<ChunkManagerSaveData>($"{pathGameState}\\ChunkManager.txt"));
            GameObjectManager = new GameObjectManager(this, Serializer.FromFile<GameObjectManagerSaveData>($"{pathGameState}\\GameObjectManager.txt"));
            Camera = new Camera(Serializer.FromFile<CameraSaveData>("${pathGameState}\\Camera.txt"));
            ResourceManager = new ResourceManager(this, Serializer.FromFile<ResourceManagerSaveData>($"{pathGameState}\\ResourceManager.txt"));
            SaveFileManager = new SaveFileManager(this);

            SetUpEngine();

            SaveFileManager.LoadChunkHeader($"{pathWorldFolder}\\ChunkHeader.txt");
        }

        // Sequence of creating new subsystems is vital!
        public void NewSave(string name, Settings settings, ChunkManager chunkManager, GameObjectManager gameObjectManager, Camera camera,
            ResourceManager resourceManager, SaveFileManager saveFileManager, Renderer renderer, InputManager inputManager)
        {
            var saveName = name;
            while (Directory.Exists($"{pathRootFolder}\\Saves\\{saveName}")) saveName += "(1)";

            // folder Saves\\{saveName}:
            // -World
            //      -ChunkHeader
            //      -ChunkData
            // -GameState
            //      - files for all subsystems

            pathSaveFolder = $"{pathRootFolder}\\Saves\\{saveName}";
            Directory.CreateDirectory($"{pathSaveFolder}");
            pathWorldFolder = $"{pathSaveFolder}\\World";
            pathGameState = $"{pathSaveFolder}\\GameState";
            Directory.CreateDirectory($"{pathWorldFolder}");
            Directory.CreateDirectory($"{pathGameState}");            

            Settings = settings;
            Renderer = renderer;
            InputManager = inputManager;
            ChunkManager = chunkManager;
            GameObjectManager = gameObjectManager;
            Camera = camera;
            ResourceManager = resourceManager;
            SaveFileManager = saveFileManager;

            SetUpEngine();
        }

        public void Save()
        {
            // Unload all chunks
            foreach (var chunk in ChunkManager.loadedChunks)
            {
                ChunkManager.ForceUnloadChunk(chunk.Index.X, chunk.Index.Y);
            }
            SaveFileManager.WriteHeaderToFile($"{pathWorldFolder}\\ChunkHeader.txt");

            // Save engine state (if such even exists)

            // Save managers
            Serializer.ToFile(Settings, $"{pathGameState}\\Settings.txt");
            Serializer.ToFile(ChunkManager.GetSaveData(), $"{pathGameState}\\ChunkManager.txt");
            Serializer.ToFile(GameObjectManager.GetSaveData(), $"{pathGameState}\\GameObjectManager.txt");
            Serializer.ToFile(Camera.GetSaveData(), $"{pathGameState}\\Camera.txt");
            Serializer.ToFile(ResourceManager.GetSaveData(), $"{pathGameState}\\ResourceManager.txt");   
        }

        // Called once on engine load. Initializes the engine.
        private void SetUpEngine()
        {
            deltaTime = 0;

            // Console settings
            Console.CursorVisible = false;
            Console.Title = "Test";
            Console.OutputEncoding = Encoding.Unicode;

            for (var i = 0; i < debugLines.Length; i++) debugLines[i] = "";
            lastFrame = DateTime.Now;

            GameObjectManager.Init();
            ResourceManager.Init();
            SaveFileManager.Init();
            Renderer.Init();
        }

        // Called once every frame
        private void EngineUpdate()
        {
            // Registers input.
            InputManager.UpdateInput();

            UIManager.Update();

            GameObjectManager.Update();

            ChunkManager.Update();
        }

        // Adds GameObject to the engine.
        public UID AddGameObject(GameObject gameObject)
        {
            return GameObjectManager.AddGameObject(gameObject);
        }
        // Removes GameObject from the engine.
        public void RemoveGameObject(UID uID)
        {
            GameObjectManager.RemoveGameObject(uID);
        }
        
        // Returns null if GO cannot be found or is unloaded
        public GameObject Find(UID uID)
        {
            foreach (var c in ChunkManager.loadedChunks)
            {
                if (c.gameObjects.ContainsKey(uID)) return c.gameObjects[uID];
            }

            return null;
        }

        public void CloseEngine() => gameShouldClose = true;

        // Called once after engine load. Initializes the derived engine.
        protected abstract void OnLoad();
        // Called once every frame after the engine has updated.
        protected abstract void Update();
        protected abstract void OnSave();
    }
}
