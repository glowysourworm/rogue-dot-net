using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Media.Animation.EventData;
using Rogue.NET.Core.Media.Animation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Rogue.NET.Core.Media.Animation
{
    public class AnimationPrimitiveGroup : IAnimationController, IAnimationNotifier, IAnimationPrimitive
    {
        public event SimpleEventHandler<AnimationTimeChangedEventData> AnimationTimeChanged;
        public event SimpleEventHandler<IAnimationPrimitive> AnimationTimeElapsed;

        List<AnimationPrimitive> _primitives;

        // Store reference to first primitive to fire time changed event for the group
        AnimationPrimitive _firstPrimitive;

        public int AnimationTime { get; private set; }

        /// <summary>
        /// Constructs a group of primitives with the SAME ANIMATION TIME.
        /// </summary>
        /// <param name="primitives">Animation primitives</param>
        /// <param name="animationTime">Total animation time for EACH PRIMITIVE</param>
        public AnimationPrimitiveGroup(IEnumerable<AnimationPrimitive> primitives, int animationTime)
        {
            this.AnimationTime = animationTime;

            _primitives = new List<AnimationPrimitive>(primitives);
            _firstPrimitive = _primitives.FirstOrDefault();
        }

        /// <summary>
        /// Adds animations from the second group onto this group.
        /// </summary>
        /// <exception cref="ArgumentException">Second group animation time is not equal to this one</exception>
        public void Combine(AnimationPrimitiveGroup group)
        {
            if (group.AnimationTime != this.AnimationTime)
                throw new ArgumentException("Trying to combine two animation groups with differing animation times");

            var primitives = group.GetPrimitives();

            _primitives.AddRange(primitives);
            _firstPrimitive = _primitives.FirstOrDefault();
        }

        public void Start()
        {
            if (_firstPrimitive != null)
            {
                // Hook first primitive for the time changed event
                _firstPrimitive.AnimationTimeChanged += new SimpleEventHandler<AnimationTimeChangedEventData>(OnChildAnimationTimeChanged);

                foreach (var primitive in _primitives)
                {
                    primitive.AnimationTimeElapsed += OnChildAnimationCompleted;
                    primitive.Start();
                }
            }
        }

        public void Stop()
        {
            foreach (var primitive in _primitives)
                primitive.Stop();
        }
        public void Pause()
        {
            foreach (var primitive in _primitives)
                primitive.Pause();
        }
        public void Resume()
        {
            foreach (var primitive in _primitives)
                primitive.Resume();
        }
        public IEnumerable<AnimationPrimitive> GetPrimitives()
        {
            return _primitives;
        }
        private void OnChildAnimationCompleted(IAnimationPrimitive sender)
        {
            // Fire off event to listeners
            if (this.AnimationTimeElapsed != null)
                this.AnimationTimeElapsed(this);

            // Unhook sender
            sender.AnimationTimeElapsed -= OnChildAnimationCompleted;

            // Unhook time changed event
            if (sender == _firstPrimitive)
                _firstPrimitive.AnimationTimeChanged -= OnChildAnimationTimeChanged;
        }

        private void OnChildAnimationTimeChanged(AnimationTimeChangedEventData sender)
        {
            if (this.AnimationTimeChanged != null)
                this.AnimationTimeChanged(sender);
        }
    }
}
