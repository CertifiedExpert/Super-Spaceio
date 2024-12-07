using System.Runtime.Serialization;
using SuperSpaceio.Engine;

namespace SpaceGame
{
    [DataContract(IsReference = true)]
    sealed class Game : Engine
    {
        // Settings
        public int millisecondsPerPlayerMove = 50;
        public const int chunkLoadRadius = 3;

        public GoBind playerShip;
        public GoBind enemy;

        public int millisecondsSinceLastPlayerMove = 0;
        public bool playerMovedInThisFrame = false;
        protected override void OnLoad()
        {
            AddNewSaveFiles("test");
            SetupEngineWithSettings($"{pathRootFolder}\\SettingsTemplates\\defaultSettings");
            FillCurrentSaveWithEmptyChunks();
            
            Serializer.knownTypes.Add(typeof(Game));
            Serializer.knownTypes.Add(typeof(BaseObject));
            Serializer.knownTypes.Add(typeof(Asteroid));
            Serializer.knownTypes.Add(typeof(Ship));
            Serializer.knownTypes.Add(typeof(PlayerShip));
            Serializer.knownTypes.Add(typeof(Enemy));
            
            var enm = new Enemy(new Vec2i(5, 5), 1, this);
            AddBaseObject(enm);
            enemy = new GoBind(enm, "enemy", this);
            
            var p = new PlayerShip(new Vec2i(40, 40), 1, this);
            AddBaseObject(p);
            playerShip = new GoBind(p, "playerShip", this);
            
            LoadLevel();
            
            //LoadSavedData("test");


            

            
            /*
            var parentPanel = new UIPanel(new Vec2i(0, 0), new Vec2i(40, 20), 1, this);
            UIManager.AddParentUIPanel(parentPanel);

            var txtbox = new UITextBox(new Vec2i(0, 0), new Vec2i(5, 5), new Vec2i(0, 0), "abcdefgh", 'L', true);
            parentPanel.AddUIComponent(txtbox);

            var pbar = new UIProgressBar(new Vec2i(0, 6), new Vec2i(20, 5), new Vec2i(1, 1), new Vec2i(18, 3), '#', 'o');
            pbar.Progress = 0.75f;
            parentPanel.AddUIComponent(pbar);

            var imgbox = new UIImageBox(new Vec2i(22, 0), new Vec2i(3, 4));
            imgbox.SetImage(ResourceManager.enemyDefault, new Vec2i(0, 0));
            parentPanel.AddUIComponent(imgbox);
            */
        }

        protected override void Update()
        {
            if (Util.random.Next(0, 30) == 5 && enemy?.IsActive == true) enemy.Val.MoveGameObject(1, 0);
            if (InputManager.Escape.IsPressed) gameShouldClose = true;

            UpdateCamera();

            //UpdateAllBaseObjects();
            var loadedGameObjectCount = 0;
            var lChunks = 0;
            foreach (var chunk in ChunkManager.loadedChunks)
            {
                foreach (var go in chunk.gameObjects)
                {
                    loadedGameObjectCount++;
                }
                lChunks++;
            }
            debugLines[0] = $"Loaded GameObjects: {loadedGameObjectCount}";
            debugLines[3] = $"Camera X: {Camera.Position.X} | Y: {Camera.Position.Y}";
            if (playerShip.IsActive) debugLines[4] = $"Current chunk X: {playerShip.Val.Chunk.Index.X} | Y: {playerShip.Val.Chunk.Index.Y}";
            debugLines[5] = $"Loaded chunks: {lChunks}";
        }

        private void UpdateCamera()
        {
            if (InputManager.ArrowUp.IsPressed) Camera.Position.Y += 3;
            if (InputManager.ArrowDown.IsPressed) Camera.Position.Y -= 3;
            if (InputManager.ArrowLeft.IsPressed) Camera.Position.X -= 3;
            if (InputManager.ArrowRight.IsPressed) Camera.Position.X += 3;
        }
        public void AddBaseObject(BaseObject baseObject)
        {
            AddGameObject(baseObject);
        }
        public void RemoveBaseObject(BaseObject baseObject)
        {
            RemoveGameObject(baseObject);
        }

        //TODO: figure this out
        /* I HAVE NO IDEA IF THIS IS NECESSARY 
        private void UpdateAllBaseObjects()
        {
            if (playerMovedInThisFrame)
            {
                foreach (var go in gameObjects)
                {
                    var bo = (BaseObject)go;
                    if (bo is Enemy)
                    {

                    }
                } 
            }
        }
        */
        /*
        public override void LoadChunk(int x, int y)
        {
            base.LoadChunk(x, y);

            foreach (var gameObject in chunks[x, y].gameObjects)
            {
                ((BaseObject)gameObject).Game = this;
            }
        }
        */
        private void LoadLevel()
        {
            for (var i = 0; i < 10000; i++)
            {
                var astSize = Util.random.Next(1, 30);
                var ast = new Asteroid(new Vec2i(Util.random.Next(0, worldSize.X), Util.random.Next(0, worldSize.Y)),
                new Vec2i(astSize, astSize), this);
                AddBaseObject(ast);
            }
        }
    }
}
