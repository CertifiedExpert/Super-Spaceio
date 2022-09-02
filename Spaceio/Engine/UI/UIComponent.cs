﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spaceio.Engine.UI
{
    abstract class UIComponent
    {
        public ReadOnlyVec2i _position;
        public Vec2i Position { get; private set; }

        public UIComponent()
        {
            _position = new ReadOnlyVec2i(Position);
        }
    }
}