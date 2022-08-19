using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Engine
{
    class Collider
    {
        public Vec2i AttachmentPos { get; set; }
        public Vec2i Size { get; set; }

        public Collider(Vec2i size, Vec2i attachmentPos = null)
        {
            if (attachmentPos == null) AttachmentPos = new Vec2i(0, 0);
            else AttachmentPos = attachmentPos;
            Size = size;
        }
    }
}
