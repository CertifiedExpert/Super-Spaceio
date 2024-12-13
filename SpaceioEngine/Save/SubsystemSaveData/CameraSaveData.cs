using System;

namespace ConsoleEngine
{
    [DataContract]
    internal class CameraSaveData
    {
        [DataMember] Vec2i Position;
        [DataMember] Vec2i Size;
    }
}