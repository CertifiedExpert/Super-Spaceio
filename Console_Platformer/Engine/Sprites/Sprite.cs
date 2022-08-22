﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Console_Platformer.Engine
{
    [DataContract]
    class Sprite
    {//TODO: derive a NormalSprite and a StaticFillSprite from this to save memory (can constitute to over 50% total memory usage)
        public Bitmap Bitmap { get; set; }
        [DataMember]
        public Vec2i AttachmentPos { get; set; }
        [DataMember]
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
