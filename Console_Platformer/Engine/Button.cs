using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Platformer.Engine
{
    class Button
    {
        public bool IsPressed { get; set; }
        // Last time when IsPressed switched from false to true, (NOT when it was last held!)
        public DateTime LastPressed { get; set; }
        public Button()
        {
            IsPressed = false;
        }
    }
}
