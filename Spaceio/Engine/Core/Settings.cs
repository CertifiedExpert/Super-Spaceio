﻿using System.Runtime.Serialization;

namespace Spaceio.Engine
{
    [DataContract(IsReference = true)]
    class Settings
    {
        [DataMember] public int spriteMaxCount { get; private set; } // The maximum number of sprites a GameObject may have.
        [DataMember] public int spriteLevelCount { get; private set; } // The maximum number of renderer levels a Sprite can have.
        [DataMember] public char backgroudPixel { get; private set; } // The pixel which is used as the background.
        [DataMember] public string pixelSpacingCharacters { get; private set; } // Unchangeable pixels put in between two changeable pixels to account for difference in width and height of unicode characters.
        [DataMember] public string title { get; private set; } // The title of the console window.
        [DataMember] public int chunkCountX { get; private set; } // The number of chunks in the X-axis.
        [DataMember] public int chunkCountY { get; private set; } // The number of chunks in the Y-axis.
        [DataMember] public int chunkSize { get; private set; } // The size of each chunk (both in X- and Y- axis as the chunk is a square.
        [DataMember] public int CameraSizeX { get; private set; } // The size of the camera in the X-axis.
        [DataMember] public int CameraSizeY { get; private set; } // The size of the camera in the Y-axis.
        [DataMember] public Vec2i CameraStartPosition { get; private set; } // The start position of the camera.
        [DataMember] public int milisecondsForNextFrame { get; private set; } // Minimum number of milliseconds which needs to pass for the next frame to proceed.
    }
}
