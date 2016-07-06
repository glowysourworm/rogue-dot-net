using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rogue.NET.Model.Media
{
    public class AnimationGroup : ITimedGraphic
    {
        public event EventHandler<AnimationTimeChangedEventArgs> AnimationTimeChanged;
        public event TimerElapsedHandler TimeElapsed;
        Animation[] _animations = null;
        int _ct = 0;
        public bool IsElapsed { get; set; }
        public bool IsPaused { get; set; }
        public int ElapsedTime { get; set; }
        public AnimationGroup(Animation[] animations)
        {
            _animations = animations;
            foreach (Animation a in animations)
                a.TimeElapsed += new TimerElapsedHandler(OnAnimationTimeElapsed);
        }
        public void Start()
        {
            this.IsElapsed = false;
            this.IsPaused = false;
            foreach (Animation a in _animations)
            {
                a.AnimationTimeChanged += (obj, e) =>
                {
                    if (AnimationTimeChanged != null)
                        AnimationTimeChanged(this, e);
                };
                a.Start();
            }
        }
        public void Stop()
        {
            this.IsPaused = false;
            foreach (Animation a in _animations)
                a.Stop();
        }
        public void Pause()
        {
            this.IsPaused = true;
            foreach (Animation a in _animations)
                a.Pause();
        }
        public void Seek(int milliSeconds)
        {
            foreach (Animation a in _animations)
                a.Seek(milliSeconds);
        }
        public void Resume()
        {
            this.IsPaused = false;
            foreach (Animation a in _animations)
                a.Resume();
        }
        public void CleanUp()
        {
            for (int i=0;i<_animations.Length;i++)
            {
                _animations[i].CleanUp();
                _animations[i] = null;
            }
        }
        public void SetStartupDelay(int delay)
        {
            foreach (Animation a in _animations)
                a.SetStartupDelay(delay);
        }
        public Graphic[] GetGraphics()
        {
            List<Graphic> list = new List<Graphic>();
            foreach (Animation a in _animations)
                list.AddRange(a.GetGraphics());
            return list.ToArray();
        }
        private void OnAnimationTimeElapsed(ITimedGraphic sender)
        {
            _ct++;
            if (_ct == _animations.Length)
            {
                if (TimeElapsed != null)
                {
                    this.IsElapsed = true;
                    TimeElapsed(this);
                }
            }
        }
    }
}
