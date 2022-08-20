using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Console_Platformer.Engine
{
    class ImputManager
    {
        // Imput state machine
        public Button W { get; private set; }
        public Button A { get; private set; }
        public Button S { get; private set; }
        public Button D { get; private set; }
        public Button Space { get; private set; }
        public Button ArrowUp { get; private set; }
        public Button ArrowDown { get; private set; }
        public Button ArrowLeft { get; private set; }
        public Button ArrowRight { get; private set; }
        public Button Escape { get; private set; }

        public ImputManager()
        {
            W = new Button();
            A = new Button();
            S = new Button();
            D = new Button();
            ArrowUp = new Button();
            ArrowDown = new Button();
            ArrowRight = new Button();
            ArrowLeft = new Button();
            Space = new Button();
            Escape = new Button();
        }

        // Updates imputs with the currently held buttons + deactivates those released
        public void UpdateImput(Engine engine)
        {
            UpdateButton(Key.W, W);
            UpdateButton(Key.A, A);
            UpdateButton(Key.S, S);
            UpdateButton(Key.D, D);
            UpdateButton(Key.Up, ArrowUp);
            UpdateButton(Key.Down, ArrowDown);
            UpdateButton(Key.Left, ArrowLeft);
            UpdateButton(Key.Right, ArrowRight);
            UpdateButton(Key.Escape, Escape);
        }

        public void UpdateButton(Key key, Button button)
        {
            if (Keyboard.IsKeyDown(key))
            {
                if (!button.IsPressed) button.LastPressed = DateTime.Now;
                button.IsPressed = true;
            }
            else
            {
                button.IsPressed = false;
            }
        }
    }
}
