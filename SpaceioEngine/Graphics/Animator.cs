using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ConsoleEngine
{
    public class Animator
    {
        private Sprite sprite;
        private List<ResID> frames;
        private int millisecondsForFrameStep;
        private bool loopable;

        private int currentFrame = 0;
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
        
        internal Animator(Sprite sprite, AnimatorSaveData animatorSaveData)
        {
            this.sprite = sprite;
            frames = animatorSaveData.frames;
            millisecondsForFrameStep = animatorSaveData.millisecondsForFrameStep;
            loopable = animatorSaveData.loopable;
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

        internal AnimatorSaveData GetSaveData()
        {
            var sd = new AnimatorSaveData();
            sd.frames = frames;
            sd.millisecondsForFrameStep = millisecondsForFrameStep;
            sd.loopable = loopable;
            return sd;
        }
    }
}
