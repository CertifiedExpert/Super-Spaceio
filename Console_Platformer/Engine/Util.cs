using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Platformer.Engine
{
    class Util
    {
        public static Random random = new Random();
        public static float RadToDeg(float rad)
        {
            return (float)(rad * 180 / Math.PI);
        }

        public static float DegToRad(float deg)
        {
            return (float)(deg / 180 * Math.PI);
        }

        public static T[][] Jaggedize2dArray<T>(T[,] instance)
        {
            var output = new T[instance.GetLength(0)][];

            for (var x = 0; x < instance.GetLength(0); x++)
            {
                output[x] = new T[instance.GetLength(1)];
                for (var y = 0; y < instance.GetLength(1); y++)
                {
                    output[x][y] = instance[x, y];
                }
            }
            return output;
        }

        public static T[,] UnJaggedize2dArray<T>(T[][] instance)
        {
            var output = new T[instance.Length, instance[0].Length];

            for (var x = 0; x < instance.Length; x++)
            {
                for (var y = 0; y < instance[x].Length; y++)
                {
                    output[x, y] = instance[x][y];
                }
            }
            return output;
        }
    }
}
