using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Console_Platformer.Engine;

namespace SpaceGame
{
    [DataContract(IsReference = true)]
    class Game : Engine
    {
        // Settings
        public int milisecondsPerPlayerMove = 50;
        public const int chunkLoadRadius = 3;

        public PlayerShip playerShip;
        public GoBind test;

        public int milisecondsSinceLastPlayerMove = 0;
        public bool playerMovedInThisFrame = false;
        protected override void OnLoad()
        {
            Serializer.knownTypes.Add(typeof(Game));
            Serializer.knownTypes.Add(typeof(BaseObject));
            Serializer.knownTypes.Add(typeof(Asteroid));
            Serializer.knownTypes.Add(typeof(Ship));
            Serializer.knownTypes.Add(typeof(PlayerShip));
            Serializer.knownTypes.Add(typeof(Enemy));

            LoadSavedData("reflection");

            //AddNewSavedData("reflection");
            //CreateChunks();
            //
            //var enm = new Enemy(new Vec2i(5, 5), 1, this);
            //AddBaseObject(enm);
            //test = new GoBind(enm, "test", this);
            //
            //
            //playerShip = new PlayerShip(new Vec2i(40, 40), 1, this);
            //AddBaseObject(playerShip);
            //LoadLevel();
        }

        protected override void Update()
        {
            if (Util.random.Next(0, 30) == 5 && test?.IsActive == true) test.Val.Position.X++;
            if (ImputManager.Escape.IsPressed) gameShouldClose = true;

            UpdateCamera();

            //UpdateAllBaseObjects();
            var loadedGameObjectCount = 0;
            var lChunks = 0;
            foreach (var chunk in loadedChunks)
            {
                foreach (var go in chunk.gameObjects)
                {
                    loadedGameObjectCount++;
                }
                lChunks++;
            }
            debugLines[0] = $"Loaded GameObjects: {loadedGameObjectCount}";
            debugLines[3] = $"Camera X: {Camera.Position.X} | Y: {Camera.Position.Y}";
            //debugLines[4] = $"Current chunk X: {playerShip.Chunk?.Index.X} | Y: {playerShip.Chunk?.Index.Y}";
            debugLines[5] = $"Loaded chunks: {lChunks}";
        }

        private void UpdateCamera()
        {
            if (ImputManager.ArrowUp.IsPressed) Camera.Position.Y += 3;
            if (ImputManager.ArrowDown.IsPressed) Camera.Position.Y -= 3;
            if (ImputManager.ArrowLeft.IsPressed) Camera.Position.X -= 3;
            if (ImputManager.ArrowRight.IsPressed) Camera.Position.X += 3;
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

        public override void LoadChunk(int x, int y)
        {
            base.LoadChunk(x, y);

            foreach (var gameObject in chunks[x, y].gameObjects)
            {
                ((BaseObject)gameObject).Game = this;
            }
        }
        private void LoadLevel()
        {
            for (var i = 0; i < 10000; i++)
            {
                var astSize = Random.Next(1, 30);
                var ast = new Asteroid(new Vec2i(Random.Next(0, worldSize.X), Random.Next(0, worldSize.Y)),
                new Vec2i(astSize, astSize), this);
                AddBaseObject(ast);
            }
        }
    }
}
