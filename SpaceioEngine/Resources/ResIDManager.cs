using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public class ResIDManager
    {
        private List<ResID> freeResIDs = new List<ResID>();
        private uint totalResIDcount = 0;

        public ResIDManager() { }
        internal ResIDManager(ResIDManagerSaveData saveData)
        {
            freeResIDs = saveData.freeResIDs;
            totalResIDcount = saveData.totalResIDCount;
        }
        internal ResID GenerateResID()
        {
            if (freeResIDs.Count == 0)
            {
                FillUIDPool(totalResIDcount, totalResIDcount + 50);
            }

            var resID = freeResIDs[0];
            freeResIDs.RemoveAt(0);
            return resID;
        }
        internal void RetireResID(ResID resID) => freeResIDs.Add(new ResID(resID.BaseID, resID.Generation + 1));
        private void FillUIDPool(uint from, uint to)
        {
            for (var i = from; i < to; i++)
            {
                freeResIDs.Add(new ResID(i, 0));
            }
        }

        internal static ResID InvalidResID => new ResID(uint.MaxValue, uint.MaxValue);

        internal ResIDManagerSaveData GetSaveData()
        {
            var sd = new ResIDManagerSaveData();
            sd.freeResIDs = freeResIDs;
            sd.totalResIDCount = totalResIDcount;
            return sd;
        }
    }
}
