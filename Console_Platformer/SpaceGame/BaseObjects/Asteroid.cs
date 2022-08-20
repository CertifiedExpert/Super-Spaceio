using Console_Platformer.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceGame.Platformer
{
    class Asteroid : BaseObject
    {
        public Asteroid(Vec2i position, Vec2i maxSize, Game game) : base(position, game)
        {
            SpriteLevel = 7;

            RandomlyGenerate(maxSize.Copy());
        }

        private void RandomlyGenerate(Vec2i maxSize)
        {
            // Generates 2 random colliders and sprites for the asteroid
            var borderX = (int)(maxSize.X * 0.15f);
            var minXLength = (int)(maxSize.X * 0.35f);
            var borderY = (int)(maxSize.Y * 0.15f);
            var minYLength = (int)(maxSize.Y * 0.35f);
            int corner1, corner2, corner3, corner4;
            corner1 = Game.gRandom.Next(borderX, maxSize.X / 2 - minXLength / 2);             //top
            corner2 = Game.gRandom.Next(maxSize.X / 2 + minXLength / 2, maxSize.X - borderX); //bottom
            corner3 = Game.gRandom.Next(borderY, maxSize.Y / 2 - minYLength / 2);             //left
            corner4 = Game.gRandom.Next(maxSize.Y / 2 + minYLength / 2, maxSize.Y - borderY); //right

            var size1 = new Vec2i(Math.Abs(corner1 - corner2) + 1, maxSize.Y);
            var attach1 = new Vec2i(Math.Min(corner1, corner2), 0);
            var bitmap1 = Bitmap.CreateStaticFillBitmap(size1, '⬛');
            Sprites[0] = new Sprite(bitmap1, attach1); 
            Colliders.Add(new Collider(size1, attach1));

            var size2 = new Vec2i(maxSize.X, Math.Abs(corner3 - corner4) + 1);
            var attach2 = new Vec2i(0, Math.Min(corner3, corner4));
            var bitmap2 = Bitmap.CreateStaticFillBitmap(size2, '⬛');
            Sprites[1] = new Sprite(bitmap2, attach2);
            Colliders.Add(new Collider(size2, attach2));
        }
    }
}
