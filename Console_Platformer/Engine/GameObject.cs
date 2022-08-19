using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Engine
{
    abstract class GameObject
    {
        // Which game engine the gameobject belongs to
        public Engine Engine { get; private set; }
        public Vec2i Position { get; private set; } //TODO: maybe change this to a readonly Vec2i so that the position cannot be accessed directly
        public Sprite[] Sprites { get; private set; } //TODO: perhaps try to add some safety features for indexes etc.
        public List<Collider> Colliders { get; set; } 
        public bool Collidable { get; set; } // Flag whether the GameObject can collide with other GameObjects //u

        private int _spriteLevel;
        public int SpriteLevel //u
        {
            get { return _spriteLevel; }
            set 
            {
                // Checks whether the SpriteLevel value is valid and sets it to minimum priority if it's invalid
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
        }

        // Moves the gameObject and return a boolean to indicate whether the object was moved successfully
        public virtual bool MoveGameObject(int x, int y)
        {
            // Checks if the whole moved sprite is in world bounds and if it is then moves the sprite
            if (Position.X + x < Engine.worldSize.X && // Most far right part of the screen - position + size to get the most far right part of the sprite but - 1 because position is counted as the first character to the left INCLUDING it
                Position.X + x >= 0 &&
                Position.Y + y < Engine.worldSize.Y &&
                Position.Y + y >= 0)
            {
                Position.X += x;
                Position.Y += y; // Moves the player

                // Collision detection
                if (Collidable)
                {
                    var isColliding = CollisionDetection();

                    if (isColliding)
                    {
                        Position.X -= x; // Unmove them because such a movement would result in gameObjects overlaping
                        Position.Y -= y;
                        return false;
                    }
                }

                return true;
            }
            else return false;
        }

        // Returns a boolean to indicate whether a collision was detected
        private bool CollisionDetection()
        {
            var isColliding = false;
            foreach (var gameObject in Engine.gameObjects)
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

            return isColliding;
        }
        public bool IsCollidingWith(GameObject gameObject)
        {
            foreach (var col1 in Colliders)
            {
                foreach (var col2 in gameObject.Colliders)
                {
                    if (Position.X < gameObject.Position.X + col2.AttachmentPos.X + col2.Size.X &&
                        Position.X + col1.AttachmentPos.X + col1.Size.X > gameObject.Position.X &&
                        Position.Y < gameObject.Position.Y + col2.AttachmentPos.Y + col2.Size.Y &&
                        Position.Y + col1.AttachmentPos.Y + col1.Size.Y > gameObject.Position.Y)
                    {
                        return true;
                    }  
                }
            }

            return false;
        }

        // Updates the animators of GameObject if it has any (if it doesn't, Animation property is set to null and is ignored)
        public virtual void Update()
        {
            foreach (var sprite in Sprites)
            {
                sprite?.Animator?.Update();
            }
        }
        public abstract void OnCollision(GameObject collidingObject);
    }
}
