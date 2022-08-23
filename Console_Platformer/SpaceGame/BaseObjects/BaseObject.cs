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
    abstract class BaseObject : GameObject
    {
        [DataMember]
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
