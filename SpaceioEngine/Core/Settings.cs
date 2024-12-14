using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract]
    public class Settings
    {
        [DataMember] public int spriteLevelCount { get; private set; } // The maximum number of renderer levels a Sprite can have.
        [DataMember] public char backgroudPixel { get; private set; } // The pixel which is used as the background.
        [DataMember] public string pixelSpacingCharacters { get; private set; } // Unchangeable pixels put in between two changeable pixels to account for difference in width and height of unicode characters.
        [DataMember] public int chunkSize { get; private set; } // The size of each chunk (both in X- and Y- axis as the chunk is a square).
        [DataMember] public int CameraSizeX { get; private set; } // The size of the camera in the X-axis.
        [DataMember] public int CameraSizeY { get; private set; } // The size of the camera in the Y-axis.
        [DataMember] public int milisecondsForNextFrame { get; private set; } // Minimum number of milliseconds which needs to pass for the next frame to proceed.

        public Settings(int spriteLevelCount, char backgroundPixel, string pixelSpacingCharacters, int chunkSize, int cameraSizeX,
            int cameraSizeY, int millisecondsForNextFrame)
        {
            this.spriteLevelCount = spriteLevelCount;
            this.backgroudPixel = backgroundPixel;
            this.pixelSpacingCharacters = pixelSpacingCharacters;
            this.chunkSize = chunkSize;
            this.CameraSizeX = cameraSizeX;
            this.CameraSizeY = cameraSizeY;
            this.milisecondsForNextFrame = millisecondsForNextFrame;
        }

    }
}