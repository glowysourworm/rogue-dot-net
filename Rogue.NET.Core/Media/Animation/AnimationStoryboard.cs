using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Media.Animation.EventData;
using Rogue.NET.Core.Media.Animation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Media.Animation
{
    /// <summary>
    /// Component that plays a series of AnimationQueue instances
    /// </summary>
    public class AnimationStoryboard : IAnimationPlayer
    {
        public static AnimationStoryboard Empty = new AnimationStoryboard(new AnimationQueue[] { });

        public int AnimationTime { get; private set; }

        public event SimpleEventHandler<IAnimationPlayer, AnimationPlayerChangeEventData> AnimationPlayerChangeEvent;
        public event SimpleEventHandler<AnimationPlayerStartEventData> AnimationPlayerStartEvent;
        public event SimpleEventHandler<AnimationTimeChangedEventData> AnimationTimeChanged;

        // Treating a list like a queue to simplfy logic
        List<AnimationQueue> _queue;

        AnimationQueue _workingAnimation;

        // NOTE*** This is a little tricky - but have to cache the "old primitives" between AnimationQueue change-overs
        IEnumerable<AnimationPrimitive> _lastAnimationQueuePrimitives;

        public AnimationStoryboard(IEnumerable<AnimationQueue> animations)
        {
            this.AnimationTime = animations.Sum(x => x.AnimationTime);

            _queue = new List<AnimationQueue>(animations);
            _workingAnimation = null;
            _lastAnimationQueuePrimitives = null;
        }

        public void Start()
        {
            // Check working animation to continue
            if (_workingAnimation != null)
                throw new Exception("Trying to start animation sequence while not completed - try pause / resume");

            if (_queue.Any())
            {
                // Set next group
                _workingAnimation = _queue[0];

                // Hook first primitive for the time changed event
                _workingAnimation.AnimationPlayerStartEvent += OnChildAnimationPlayerStart;
                _workingAnimation.AnimationPlayerChangeEvent += OnChildAnimationPlayerChange;
                _workingAnimation.AnimationTimeChanged += OnChildAnimationTimeChanged;
                _workingAnimation.Start();
            }
        }

        public void Stop()
        {
            if (_workingAnimation != null)
            {
                // Unhook events
                _workingAnimation.AnimationPlayerStartEvent -= OnChildAnimationPlayerStart;
                _workingAnimation.AnimationPlayerChangeEvent -= OnChildAnimationPlayerChange;
                _workingAnimation.AnimationTimeChanged -= OnChildAnimationTimeChanged;

                // Stop the working animation
                _workingAnimation.Stop();

                // Set to null to make certain the queue's state
                _workingAnimation = null;

                // To be careful - set last primitives here
                _lastAnimationQueuePrimitives = null;
            }
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

        private void OnChildAnimationPlayerChange(IAnimationPlayer sender, AnimationPlayerChangeEventData eventData)
        {
            if (_workingAnimation == null ||
                _queue.None())
                throw new Exception("Improper use of Animation Storyboard events");

            // CHILD SEQUENCE FINISEHD: queue next storyboard animation
            if (eventData.SequenceFinished)
            {
                // Get index of working animation
                var animationIndex = _queue.IndexOf(_workingAnimation) + 1;

                // NEXT ANIMATION:  Notify Listeners -> Start next animation
                if (animationIndex < _queue.Count)
                {
                    var nextGroup = _queue[animationIndex];

                    // Unhook working animation
                    _workingAnimation.AnimationPlayerStartEvent -= OnChildAnimationPlayerStart;
                    _workingAnimation.AnimationPlayerChangeEvent -= OnChildAnimationPlayerChange;
                    _workingAnimation.AnimationTimeChanged -= OnChildAnimationTimeChanged;

                    // CHANGE-OVER EVENT: This gets delayed until the next AnimationQueue start event is fired.
                    //                    So, have to cache the "old primitives" to wait for the event.
                    _lastAnimationQueuePrimitives = eventData.OldPrimitives;

                    // Set next group
                    _workingAnimation = nextGroup;

                    // Hook first primitive for the time changed event
                    _workingAnimation.AnimationPlayerStartEvent += OnChildAnimationPlayerStart;
                    _workingAnimation.AnimationPlayerChangeEvent += OnChildAnimationPlayerChange;
                    _workingAnimation.AnimationTimeChanged += OnChildAnimationTimeChanged;
                    _workingAnimation.Start();
                }

                // ANIMATION SEQUENCE COMPLETE:  Notify Listeners -> ()
                else
                {
                    // To be careful - set last primitives here
                    _lastAnimationQueuePrimitives = null;

                    if (this.AnimationPlayerChangeEvent != null)
                        this.AnimationPlayerChangeEvent(this, new AnimationPlayerChangeEventData()
                        {
                            NewPrimitives = null,
                            OldPrimitives = eventData.OldPrimitives,
                            SequenceFinished = true
                        });
                }
            }

            // CHILD SEQUENCE NOT FINISHED:  Else allow continued processing - send new primitives to listeners
            else
            {
                if (this.AnimationPlayerChangeEvent != null)
                    this.AnimationPlayerChangeEvent(this, new AnimationPlayerChangeEventData()
                    {
                        NewPrimitives = eventData.NewPrimitives,
                        OldPrimitives = eventData.OldPrimitives,
                        SequenceFinished = false
                    });
            }
        }

        private void OnChildAnimationPlayerStart(AnimationPlayerStartEventData eventData)
        {
            if (_workingAnimation == null ||
                _queue.None())
                throw new Exception("Improper use of Animation Storyboard events");

            // CHILD SEQUENCE STARTED: Check for the first element in this sequence - propagate just those primitives
            if (_workingAnimation == _queue.First())
            {
                // Use the sender's event data to propagate animation primitives
                if (this.AnimationPlayerStartEvent != null)
                    this.AnimationPlayerStartEvent(new AnimationPlayerStartEventData()
                    {
                        Primitives = eventData.Primitives
                    });
            }
            // CHANGE-OVER EVENT:  Between AnimationQueue children
            else
            {
                if (_lastAnimationQueuePrimitives == null)
                    throw new Exception("Last animation primitives not set properly");

                // DELAYED CHANGE-OVER EVENT:  This event is delayed because the AnimationQueue needs to propagate the next
                //                             set of primitives.
                //
                if (this.AnimationPlayerChangeEvent != null)
                    this.AnimationPlayerChangeEvent(this, new AnimationPlayerChangeEventData()
                    {
                        NewPrimitives = eventData.Primitives,
                        OldPrimitives = _lastAnimationQueuePrimitives
                    });
            }
        }

        private void OnChildAnimationTimeChanged(AnimationTimeChangedEventData eventData)
        {
            if (_workingAnimation == null)
                throw new Exception("Improper use of animation queues in AnimationStoryboard");

            // Calculate offset
            var animationIndex = _queue.IndexOf(_workingAnimation);

            // Sum over previous elements + current time from the working animation
            var offsetTime = Enumerable.Range(0, animationIndex).Sum(x => _queue[x].AnimationTime) + eventData.CurrentTimeMilliseconds;

            if (this.AnimationTimeChanged != null)
                this.AnimationTimeChanged(new AnimationTimeChangedEventData(offsetTime));
        }
    }
}
