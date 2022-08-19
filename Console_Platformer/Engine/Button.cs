using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Engine
{
    class Button
    {
        public bool IsPressed { get; set; }
        // NOT last held, but DateTime of the last time IsPressed switched from false to true
        public DateTime LastPressed { get; set; }
        public Button()
        {
            IsPressed = false;
        }
    }
}
