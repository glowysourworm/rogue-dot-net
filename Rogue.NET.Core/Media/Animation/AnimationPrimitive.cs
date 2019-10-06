using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Shapes;

using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Media.Animation.Interface;
using Rogue.NET.Core.Media.Animation.EventData;

namespace Rogue.NET.Core.Media.Animation
{
    /// <summary>
    /// An animation primitive contains several clocks pertaining to its inherited shape
    /// </summary>
    public class AnimationPrimitive : Shape, IAnimationNotifier, IAnimationController, IAnimationPrimitive
    {
        // List of all animation clocks for this primitive
        List<AnimationClock> _clocks;

        // Saved reference to the first clock to use for firing events for the whole list
        AnimationClock _firstClock;

        // Geometry for the shape
        Geometry _geometry;

        public event SimpleEventHandler<IAnimationPrimitive> AnimationTimeElapsed;
        public event SimpleEventHandler<AnimationTimeChangedEventData> AnimationTimeChanged;

        public int AnimationTime { get; private set; }

        public AnimationPrimitive(Geometry geometry, AnimationClock[] clocks, int animationTime)
        {
            // Set the geometry for this shape
            _geometry = geometry;

            // Set animation clocks
            _clocks = new List<AnimationClock>(clocks);

            _firstClock = _clocks.FirstOrDefault();

            this.AnimationTime = animationTime;
        }

        protected override Geometry DefiningGeometry { get { return _geometry; } }

        public IEnumerable<AnimationPrimitive> GetPrimitives()
        {
            return new AnimationPrimitive[] { this };
        }

        public void Start()
        {
            StartAnimation();
        }
        public void Stop()
        {
            foreach (var clock in _clocks)
                clock.Controller.Stop();
        }
        public void Pause()
        {
            foreach (var clock in _clocks)
                clock.Controller.Pause();
        }
        public void Resume()
        {
            foreach (var clock in _clocks)
                clock.Controller.Resume();
        }
        private void StartAnimation()
        {
            // Hook up first clock to fire events for the group
            if (_firstClock != null)
            {
                // Hook first clock 
                _firstClock.CurrentTimeInvalidated += OnCurrentTimeInvalidated;

                // Hook completed event
                foreach (var clock in _clocks)
                    clock.Completed += new EventHandler(OnClockCompleted);

                // Start clocks
                foreach (var clock in _clocks)
                {
                    //if (clock.IsPaused)
                    //    clock.Controller.Resume();
                    //else
                    //    clock.Controller.Begin();

                    clock.Controller.Resume();
                }
            }
        }

        private void OnCurrentTimeInvalidated(object sender, EventArgs e)
        {
            var clock = sender as AnimationClock;

            if (this.AnimationTimeChanged != null &&
                clock != null)
                this.AnimationTimeChanged(
                    new AnimationTimeChangedEventData(clock.CurrentTime.HasValue ?
                                                 (int)clock.CurrentTime.Value.TotalMilliseconds : 0));
        }

        private void OnClockCompleted(object sender, EventArgs e)
        {
            if (sender == _firstClock &&
                this.AnimationTimeElapsed != null)
                this.AnimationTimeElapsed(this);
            
            // Unhook event
            (sender as AnimationClock).Completed -= OnClockCompleted;

            // Unhook primary clock
            if (sender == _firstClock)
                _firstClock.CurrentTimeInvalidated -= OnCurrentTimeInvalidated;
        }
    }
}
