using System;

namespace ConsoleEngine
{
    [DataContract]
    internal class SpriteSaveData
    {
        [DataMember] public ResID ResID;
        [DataMember] public Vec2i AttachmentPos;
        [DataMember] public AnimatorSaveData Animator;
        [DataMember] public ShaderSaveData Shader;
    }
}