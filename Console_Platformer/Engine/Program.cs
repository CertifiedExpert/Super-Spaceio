using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpaceGame.Platformer;

namespace SpaceGame.Engine
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Engine engine = new Game();
        }
    }
}
