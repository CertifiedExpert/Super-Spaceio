using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract]
    public struct Vec2i 
    {
        [DataMember] public int X { get; set; }
        [DataMember] public int Y { get; set; }
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

        
        public override bool Equals(object obj)
        {
            if (obj is Vec2i other)
            {
                return this == other;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public static bool operator ==(Vec2i left, Vec2i right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        public static bool operator !=(Vec2i left, Vec2i right)
        {
            return !(left == right);
        }
    }
}
