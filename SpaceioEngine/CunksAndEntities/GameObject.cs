using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    public class GameObject
    {
        public Engine Engine { get; private set; }
        public Vec2i Chunk { get; internal set; }

        public UID UID { get; private set; }
        public ReadOnlyVec2i Position { get; private set; }
        public List<Sprite> Sprites { get; private set; } 
        public List<Collider> Colliders { get; private set; }
        
        private int _spriteLevel;
        public int SpriteLevel 
        {
            get => _spriteLevel;
            set
            {
                // Checks whether the SpriteLevel value is valid and sets it to minimum priority if it's invalid.
                if (value >= 0 && value < Engine.Settings.spriteLevelCount) _spriteLevel = value;
                else _spriteLevel = Engine.Settings.spriteLevelCount;
            }
        }

        protected GameObject(Vec2i position, Engine engine)
        {
            // Initialize UID as invalid, untill the GameObject is added to the engine and GameObjectManager assigns a UID.
            UID = UID.InvalidUID();

            Position = new ReadOnlyVec2i(position);
            Engine = engine;
            SpriteLevel = 5;
            Sprites = new List<Sprite>();
            Colliders = new List<Collider>();
        }

        internal GameObject(Engine engine, Vec2i chunk, GameObjectSaveData saveData)
        {
            Engine = engine;
            Chunk = chunk;

            Sprites = new List<Sprite>();
            Colliders = new List<Collider>();

            UID = saveData.UID;
            SpriteLevel = saveData.SpriteLevel;
            foreach (var spriteSD in saveData.Sprites) 
            {
                var sprite = new Sprite(spriteSD);
                Sprites.Add(sprite);
            }
            foreach (var colliderSD in saveData.Colliders)
            {
                var collider = new Collider(colliderSD);
                Colliders.Add(collider);
            }
        }

        internal void SetUID (UID uid) => UID = uid;

        // Moves the GameObject and returns a boolean to indicate whether the object was moved successfully.
        public virtual bool MoveGameObject(int x, int y)
        {
            // Checks if the whole moved sprite is in world bounds and if it is then moves the sprite.
            if (Position.X + x < Engine.worldSize.X && 
                Position.X + x >= 0 &&
                Position.Y + y < Engine.worldSize.Y &&
                Position.Y + y >= 0)
            {
                // Moves the player.
                Position = new ReadOnlyVec2i(Position.X + x, Position.Y + y);

                // Collision detection.
                if (Colliders.Count > 0)
                {
                    var isColliding = CollisionDetection();

                    if (isColliding)
                    {
                        // Unmove the GameObject because such a movement would result in GameObjects overlapping.
                        Position = new ReadOnlyVec2i(Position.X - x, Position.Y - y);
                        return false;
                    }
                }

                Engine.GameObjectManager.AddToMovedGameObjects(this);

                return true;
            }
            else return false;
        }
        // Is called when a chunk was traversed by the GameObject.
        internal virtual void OnChunkTraverse() { }

        // Returns a boolean to indicate whether a collision was detected. If a collision was detected, it calls OnCollision in both GameObjects.
        private bool CollisionDetection()
        {
            var isColliding = false;
             
            foreach (var c in Engine.ChunkManager.loadedChunks)
            {
                foreach (var other in c.gameObjects.Values)
                {
                    if (this != other && other.Colliders.Count > 0)
                    {
                        if (IsCollidingWith(other))
                        {
                            isColliding = true;

                            // Collision detected
                            OnCollision(other);
                            other.OnCollision(this);
                        }
                    }
                }
            }

            return isColliding;
        }
        // Checks if this GameObject is colliding with a GameObject specified in the parameter. Return the result as a bool.
        public bool IsCollidingWith(GameObject other) // TODO: add a return struct CollissionInfo
        {
            foreach (var col in Colliders)
            {
                foreach (var otherCol in other.Colliders)
                {
                    if (Position.X + col.AttachmentPos.X < other.Position.X + otherCol.AttachmentPos.X + otherCol.Size.X &&
                        Position.X + col.AttachmentPos.X + col.Size.X > other.Position.X + otherCol.AttachmentPos.X &&
                        Position.Y + col.AttachmentPos.Y < other.Position.Y + otherCol.AttachmentPos.Y + otherCol.Size.Y &&
                        Position.Y + col.AttachmentPos.Y + col.Size.Y > other.Position.Y + otherCol.AttachmentPos.Y)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        // Gets called when a collision has been detected. Passes the GameObject which it collided with.
        internal virtual void OnCollision(GameObject collidingObject) { }
        
        // Is called each frame and updates the GameObject. Updated all Animators of GameObject if it has any (if it doesn't, Animation property is null and is ignored).
        public virtual void Update()
        {
            foreach (var sprite in Sprites)
            {
                sprite?.Animator?.Update();
            }
        }

        internal virtual GameObjectSaveData GetSaveData()
        {
            var sd = new GameObjectSaveData();
            sd.UID = UID;
            sd.Position = new Vec2i(Position.X, Position.Y);
            sd.SpriteLevel = SpriteLevel;
            foreach (var sprite in Sprites) 
            {
                var spriteSD = sprite.GetSaveData();
                sd.Sprites.Add(spriteSD);
            }
            foreach (var collider in Colliders) 
            {
                var colliderSD = collider.GetSaveData();
                sd.Colliders.Add(colliderSD);
            }

            return sd;
        }



        // Is called when a Chunk to which the GameObject started belonging during it being unloaded is finally loaded. Calls OnChunkTraverse and sets GameObject.Chunk because when a GameObject is added to an unloaded Chunk is it automatically treated as if it was unloaded itself, so the OnChunkTraverse() or GameObject.Chunk.set() is not called as that would be calling a method on an unloaded GameObject meaning the GameObject must be processed after the chunk awakens to have all data.
        public void OnUnloadedChunkAwake(int chunkX, int chunkY) //TODO: update this when you get around to writinng save/load system
        {
            Chunk = Engine.chunks[chunkX, chunkY];
            OnChunkTraverse(chunkX, chunkY);
        }
        
        public GameObjectSaveData GenerateGameObjectSaveData()
        {
            var sd = new GameObjectSaveData();

            throw new NotImplementedException();

            return sd;
        }
    }
}
