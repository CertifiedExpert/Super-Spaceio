using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Engine
{
    class Vec2i
    {
        public int X { get; set; }
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
