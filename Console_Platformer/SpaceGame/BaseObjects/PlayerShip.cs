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
    class PlayerShip : Ship
    {
        public PlayerShip(Vec2i position, int mass, Game game) : base(position, mass, game)
        {
            Sprites[0] = new Sprite(ResourceManager.fighter1Right);
            Colliders.Add(new Collider(new Vec2i(9, 9)));
            SpriteLevel = 0;

            movementSprites[0] = new Sprite(ResourceManager.fighter1Right);
            movementSprites[1] = new Sprite(ResourceManager.fighter1UpRight);
            movementSprites[2] = new Sprite(ResourceManager.fighter1Up);
            movementSprites[3] = new Sprite(ResourceManager.fighter1UpLeft);
            movementSprites[4] = new Sprite(ResourceManager.fighter1Left);
            movementSprites[5] = new Sprite(ResourceManager.fighter1DownLeft);
            movementSprites[6] = new Sprite(ResourceManager.fighter1Down);
            movementSprites[7] = new Sprite(ResourceManager.fighter1DownRight);
            ThrustStrength = 4;

            OnChunkTraverse(Position.X / Game.chunkSize, Position.Y / Game.chunkSize); // Call this with the curren position in order to load in chunks for the first time
        }

        public override void Update()
        {
            base.Update();

            MoveThroughImput();
        }

        public override bool MoveGameObject(int x, int y)
        {
            var result = base.MoveGameObject(x, y);

            if (result)
            {
                Game.Camera.Position.X += x;
                Game.Camera.Position.Y += y;
            }

            return result;
        }

        private void MoveThroughImput()
        {
            if (Game.milisecondsSinceLastPlayerMove > Game.milisecondsPerPlayerMove)
            {
                if (Game.ImputManager.W.IsPressed && Game.ImputManager.D.IsPressed) ApplyForce(new Vec2f(1, 1) * ThrustStrength);
                else if (Game.ImputManager.W.IsPressed && Game.ImputManager.A.IsPressed) ApplyForce(new Vec2f(-1, 1) * ThrustStrength);
                else if (Game.ImputManager.S.IsPressed && Game.ImputManager.D.IsPressed) ApplyForce(new Vec2f(1, -1) * ThrustStrength);
                else if (Game.ImputManager.S.IsPressed && Game.ImputManager.A.IsPressed) ApplyForce(new Vec2f(-1, -1) * ThrustStrength);
                else if (Game.ImputManager.W.IsPressed) ApplyForce(new Vec2f(0, 1) * ThrustStrength);
                else if (Game.ImputManager.A.IsPressed) ApplyForce(new Vec2f(-1, 0) * ThrustStrength);
                else if (Game.ImputManager.S.IsPressed) ApplyForce(new Vec2f(0, -1) * ThrustStrength);
                else if (Game.ImputManager.D.IsPressed) ApplyForce(new Vec2f(1, 0) * ThrustStrength);

                Game.playerMovedInThisFrame = true;
                Game.milisecondsSinceLastPlayerMove = 0;
            }
            else
            {
                Game.playerMovedInThisFrame = false;
                Game.milisecondsSinceLastPlayerMove += Game.deltaTime;
            }
        }

        protected override void OnChunkTraverse(int chunkX, int chunkY)
        {
            base.OnChunkTraverse(chunkX, chunkY);

            var chunksToBeUnloaded = new List<Chunk>();
            for (var x = 0; x < Game.chunks.GetLength(0); x++)
            {
                for (var y = 0; y < Game.chunks.GetLength(1); y++)
                {
                    if (Game.IsChunkLoaded(x, y)) chunksToBeUnloaded.Add(Game.chunks[x, y]);
                }
            }

            var begginX = chunkX - Game.chunkLoadRadius + 1;
            var begginY = chunkY - Game.chunkLoadRadius + 1;
            if (begginX < 0) begginX = 0;
            if (begginY < 0) begginY = 0;
            var endX = chunkX + Game.chunkLoadRadius - 1;
            var endY = chunkY + Game.chunkLoadRadius - 1;
            if (endX >= Game.chunks.GetLength(0)) endX = 0;
            if (endY >= Game.chunks.GetLength(1)) endY = 0;

            for (var y = begginY; y <= endY; y++)
            {
                for (var x = begginX; x <= endX; x++)
                {
                    if (Game.IsChunkLoaded(x, y))
                    {
                        chunksToBeUnloaded.Remove(Game.chunks[x, y]);
                    }
                    else Game.LoadChunk(x, y);
                }
            }

            foreach (var chunk in chunksToBeUnloaded) Game.ScheduleUnloadChunk(chunk.Index.X, chunk.Index.Y);
        }
    }
}