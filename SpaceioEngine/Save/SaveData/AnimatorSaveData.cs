using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract]
    internal class AnimatorSaveData
    {
        [DataMember] public List<ResID> frames;
        [DataMember] public int millisecondsForFrameStep;
        [DataMember] public bool loopable;
    }
}