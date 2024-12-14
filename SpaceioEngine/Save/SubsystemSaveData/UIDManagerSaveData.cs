using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
namespace ConsoleEngine
{
    [DataContract]
    internal class UIDManagerSaveData
    {
        [DataMember] public List<UID> freeUIDs;
        [DataMember] public uint totalUIDcount;
    }
}