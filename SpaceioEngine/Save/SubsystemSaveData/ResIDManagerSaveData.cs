using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    [DataContract]
    internal class ResIDManagerSaveData
    {
        [DataMember] public List<ResID> freeResIDs;
        [DataMember] public uint totalResIDCount;
    }
}
