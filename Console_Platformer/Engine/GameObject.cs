﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Console_Platformer.Engine
{
    [DataContract(IsReference = true)]
    abstract class GameObject
    {
        public Engine Engine { get; private set; }
        public Chunk Chunk { get; set; }
        [DataMember]
        public Vec2i Position { get; private set; } //TODO: maybe change this to a readonly Vec2i so that the position cannot be accessed directly
        [DataMember]
        public Sprite[] Sprites { get; private set; } //TODO: perhaps try to add some safety features for indexes etc.
        [DataMember]
        public List<Collider> Colliders { get; private set; }
        [DataMember]
        public bool Collidable { get; set; } // Flag whether the GameObject can collide with other GameObjects. 
        [DataMember]
        public List<GoBind> Binds { get; private set; } // A list of GoBinds refering to this GameObject.
        [DataMember]
        private int _spriteLevel;
        public int SpriteLevel 
        {
            get { return _spriteLevel; }
            set
            {
                // Checks whether the SpriteLevel value is valid and sets it to minimum priority if it's invalid.
                if (value >= 0 && value < Engine.spriteLevelCount) _spriteLevel = value;
                else _spriteLevel = Engine.spriteLevelCount;
            }
        }


        public GameObject(Vec2i position, Engine engine)
        {
            Position = position.Copy();
            Engine = engine;
            Collidable = true;
            SpriteLevel = 5;
            Sprites = new Sprite[Engine.spriteMaxCount];
            Colliders = new List<Collider>();
            Binds = new List<GoBind>();
        }

        // Moves the GameObject and returns a boolean to indicate whether the object was moved successfully
        public virtual bool MoveGameObject(int x, int y)
        {
            // Checks if the whole moved sprite is in world bounds and if it is then moves the sprite.
            if (Position.X + x < Engine.worldSize.X && 
                Position.X + x >= 0 &&
                Position.Y + y < Engine.worldSize.Y &&
                Position.Y + y >= 0)
            {
                // Moves the player.
                Position.X += x;
                Position.Y += y; 

                // Collision detection.
                if (Collidable)
                {
                    var isColliding = CollisionDetection();

                    if (isColliding)
                    {
                        Position.X -= x; // Unmove the GameObject because such a movement would result in GameObjects overlaping.
                        Position.Y -= y;
                        return false;
                    }
                }

                // Chunk traverse detection.
                var newChunkX = Position.X / Engine.chunkSize;
                var newChunkY = Position.Y / Engine.chunkSize;
                if (Chunk != Engine.chunks[newChunkX, newChunkY])
                {
                    Chunk.UnInsertGameObject(this);
                    Chunk = Engine.chunks[newChunkX, newChunkY];

                    if (Engine.IsChunkLoaded(newChunkX, newChunkY))
                    {
                        Chunk.InsertGameObject(this);
                        OnChunkTraverse(newChunkX, newChunkY); 
                    }
                    else
                    {
                        Engine.unloadedChunkTransitionAddGameObjects[newChunkX, newChunkY].Add(this);
                    }
                    
                }

                return true;
            }
            else return false;
        }

        // Returns a boolean to indicate whether a collision was detected. If a collision was detected, it calls OnCollision in both GameObjects.
        private bool CollisionDetection()
        {
            var isColliding = false;
            for (int x = 0; x < Engine.chunks.GetLength(0); x++)
            {
                for (int y = 0; y < Engine.chunks.GetLength(1); y++)
                {
                    if (Engine.IsChunkLoaded(x, y))
                    {
                        foreach (var gameObject in Engine.chunks[x, y].gameObjects)
                        {
                            if (this != gameObject && gameObject.Collidable)
                            {
                                if (IsCollidingWith(gameObject))
                                {
                                    isColliding = true;

                                    // Collsion detected
                                    OnCollision(gameObject);
                                    gameObject.OnCollision(this);
                                }
                            }
                        }
                    }
                }
            }

            return isColliding;
        }
        // Checks if this GameObject is colliding with a GameObject specified in the parameter. Return the result as a bool.
        public bool IsCollidingWith(GameObject other)
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
        // Is called when a chunk was traversed by the GameObject.
        protected virtual void OnChunkTraverse(int chunkX, int chunkY) { }

        // Is called each frame and updates the GameObject. Updated all Animators of GameObject if it has any (if it doesn't, Animation property is null and is ignored).
        public virtual void Update()
        {
            foreach (var sprite in Sprites)
            {
                sprite?.Animator?.Update();
            }
        }
        // Is called when a Chunk to which the GameObject started belonging during it being unloaded is finally loaded. Calls OnChunkTraverse and sets GameObject.Chunk because when a GameObject is added to an unloaded Chunk is it automatically treated as if it was unloaded itself, so the OnChunkTraverse() or GameObject.Chunk.set() is not called as that would be calling a method on an unloaded GameObject meaning the GameObject must be processed after the chunk awakens to have all data.
        public void OnUnloadedChunkAwake(int chunkX, int chunkY)
        {
            Chunk = Engine.chunks[chunkX, chunkY];
            OnChunkTraverse(chunkX, chunkY);
        }
        // Completes data after the GameObject has been serilized and calls OnDeserialized on its GoBinds so that they can bind to their properties as the GameObject is finally existent.
        public virtual void CompleteDataAfterSerialization(Engine engine, Vec2i index)
        {
            Engine = engine;
            Chunk = engine.chunks[index.X, index.Y];

            foreach (var sprite in Sprites)
            {
                if (sprite != null) sprite.OnDeserialization();
            }

            foreach (var bind in Binds) bind.OnDeserialized(this);
        }
        // Prepares the GameObject for deserialization. (Deactivates its binds as the GameObject is about to be deserialized.)
        public virtual void PrepareForDeserialization()
        {
            foreach (var sprite in Sprites)
            {
                if (sprite != null) sprite.PrepareForDeserialization();
            }

            foreach (var bind in Binds)
            {
                bind.IsActive = false;
            }
        }
        // Gets called when a collision has been detected. Passes the GameObject which it collided with.
        public abstract void OnCollision(GameObject collidingObject);
    }
}
