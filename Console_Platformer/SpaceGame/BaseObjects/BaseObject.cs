using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceGame.Engine;

namespace SpaceGame.Platformer
{
    abstract class BaseObject : GameObject
    {
        public Game Game { get; private set; }
        public BaseObject(Vec2i position, Game game) : base(position, game)
        {
            Game = game;
        }

        public override void OnCollision(GameObject collidingObject)
        {

        }
    }
}
