using System.Runtime.Serialization;

namespace SuperSpaceio.Engine
{
    [DataContract(IsReference = true)]
    class Vec2i //TODO: make this a struct
    {
        [DataMember]
        public int X { get; set; }
        [DataMember]
        public int Y { get; set; }
        public Vec2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Vec2i operator +(Vec2i a, Vec2i b)
        {
            return new Vec2i(a.X + b.X, a.Y + b.Y);
        }
        public static Vec2i operator -(Vec2i a, Vec2i b)
        {
            return new Vec2i(a.X - b.X, a.Y - b.Y);
        }
        public static Vec2i operator *(Vec2i a, int b)
        {
            return new Vec2i(a.X * b, a.Y * b);
        }
        public static Vec2i operator /(Vec2i a, int b)
        {
            return new Vec2i(a.X / b, a.Y / b);
        }

        public Vec2i Copy()
        {
            return new Vec2i(X, Y);
        }
    }
}
