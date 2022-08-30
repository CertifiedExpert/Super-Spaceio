using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Spaceio.Engine
{
    [DataContract]
    class Button
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
