using System;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract]
    internal class SpriteSaveData
    {
        [DataMember] public ResID BitmapID;
        [DataMember] public Vec2i AttachmentPos;
        [DataMember] public AnimatorSaveData Animator;
        [DataMember] public ShaderSaveData Shader;
    }
}