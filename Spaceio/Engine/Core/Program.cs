using SpaceGame;
using System;
using System.Windows.Media;

namespace SuperSpaceio.Engine
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            /*
            var data = new char[30, 30];
            var bt = new Bitmap(data);
            
            for (var x = 0; x < 30; x++)
            {
                for (var y = 0; y < 30; y++)
                {
                    data[x, y] = '.';
                }
            }

            var pos = new Vec2i(1, 1);
            var sx = 3;
            var sy = 5;
            bt.DrawRectangleOutline(pos, new Vec2i(sx, sy), 'X');
            Draw(data);

            Console.ReadLine();
            */
            
            Engine engine = new Game();
        }
        
        /*
        private static void Draw(char[,] data)
        {
            Console.SetCursorPosition(0, 0);

            var finalString = string.Empty;
            for (int y = data.GetLength(1) - 1; y >= 0; y--)
            {
                var line = "";
                for (int x = 0; x < data.GetLength(0); x++)
                {
                    line += data[x, y].ToString();
                    line += " ";
                }

                finalString += line + "\n";
            }

            Console.WriteLine(finalString);
        }
        */
    }
}
