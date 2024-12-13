using System;

namespace ConsoleEngine
{
    [DataContract]
    internal class GameObjectManagerSaveData
    {
        [DataMember] public UIDManagerSaveData UIDManager;
    }
}