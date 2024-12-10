using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract]
    public class Sprite
    {//TODO: derive a NormalSprite and a StaticFillSprite from this to save memory (can constitute to over 50% total memory usage)

        /// <summary>
        /// Upgrade Sprite class. There should be static sprites, animated sprites, shader sprites. All derived from abstract Sprite.
        /// Static has a bitmap. Animated has a bitmap and animator. Shader has a delegate.
        /// </summary>
        public Bitmap Bitmap { get; set; }
        [DataMember]
        public Vec2i AttachmentPos { get; set; }
        [DataMember]
        public Animator Animator { get; set; }

        [DataMember]
        private char[][] jaggedizedBitmapData_serialize;
        public Sprite(Bitmap bitmap, Vec2i attachmentInfo = null, Animator animator = null)
        {
            AttachmentPos = attachmentInfo ?? new Vec2i(0, 0);
            Bitmap = bitmap;
            Animator = animator;
        }
        // Prepares the sprite for deserialization (serializer does not allow for 2d-arrays but allows jagged arrays)
        public void PrepareForDeserialization()
        {
            jaggedizedBitmapData_serialize = Util.Jaggedize2dArray(Bitmap.Data);
        }
        // Called on deserialization of the sprite. Brings it back to it's pre-jaggedized state.
        public void OnDeserialization()
        {
            var data = Util.UnJaggedize2dArray(jaggedizedBitmapData_serialize);
            Bitmap = new Bitmap(data);
            jaggedizedBitmapData_serialize = null;
        }
    }
}
