using System.Collections.Generic;
using System.Runtime.Serialization;
using ConsoleEngine;

namespace Spaceio
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

            OnChunkTraverse(Position.X / Game.Settings.chunkSize, Position.Y / Game.Settings.chunkSize); // Call this with the current position in order to load in chunks for the first time
        }

        public override void Update()
        {
            base.Update();

            MoveThroughInput();
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

        private void MoveThroughInput()
        {
            if (Game.millisecondsSinceLastPlayerMove > Game.millisecondsPerPlayerMove)
            {
                if (Game.InputManager.W.IsPressed && Game.InputManager.D.IsPressed) ApplyForce(new Vec2f(1, 1) * ThrustStrength);
                else if (Game.InputManager.W.IsPressed && Game.InputManager.A.IsPressed) ApplyForce(new Vec2f(-1, 1) * ThrustStrength);
                else if (Game.InputManager.S.IsPressed && Game.InputManager.D.IsPressed) ApplyForce(new Vec2f(1, -1) * ThrustStrength);
                else if (Game.InputManager.S.IsPressed && Game.InputManager.A.IsPressed) ApplyForce(new Vec2f(-1, -1) * ThrustStrength);
                else if (Game.InputManager.W.IsPressed) ApplyForce(new Vec2f(0, 1) * ThrustStrength);
                else if (Game.InputManager.A.IsPressed) ApplyForce(new Vec2f(-1, 0) * ThrustStrength);
                else if (Game.InputManager.S.IsPressed) ApplyForce(new Vec2f(0, -1) * ThrustStrength);
                else if (Game.InputManager.D.IsPressed) ApplyForce(new Vec2f(1, 0) * ThrustStrength);

                Game.playerMovedInThisFrame = true;
                Game.millisecondsSinceLastPlayerMove = 0;
            }
            else
            {
                Game.playerMovedInThisFrame = false;
                Game.millisecondsSinceLastPlayerMove += Game.deltaTime;
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
                    if (Game.ChunkManager.IsChunkLoaded(x, y)) chunksToBeUnloaded.Add(Game.chunks[x, y]);
                }
            }

            var beginX = chunkX - Game.chunkLoadRadius + 1;
            var beginY = chunkY - Game.chunkLoadRadius + 1;
            if (beginX < 0) beginX = 0;
            if (beginY < 0) beginY = 0;
            var endX = chunkX + Game.chunkLoadRadius - 1;
            var endY = chunkY + Game.chunkLoadRadius - 1;
            if (endX >= Game.chunks.GetLength(0)) endX = 0;
            if (endY >= Game.chunks.GetLength(1)) endY = 0;

            for (var y = beginY; y <= endY; y++)
            {
                for (var x = beginX; x <= endX; x++)
                {
                    if (Game.ChunkManager.IsChunkLoaded(x, y))
                    {
                        chunksToBeUnloaded.Remove(Game.chunks[x, y]);
                    }
                    else Game.ChunkManager.LoadChunk(x, y);
                }
            }

            foreach (var chunk in chunksToBeUnloaded) Game.ChunkManager.ScheduleUnloadChunk(chunk.Index.X, chunk.Index.Y);
        }
    }
}