﻿using System;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract]
    public class Button
    {
        [DataMember]
        public bool IsPressed { get; set; }
        // Last time when IsPressed switched from false to true, (NOT when it was last held!)
        [DataMember]
        public DateTime LastPressed { get; set; }
        public Button()
        {
            IsPressed = false;
        }
    }
}