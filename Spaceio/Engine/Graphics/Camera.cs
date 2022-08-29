using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Spaceio.Engine
{
    [DataContract(IsReference = true)]
    class Camera
    {
        // Position refers to the bottom left corner of the camera view.
        [DataMember]
        public Vec2i Position { get; set; }
        [DataMember]
        public Vec2i Size { get; private set; } // Does not allow the change of size, as that would mean needing to reinstantiate the Renderer in the Engine. To change the size of the camera create a new one in the engine.

        public Camera(Vec2i position, Vec2i size)
        {
            Position = position.Copy();
            Size = size.Copy();
        }
    }
}
