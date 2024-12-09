using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public static class EMath
    {
        // Converts radians to degrees.
        public static float DegToRad(float deg) => (float) (deg / 180 * Math.PI);

        // Converts degrees to radians.
        public static float RadToDeg(float rad) => (float) (rad* 180 / Math.PI);

        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
