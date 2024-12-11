using System;

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