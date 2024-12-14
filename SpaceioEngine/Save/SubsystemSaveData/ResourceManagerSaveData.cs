using System;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract]
    internal class ResourceManagerSaveData
    {
        [DataMember] public ResIDManagerSaveData ResIDManager;
    }
}