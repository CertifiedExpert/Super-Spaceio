using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Engine
{
    class Util
    {
        public static float RadToDeg(float rad)
        {
            return (float)(rad * 180 / Math.PI);
        }

        public static float DegToRad(float deg)
        {
            return (float)(deg / 180 * Math.PI);
        }
    }
}
