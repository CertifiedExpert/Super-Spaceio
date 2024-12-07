using System.Runtime.Serialization;

namespace SuperSpaceio.Engine
{
    [DataContract]
    class ReadOnlyVec2i
    {
        [DataMember]
        private Vec2i Vec { get; set; }

        public int X => Vec.X;
        public int Y => Vec.Y;

        public ReadOnlyVec2i(Vec2i vec)
        {
            Vec = vec;
        }
    }
}
