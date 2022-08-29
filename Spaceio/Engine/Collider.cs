using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Spaceio.Engine
{
    [DataContract(IsReference = true)]
    class Collider
    {
        [DataMember]
        public Vec2i AttachmentPos { get; set; }
        [DataMember]
        public Vec2i Size { get; set; }

        public Collider(Vec2i size, Vec2i attachmentPos = null)
        {
            if (attachmentPos == null) AttachmentPos = new Vec2i(0, 0);
            else AttachmentPos = attachmentPos;
            Size = size;
        }
    }
}
