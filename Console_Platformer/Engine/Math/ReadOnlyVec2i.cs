using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Console_Platformer.Engine
{
    [DataContract]
    class ReadOnlyVec2i
    {
        [DataMember]
        private Vec2i Vec { get; set; }

        public int X
        {
            get 
            {
                return Vec.X;
            }
        }
        public int Y
        {
            get
            {
                return Vec.Y;
            }
        }

        public ReadOnlyVec2i(Vec2i vec)
        {
            Vec = vec;
        }
    }
}
