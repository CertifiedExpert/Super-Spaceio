﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    [DataContract]
    public class GameObjectSaveData
    {
        public UID UID;
        public Vec2i Position;
    }
}
