using System.Runtime.Serialization;

namespace ConsoleEngine
{
    public class Collider
    {
        public Vec2i AttachmentPos { get; set; }
        public Vec2i Size { get; set; }

        public Collider(Vec2i size)
        {
            AttachmentPos = new Vec2i(0, 0);
            Size = size;
        }
        public Collider(Vec2i size, Vec2i attachmentPos)
        {
            AttachmentPos = attachmentPos;
            Size = size;
        }

        internal Collider(ColliderSaveData saveData)
        {
            AttachmentPos = saveData.AttachmentPos;
            Size = saveData.Size;
        }

        internal ColliderSaveData GetSaveData()
        {
            var sd = new ColliderSaveData();
            sd.AttachmentPos = AttachmentPos;
            sd.Size = Size;
            return sd;
        }
    }
}
