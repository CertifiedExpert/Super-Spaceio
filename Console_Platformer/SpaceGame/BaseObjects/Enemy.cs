using Console_Platformer.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame
{
    [DataContract(IsReference = true)]
    class Enemy : Ship
    {
        public Enemy(Vec2i position, int mass, Game game) : base(position, mass, game)
        {
            Sprites[0] = new Sprite(ResourceManager.enemyDefault);

            Colliders.Add(new Collider(new Vec2i(3, 4)));

            SpriteLevel = 7;

            for (var i = 0; i < movementSprites.Length; i++) movementSprites[i] = new Sprite(ResourceManager.enemyDefault);
            ThrustStrength = 1;
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
