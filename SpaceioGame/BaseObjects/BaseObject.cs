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

        public override void OnCollision(GameObject collidingObject)
        {

        }

        public override void CompleteDataAfterDeserialization(Engine engine, Vec2i index)
        {
            base.CompleteDataAfterDeserialization(engine, index);

            Game = (Game)engine;
        }
    }
}
