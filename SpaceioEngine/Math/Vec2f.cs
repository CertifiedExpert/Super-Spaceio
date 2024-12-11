using System;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract]
    public struct Vec2f 
    {
        [DataMember] public float X { get; set; }
        [DataMember] public float Y { get; set; }
        public float Length => (float)Math.Sqrt(X * X + Y * Y);

        public Vec2f(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vec2f operator +(Vec2f a, Vec2f b)
        {
            return new Vec2f(a.X + b.X, a.Y + b.Y);
        }
        public static Vec2f operator -(Vec2f a, Vec2f b)
        {
            return new Vec2f(a.X - b.X, a.Y - b.Y);
        }
        public static Vec2f operator *(Vec2f a, float b)
        {
            return new Vec2f(a.X * b, a.Y * b);
        }
        public static Vec2f operator /(Vec2f a, float b)
        {
            return new Vec2f(a.X / b, a.Y / b);
        }
        public static Vec2f operator +(Vec2f a, int b)
        {
            return new Vec2f(a.X + b, a.Y + b);
        }
        public static Vec2f operator -(Vec2f a, int b)
        {
            return new Vec2f(a.X - b, a.Y - b);
        }
        public static Vec2f operator *(Vec2f a, int b)
        {
            return new Vec2f(a.X * b, a.Y * b);
        }
        public static Vec2f operator /(Vec2f a, int b)
        {
            return new Vec2f(a.X / b, a.Y / b);
        }

        public static implicit operator Vec2f(Vec2i i)
        {
            return new Vec2f(i.X, i.Y);
        }

        public void Normalize()
        {
            var length = Length;
            X /= length;
            Y /= length;
        }
        public float Dot(Vec2f a)
        {
            return X * a.X + Y * a.Y;
        }
        public float AngleBetween(Vec2f a)
        {
            return EMath.RadToDeg((float)Math.Atan2(X * a.Y - Y * a.X, X * a.X + Y * a.Y));
        }
    }
}
