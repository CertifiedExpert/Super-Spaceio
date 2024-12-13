using System.Runtime.Serialization;

namespace ConsoleEngine
{
    public class Camera
    {
        // Position refers to the bottom left corner of the camera view.
        public Vec2i Position { get; set; }
        public ReadOnlyVec2i Size { get; private set; } // Does not allow the change of size, as that would mean needing to re-instantiate the Renderer in the Engine. To change the size of the camera create a new one in the engine.

        public Camera(Vec2i position, Vec2i size)
        {
            Position = position;
            Size = new ReadOnlyVec2i(size);
        }

        internal Camera(CameraSaveData saveData)
        {
            Position = saveData.Position;
            Size = saveData.Size;
        }

        internal CameraSaveData GetSaveData()
        {
            var sd = new CamerSaveData();
            sd.Position = Position;
            sd.Size = Size;
            return sd;
        }
    }
}
