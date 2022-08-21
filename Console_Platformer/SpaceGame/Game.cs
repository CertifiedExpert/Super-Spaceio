using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Console_Platformer.Engine;

namespace SpaceGame.Platformer
{
    class Game : Engine
    {
        // Settings
        public int milisecondsPerPlayerMove = 50;

        public PlayerShip playerShip;

        public int milisecondsSinceLastPlayerMove = 0;
        public bool playerMovedInThisFrame = false;
        protected override void OnLoad()
        {
            playerShip = new PlayerShip(new Vec2i(40, 40), 1, this);
            AddBaseObject(playerShip);

            LoadLevel();
        }

        protected override void Update()
        {
            if (ImputManager.Escape.IsPressed) gameShouldClose = true;

            MovePlayer();
            UpdateCamera();

            //UpdateAllBaseObjects();
            var loadedGameObjectCount = 0;
            foreach (var c in chunks)
            {
                if (c.IsLoaded)
                {
                    foreach (var go in c.gameObjects)
                    {
                        loadedGameObjectCount++;
                    } 
                }
            }
            debugLines[0] = $"Loaded GameObjects: {loadedGameObjectCount}";
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
        private void MovePlayer()
        {
            
        }

        private void LoadLevel()
        {
            for (var i = 0; i < 100000; i++)
            {
                var astSize = gRandom.Next(1, 30);
                var ast = new Asteroid(new Vec2i(gRandom.Next(0, worldSize.X), gRandom.Next(0, worldSize.Y)),
                    new Vec2i(astSize, astSize), this);
                AddBaseObject(ast);
            }
            //var enm = new Enemy(new Vec2i(50, 20), 1, this);
            //AddBaseObject(enm);

            /*
            for (var i = 0; i < 30; i++)
            {
                var e = new Enemy(new Vec2i(30 + 30 * i, 30), 1, this);
                AddBaseObject(e);
            }
            */
        }
    }
}
