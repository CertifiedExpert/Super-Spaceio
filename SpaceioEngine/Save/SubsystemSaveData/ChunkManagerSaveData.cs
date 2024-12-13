using System;

namespace ConsoleEngine
{
    [DataContract]
    internal class ChunkManagerSaveData
    {
        [DataMember] public List<Vec2i> Indexes;
    }
}