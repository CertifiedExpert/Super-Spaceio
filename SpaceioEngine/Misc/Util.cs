using System;
using System.IO;
using System.Reflection;

namespace ConsoleEngine
{
    public static class Util
    {
        // A global Random.
        public static Random random = new Random();

        // Converts a 2d-array of T instances into a corresponding jagged array of T instances.
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

        // Converts an equal sized jagged array of T instances into a corresponding 2d-array of T instances.
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

        // Gets the assembly directory of the executable
        public static string GetAssemblyDirectory()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }
    }
}
