using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Engine
{
    abstract class Engine
    {
        // Public variables
        public bool gameShouldClose = false;
        public int deltaTime = 0; // Miliseconds since last frame
        public string title = "default title";
        public readonly Vec2i worldSize = new Vec2i(500, 500);
        public readonly string pixelSpacingCharacters = " ";
        public readonly char backgroudPixel = ' ';
        public readonly int spriteLevelCount = 10;
        public readonly int spriteMaxCount = 10;

        public readonly int chunkSize = 100;
        public readonly int neighbouringChunks = 2;
        public List<GameObject> gameObjects = new List<GameObject>();
        public static Random gRandom = new Random();

        //"  " <and> font 20 | width 70 | height 48 <or> font 10 | width 126 | height 90 <or> font 5 | width 316 | height 203
        //" " <and> font 10 | width 189 | height 99
        public Camera Camera { get; private set; } 
        protected Renderer Renderer { get; private set; }
        public ImputManager ImputManager { get; private set; }

        private List<GameObject>[] gameObjectRenderLists;
        private List<GameObject> gameObjectsToRemove = new List<GameObject>();
        private DateTime lastFrame;
        private readonly int milisecondsForNextFrame = 40;

        //debug 
        public readonly bool allowDebug = true;
        public readonly int debugLinesCount = 10;
        public readonly int debugLinesLength = 40;
        public string[] debugLines;


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

                    debugLines[5] = deltaTime.ToString();

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
            foreach(var gameObject in gameObjects)
            {
                gameObject.Update();
            }

            // Lazy removing game objects. 
            foreach (var gameObject in gameObjectsToRemove)
            {
                gameObjects.Remove(gameObject);
            }
            gameObjectsToRemove.Clear();
        }


        private void OnEngineLoad()
        {
            // Console settings
            Console.CursorVisible = false;
            Console.Title = title;
            Console.OutputEncoding = Encoding.Unicode;

            // Debug
            debugLines = new string[debugLinesCount];
            for (var i = 0; i < debugLines.Length; i++)
            {
                debugLines[i] = "";
            }

            // Set up render lists
            gameObjectRenderLists = new List<GameObject>[spriteLevelCount];
            for (var i = 0; i < gameObjectRenderLists.Length; i++)
            {
                gameObjectRenderLists[i] = new List<GameObject>();
            }

            // Set up frame timers
            lastFrame = DateTime.Now;
            //deltaTime = TimeSpan.Zero;

            // Set up the Renderer, the Camera, the ImputManager and initialize the static Resourcemanager
            Camera = new Camera(new Vec2i(0, 0), new Vec2i(189, 99));
            Renderer = new Renderer(this, gameObjectRenderLists);
            ImputManager = new ImputManager();
            ResourceManager.Init();
        }


        // Adds and deletes gameobjects
        public void AddGameObject(GameObject gameObject)
        {
            gameObjects.Add(gameObject);
            gameObjectRenderLists[gameObject.SpriteLevel].Add(gameObject);
        }
        public void RemoveGameObject(GameObject gameObject)
        {
            gameObjectsToRemove.Add(gameObject);
            gameObjectRenderLists[gameObject.SpriteLevel].Remove(gameObject);
        }

        protected abstract void OnLoad();
        protected abstract void Update();
    }
}
