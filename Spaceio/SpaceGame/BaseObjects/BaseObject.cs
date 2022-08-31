﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Spaceio.Engine;

namespace SpaceGame
{
    [DataContract(IsReference = true)]
    abstract class BaseObject : GameObject
    {
        public Game Game { get; set; }
        public BaseObject(Vec2i position, Game game) : base(position, game)
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
