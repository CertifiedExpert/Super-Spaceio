using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public readonly struct UID : IEquatable<UID>
    {
        public readonly uint BaseID;
        public readonly uint Generation;
        internal UID(uint ID = uint.MaxValue, uint generation = uint.MaxValue)    
        {
            BaseID = ID;
            Generation = generation;
        }
        public bool Equals(UID other) => BaseID == other.BaseID && Generation == other.Generation;
        public override string ToString() => $"{BaseID}-{Generation}";
        public override bool Equals(object obj) => obj is UID other && Equals(other);
        public static bool operator ==(UID left, UID right) => left.Equals(right);
        public static bool operator !=(UID left, UID right) => !left.Equals(right); 
        public override int GetHashCode() => HashCode.Combine(BaseID, Generation);

        public static UID InvalidUID() => new UID(uint.MaxValue, uint.MaxValue);
    }
}
