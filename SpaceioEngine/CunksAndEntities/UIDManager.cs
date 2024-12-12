using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public class UIDManager
    {
        private List<UID> freeUIDs = new List<UID>();
        private uint totalUIDcount = 50;

        internal UID GenerateUID()
        {
            if (freeUIDs.Count == 0) 
            {
                FillUIDPool(totalUIDcount, totalUIDcount + 50);
            }

            var uid = freeUIDs[0];
            freeUIDs.RemoveAt(0);
            return uid;
        }
        internal void RetireUID(UID uid) => freeUIDs.Add(new UID(uid.BaseID, uid.Generation + 1));
        private void FillUIDPool(uint from, uint to) 
        {
            for (var i = from; i < to; i++) 
            {
                freeUIDs.Add(new UID(i, 0));
            }
        }
    }
}
