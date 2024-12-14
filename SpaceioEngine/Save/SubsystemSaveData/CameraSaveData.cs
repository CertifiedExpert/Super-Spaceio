using System;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract]
    internal class CameraSaveData
    {
        [DataMember] public Vec2i Position;
        [DataMember] public Vec2i Size;
    }
}