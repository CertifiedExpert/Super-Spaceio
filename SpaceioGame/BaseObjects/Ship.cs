using System.Runtime.Serialization;
using ConsoleEngine;

namespace Spaceio
{
    [DataContract(IsReference = true)]
    class Ship : BaseObject
    {
        [DataMember]
        public Vec2f Velocity { get; set; } // Stored in units moved per second (not taking mass into account)
        [DataMember]
        public int Mass { get; set; }
        [DataMember]
        public int ThrustStrength { get; set; }

        [DataMember]
        protected Sprite[] movementSprites = new Sprite[8]; // index 0: facing right, next indexes rotating counter clockwise
        [DataMember]
        private Vec2f restOfVelocity = new Vec2f(0, 0);

        public Ship(Vec2i position, int mass, Game game) : base(position, game)
        {
            Mass = mass;
            Velocity = new Vec2f(0, 0);
        }

        public override void Update()
        {
            base.Update();

            Move();
            UpdateSprite();
        }
        
        public void ApplyForce(Vec2f force)
        {
            Velocity += force / Mass;
        }

        private void UpdateSprite()
        {
            var nVelocity = new Vec2f(Velocity.X + restOfVelocity.X, Velocity.Y + restOfVelocity.Y);
            nVelocity.Normalize();
            var ang = (new Vec2f(1, 0)).AngleBetween(nVelocity);

            if (ang >= -22.5f && ang < 22.5f) Sprites[0] = movementSprites[0];
            else if (ang >= 22.5f && ang < 67.5f) Sprites[0] = movementSprites[1];
            else if (ang >= 67.5f && ang < 112.5f) Sprites[0] = movementSprites[2];
            else if (ang >= 112.5f && ang < 157.5f) Sprites[0] = movementSprites[3];
            else if ((ang >= 157.5f && ang <= 180f) || (ang >= -180f && ang < -157.5f)) Sprites[0] = movementSprites[4];
            else if (ang >= -157.5f && ang < -112.5f) Sprites[0] = movementSprites[5];
            else if (ang >= -112.5f && ang < -67.5f) Sprites[0] = movementSprites[6];
            else Sprites[0] = movementSprites[7];
        }
        private void Move()
        {
            var totalX = Velocity.X * Game.deltaTime / Mass / 1000 + restOfVelocity.X;
            var finalX = (int)totalX;
            restOfVelocity.X = totalX - finalX;

            var totalY = Velocity.Y * Game.deltaTime / Mass / 1000 + restOfVelocity.Y;
            var finalY = (int)totalY;
            restOfVelocity.Y = totalY - finalY;

            MoveGameObject(finalX, finalY);
        }

        protected override void OnCollision(GameObject collidingObject)
        {
            base.OnCollision(collidingObject);

            Velocity = new Vec2f(0, 0);
            restOfVelocity = new Vec2f(0, 0);
        }
    }
}