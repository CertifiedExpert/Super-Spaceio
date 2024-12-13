using System;

namespace ConsoleEngine
{
    [DataContract]
    internal class UIDManagerSaveData
    {
        [DataMember] public List<UID> freeUIDs;
        [DataMember] public uint totalUIDcount;
    }
}