using Console_Platformer.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Platformer
{
    class Enemy : Ship
    {
        public Enemy(Vec2i position, int mass, Game game) : base(position, mass, game)
        {
            Sprites[0] = new Sprite(ResourceManager.enemyDefault);

            Colliders.Add(new Collider(new Vec2i(13, 3), new Vec2i(5, 0)));
            Colliders.Add(new Collider(new Vec2i(17, 1), new Vec2i(1, 0)));
            Colliders.Add(new Collider(new Vec2i(18, 5), new Vec2i(0, 4)));
            Colliders.Add(new Collider(new Vec2i(17, 2), new Vec2i(1, 9)));
            Colliders.Add(new Collider(new Vec2i(14, 5), new Vec2i(1, 11)));
            Colliders.Add(new Collider(new Vec2i(10, 2), new Vec2i(3, 16)));

            SpriteLevel = 7;

            for (var i = 0; i < movementSprites.Length; i++) movementSprites[i] = new Sprite(ResourceManager.asteroid);
            ThrustStrength = 1;
        }

        public override void Update()
        {
            base.Update();

            //var pos = (Vec2f)(Game.playerShip.Position - Position);
            //pos.Normalize();
            //Velocity = pos * 10;
        }
    }
}
