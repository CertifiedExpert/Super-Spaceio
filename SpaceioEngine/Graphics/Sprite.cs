using System.Runtime.Serialization;

namespace ConsoleEngine
{
    public class Sprite
    {//TODO: derive a NormalSprite and a StaticFillSprite from this to save memory (can constitute to over 50% total memory usage)

        /// <summary>
        /// Upgrade Sprite class. There should be static sprites, animated sprites, shader sprites. All derived from abstract Sprite.
        /// Static has a bitmap. Animated has a bitmap and animator. Shader has a delegate.
        /// </summary>
        public ResID BitmapID { get; set; }
        public Vec2i AttachmentPos { get; set; }
        public Animator Animator { get; set; }
        public Shader Shader { get; set; }

        public Sprite (ResID bitmap)
        {
            BitmapID = bitmap;
            AttachmentPos = new Vec2i (0, 0);
            Shader = new Shader(DefaultShader, null);
        }
        public Sprite(ResID bitmapID, Vec2i attachmentInfo, Shader shader, Animator animator = null)
        {
            AttachmentPos = attachmentInfo;
            BitmapID = bitmapID;
            Shader = shader;
            Animator = animator;
            Shader = shader;
        }

        internal Sprite(SpriteSaveData saveData)
        {
            BitmapID = saveData.BitmapID;
            AttachmentPos = saveData.AttachmentPos;
            Animator = new Animator(this, saveData.Animator);
            Shader = new Shader(saveData.Shader);
        }

        internal SpriteSaveData GetSaveData()
        {
            var sd = new SpriteSaveData();
            sd.BitmapID = BitmapID;
            sd.AttachmentPos = AttachmentPos;
            sd.Animator = Animator.GetSaveData();
            sd.Shader = Shader.GetSaveData();
            return sd;
        }

        public static char DefaultShader(int x, int y, Bitmap bitmap, object[] args) => bitmap.Data[x, y];
    }
}
