using System;

namespace ConsoleEngine
{
    [DataContract]
    internal class ResourceManagerSaveData
    {
        [DataMember] public ResIDManagerSaveData ResIDManager;
    }
}