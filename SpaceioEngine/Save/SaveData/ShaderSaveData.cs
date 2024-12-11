using System;

namespace ConsoleEngine
{
    [DataContract]
    internal class ShaderSaveData
    {
        [DataMember] public string MethodName;
        [DataMember] public string TargetType;
        [DataMember] public object[] Args;

    }
}