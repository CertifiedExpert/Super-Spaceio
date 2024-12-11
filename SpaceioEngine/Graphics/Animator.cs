using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    [DataContract]
    public class Animator
    {
        private Sprite sprite;
        [DataMember]
        private List<ResID> frames;
        [DataMember]
        private int millisecondsForFrameStep;
        [DataMember]
        private bool loopable;

        [DataMember]
        private int currentFrame = 0;
        [DataMember]
        private DateTime lastFrameUpdate = DateTime.Now;
        public Animator(List<ResID> frames, int millisecondsForFrameStep, bool loopable, Sprite sprite,
                        bool randomizeStartFrame = false)
        {
            this.frames = frames;
            this.millisecondsForFrameStep = millisecondsForFrameStep;
            this.loopable = loopable;
            this.sprite = sprite;

            if (randomizeStartFrame) currentFrame = Util.random.Next(0, frames.Count);
        }

        
        // Gets called every frame by the engine. Updates the animator
        internal void Update()
        {
            if ((DateTime.Now - lastFrameUpdate).TotalMilliseconds >= millisecondsForFrameStep)
            {
                currentFrame++;
                if (currentFrame == frames.Count)
                {
                    if (loopable) currentFrame = 0;
                    else
                    {
                        // If the animator run out of frames, terminate the animation and set the Animator property of Sprite to null
                        sprite.Animator = null;
                        return;
                    }
                }

                sprite.BitmapID = frames[currentFrame];

                lastFrameUpdate = DateTime.Now;
            }
        }
    }
}
