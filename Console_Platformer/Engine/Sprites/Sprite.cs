using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Platformer.Engine
{
    class Sprite
    {//TODO: maybe derive a NormalSprite and a StaticFillSprite from this to save memory
        public Bitmap Bitmap { get; set; }
        public Vec2i AttachmentPos { get; set; }
        public Animator Animator { get; set; }

        public Sprite(Bitmap bitmap, Vec2i attachmentInfo = null, Animator animator = null)
        {
            if (attachmentInfo == null) AttachmentPos = new Vec2i(0, 0);
            else AttachmentPos = attachmentInfo;
            Bitmap = bitmap;
            Animator = animator;
        }
    }
}
