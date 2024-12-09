using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract]
    public class ReadOnlyVec2i // TODO: is this even needed? delet this.
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
