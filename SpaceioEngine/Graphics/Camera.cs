using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract(IsReference = true)]
    public class Camera
    {
        // Position refers to the bottom left corner of the camera view.
        [DataMember]
        public Vec2i Position { get; set; }
        [DataMember]
        public Vec2i Size { get; private set; } // Does not allow the change of size, as that would mean needing to re-instantiate the Renderer in the Engine. To change the size of the camera create a new one in the engine.

        public Camera(Vec2i position, Vec2i size)
        {
            Position = position;
            Size = size;
        }
    }
}
