using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    [DataContract]
    internal class ChunkSaveData
    {
        [DataMember]
        public List<GameObjectSaveData> gameObjects;

        [DataMember]
        public DateTime lastUnloaded;
    }
}
