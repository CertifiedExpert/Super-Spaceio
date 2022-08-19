using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Engine
{
    class Camera
    {
        // Position refers to the bottom left corner of the camera view
        public Vec2i Position { get; set; }
        public Vec2i Size { get; set; }

        public Camera(Vec2i position, Vec2i size)
        {
            Position = position.Copy();
            Size = size.Copy();
        }
    }
}
