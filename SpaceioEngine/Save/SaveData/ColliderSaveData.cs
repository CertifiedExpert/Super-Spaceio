using System;

namespace ConsoleEngine
{
    [DataContract]
    internal class ColliderSaveData
    {
        [DataMember] public Vec2i AttachmentPos;
        [DataMember] public Vec2i Size;
    }
}