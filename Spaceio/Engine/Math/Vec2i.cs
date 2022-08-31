using System.Runtime.Serialization;

namespace Spaceio.Engine
{
    [DataContract(IsReference = true)]
    class Vec2i
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

        static public Vec2i operator +(Vec2i a, Vec2i b)
        {
            return new Vec2i(a.X + b.X, a.Y + b.Y);
        }
        static public Vec2i operator -(Vec2i a, Vec2i b)
        {
            return new Vec2i(a.X - b.X, a.Y - b.Y);
        }
        static public Vec2i operator *(Vec2i a, int b)
        {
            return new Vec2i(a.X * b, a.Y * b);
        }
        static public Vec2i operator /(Vec2i a, int b)
        {
            return new Vec2i(a.X / b, a.Y / b);
        }

        public Vec2i Copy()
        {
            return new Vec2i(X, Y);
        }
    }
}
