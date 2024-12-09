using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract(IsReference = true)]
    public class Collider
    {
        [DataMember]
        public Vec2i AttachmentPos { get; set; }
        [DataMember]
        public Vec2i Size { get; set; }

        public Collider(Vec2i size, Vec2i attachmentPos = null)
        {
            AttachmentPos = attachmentPos ?? new Vec2i(0, 0);
            Size = size;
        }
    }
}
