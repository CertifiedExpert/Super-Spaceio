using System;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract]
    internal class GameObjectManagerSaveData
    {
        [DataMember] public UIDManagerSaveData UIDManager;
    }
}