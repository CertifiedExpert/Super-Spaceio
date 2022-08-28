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
        public Button D1 { get; private set; }
        public Button D2 { get; private set; }
        public Button D3 { get; private set; }
        public Button D4 { get; private set; }
        public Button D5 { get; private set; }
        public Button D6 { get; private set; }
        public Button D7 { get; private set; }
        public Button D8 { get; private set; }
        public Button D9 { get; private set; }
        public Button D0 { get; private set; }
        public Button W { get; private set; }
        public Button A { get; private set; }
        public Button S { get; private set; }
        public Button D { get; private set; }
        public Button F { get; private set; }
        public Button G { get; private set; }
        public Button H { get; private set; }
        public Button J { get; private set; }
        public Button K { get; private set; }
        public Button L { get; private set; }
        public Button Z { get; private set; }
        public Button X { get; private set; }
        public Button C { get; private set; }
        public Button V { get; private set; }
        public Button B { get; private set; }
        public Button N { get; private set; }
        public Button M { get; private set; }

        public Button Tab { get; private set; }
        public Button CapsLock { get; private set; }
        public Button LeftShift { get; private set; }
        public Button LeftControl { get; private set; }
        public Button BackSpace { get; private set; }
        public Button Enter { get; private set; }

        public Button Delete { get; private set; }

        public Button Escape { get; private set; }
        public Button Space { get; private set; }
        public Button ArrowUp { get; private set; }
        public Button ArrowDown { get; private set; }
        public Button ArrowLeft { get; private set; }
        public Button ArrowRight { get; private set; }

        public ImputManager()
        {
            D1 = new Button();
            D2 = new Button();
            D3 = new Button();
            D4 = new Button();
            D5 = new Button();
            D6 = new Button();
            D7 = new Button();
            D8 = new Button();
            D9 = new Button();
            D0 = new Button();
            W = new Button();
            A = new Button();
            S = new Button();
            D = new Button();
            F = new Button();
            G = new Button();
            H = new Button();
            J = new Button();
            K = new Button();
            L = new Button();
            Z = new Button();
            X = new Button();
            C = new Button();
            V = new Button();
            B = new Button();
            N = new Button();
            M = new Button();

            Tab = new Button();
            CapsLock = new Button();
            LeftShift = new Button();
            LeftControl = new Button();
            BackSpace = new Button();
            Enter = new Button();

            Delete = new Button();

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
            UpdateButton(Key.D1, D1);
            UpdateButton(Key.D2, D2);
            UpdateButton(Key.D3, D3);
            UpdateButton(Key.D4, D4);
            UpdateButton(Key.D5, D5);
            UpdateButton(Key.D6, D6);
            UpdateButton(Key.D7, D7);
            UpdateButton(Key.D8, D8);
            UpdateButton(Key.D9, D9);
            UpdateButton(Key.D0, D0);
            UpdateButton(Key.W, W);
            UpdateButton(Key.A, A);
            UpdateButton(Key.S, S);
            UpdateButton(Key.D, D);
            UpdateButton(Key.F, F);
            UpdateButton(Key.G, G);
            UpdateButton(Key.H, H);
            UpdateButton(Key.J, J);
            UpdateButton(Key.K, K);
            UpdateButton(Key.L, L);
            UpdateButton(Key.Z, Z);
            UpdateButton(Key.X, X);
            UpdateButton(Key.C, C);
            UpdateButton(Key.V, V);
            UpdateButton(Key.B, B);
            UpdateButton(Key.N, N);
            UpdateButton(Key.M, M);

            UpdateButton(Key.Tab, Tab);
            UpdateButton(Key.CapsLock, CapsLock);
            UpdateButton(Key.LeftShift, LeftShift);
            UpdateButton(Key.LeftCtrl, LeftControl);
            UpdateButton(Key.Back, BackSpace);
            UpdateButton(Key.Enter, Enter);

            UpdateButton(Key.Up, ArrowUp);
            UpdateButton(Key.Down, ArrowDown);
            UpdateButton(Key.Left, ArrowLeft);
            UpdateButton(Key.Right, ArrowRight);
            UpdateButton(Key.Escape, Escape);
        }

        // Updates the state machine Buttons.
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
