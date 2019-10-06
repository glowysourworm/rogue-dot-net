using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Media.Animation;
using Rogue.NET.Core.Media.Animation.EventData;
using Rogue.NET.Core.Media.Animation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Media.Animation
{
    /// <summary>
    /// Plays AnimationPrimitiveGroup instances in order
    /// </summary>
    public class AnimationQueue : IAnimationPlayer
    {
        public static AnimationQueue Empty = new AnimationQueue(new AnimationPrimitiveGroup[] { });

        public event SimpleEventHandler<IAnimationPlayer, AnimationPlayerChangeEventData> AnimationPlayerChangeEvent;
        public event SimpleEventHandler<AnimationPlayerStartEventData> AnimationPlayerStartEvent;
        public event SimpleEventHandler<AnimationTimeChangedEventData> AnimationTimeChanged;

        public int AnimationTime { get; private set; }

        // Treating a list like a queue to simplfy logic
        List<AnimationPrimitiveGroup> _queue;

        AnimationPrimitiveGroup _workingAnimation;

        /// <summary>
        /// Stores animations in sequence
        /// </summary>
        /// <param name="primitiveGroups">sequence of animation groups</param>
        public AnimationQueue(IEnumerable<AnimationPrimitiveGroup> animations)
        {
            this.AnimationTime = animations.Sum(x => x.AnimationTime);

            _queue = new List<AnimationPrimitiveGroup>(animations);
            _workingAnimation = null;
        }

        public void Start()
        {
            // Check working animation to continue
            if (_workingAnimation != null)
                throw new Exception("Trying to start animation sequence while not completed - try pause / resume");

            if (_queue.Any())
                StartAnimation(_queue[0]);
        }
        public void Stop()
        {
            if (_workingAnimation != null)
                StopAnimation();
        }
        public void Pause()
        {
            if (_workingAnimation != null)
                _workingAnimation.Pause();
        }
        public void Resume()
        {
            if (_workingAnimation != null)
                _workingAnimation.Resume();
        }

        private void StartAnimation(AnimationPrimitiveGroup nextGroup)
        {
            if (_workingAnimation != null)
                throw new Exception("Working animation not completed before starting next");

            _workingAnimation = nextGroup;

            // Fire start event for listeners
            if (this.AnimationPlayerStartEvent != null)
                this.AnimationPlayerStartEvent(new AnimationPlayerStartEventData()
                {
                    Primitives = _workingAnimation.GetPrimitives()
                });

            // Hook first primitive for the time changed event
            _workingAnimation.AnimationTimeChanged += OnChildAnimationTimeChanged;
            _workingAnimation.AnimationTimeElapsed += OnChildAnimationCompleted;
            _workingAnimation.Start();
        }

        private void StopAnimation()
        {
            if (_workingAnimation == null)
                throw new Exception("Working animation already stopped");

            // Unhook time elapsed event
            _workingAnimation.AnimationTimeElapsed -= OnChildAnimationCompleted;

            // Unhook time changed event
            _workingAnimation.AnimationTimeChanged -= OnChildAnimationTimeChanged;

            // Stop the working animation
            _workingAnimation.Stop();

            // Set to null to make certain the queue's state
            _workingAnimation = null;
        }

        private void OnChildAnimationCompleted(IAnimationPrimitive sender)
        {
            if (_workingAnimation != sender)
                throw new Exception("Working animatino improperly hooked up");

            // Unhook sender
            _workingAnimation.AnimationTimeElapsed -= OnChildAnimationCompleted;

            // Unhook time changed event
            _workingAnimation.AnimationTimeChanged -= OnChildAnimationTimeChanged;

            // Get index of working animation
            var animationIndex = _queue.IndexOf(_workingAnimation) + 1;

            // NEXT ANIMATION:  Notify Listeners -> Start next animation
            if (animationIndex < _queue.Count)
            {
                // Get next animation group
                var nextAnimation = _queue[animationIndex];

                // Notify listeners of a switch-over
                var oldPrimitives = _workingAnimation.GetPrimitives();
                var newPrimitives = nextAnimation.GetPrimitives();

                if (this.AnimationPlayerChangeEvent != null)
                    this.AnimationPlayerChangeEvent(this, new AnimationPlayerChangeEventData()
                    {
                        OldPrimitives = oldPrimitives,
                        NewPrimitives = newPrimitives,
                        SequenceFinished = false
                    });

                // Set working animation to null
                _workingAnimation = null;

                StartAnimation(_queue[animationIndex]);
            }
            // ANIMATION SEQUENCE COMPLETE:  Notify Listeners -> ()
            else
            {
                // First, notify of the primtive change-over
                if (this.AnimationPlayerChangeEvent != null)
                    this.AnimationPlayerChangeEvent(this, new AnimationPlayerChangeEventData()
                    {
                        OldPrimitives = _workingAnimation.GetPrimitives(),
                        NewPrimitives = null,
                        SequenceFinished = true
                    });

                // Set working animation to null
                _workingAnimation = null;
            }
        }

        private void OnChildAnimationTimeChanged(AnimationTimeChangedEventData sender)
        {
            if (this.AnimationTimeChanged != null)
                this.AnimationTimeChanged(sender);
        }
    }
}
