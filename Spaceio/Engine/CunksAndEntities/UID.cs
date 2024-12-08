using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Spaceio
{
    readonly struct UID : IEquatable<UID>
    {
        public readonly uint ID;
        public readonly uint Generation;
        public UID(uint ID, uint generation)
        {
            this.ID = ID;
            Generation = generation;
        }
        public bool Equals(UID other) => ID == other.ID && Generation == other.Generation;
        public override string ToString() => $"{ID}-{Generation}";
        public override bool Equals(object obj) => obj is UID other && Equals(other);
        public static bool operator ==(UID left, UID right) => left.Equals(right);
        public static bool operator !=(UID left, UID right) => !left.Equals(right); 
        public override int GetHashCode() => HashCode.Combine(ID, Generation);
    }
}
