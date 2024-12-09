using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleEngine
{
    public class UIException : Exception
    {
        public UIException() { }
        public UIException(string message) : base(message) { }
        public UIException(string message, Exception inner) : base(message, inner) { }
    }
}
