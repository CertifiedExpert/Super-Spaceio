using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Console_Platformer.Engine
{
    [DataContract(IsReference = true)]
    class Camera
    {
        // Position refers to the bottom left corner of the camera view
        [DataMember]
        public Vec2i Position { get; set; }
        [DataMember]
        public Vec2i Size { get; set; }

        public Camera(Vec2i position, Vec2i size)
        {
            Position = position.Copy();
            Size = size.Copy();
        }
    }
}
