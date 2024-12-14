using System.Runtime.Serialization;
using ConsoleEngine;

namespace Spaceio
{
    [DataContract(IsReference = true)]
    abstract class BaseObject : GameObject
    {
        public Game Game { get; set; }

        protected BaseObject(Vec2i position, Game game) : base(position, game)
        {
            Game = game;
        }

        protected override void OnCollision(GameObject collidingObject)
        {

        }
    }
}
