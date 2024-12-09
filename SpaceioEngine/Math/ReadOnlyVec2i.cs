using System;
using System.CodeDom;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    // This is used with fields { public get; private set; } If you want to retrieve a value
    // access it directly. If you want to modify a value, create a whole new ReadOnlyVec2i with the new value.
    // Creating new ReadOnlyVec2i can only be done privately in the original class

    [DataContract]
    public struct ReadOnlyVec2i 
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        
        public ReadOnlyVec2i(Vec2i vec)
        {
            X = vec.X;
            Y = vec.Y;
        }
        public ReadOnlyVec2i(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator Vec2i(ReadOnlyVec2i readOnlyVec) => new Vec2i(readOnlyVec.X, readOnlyVec.Y);
    }
}
