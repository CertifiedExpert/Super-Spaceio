using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract]
    internal class ChunkManagerSaveData
    {
        [DataMember] public List<Vec2i> Indexes;
    }
}