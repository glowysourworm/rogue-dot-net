using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rogue.NET.Model.Media
{
    public class Animation : ITimedGraphic
    {
        public event EventHandler<AnimationTimeChangedEventArgs> AnimationTimeChanged;

        static Animation _empty = new Animation(new ITimedGraphic[]{});
        public static Animation Empty { get { return _empty; } }
        public event TimerElapsedHandler TimeElapsed;

        ITimedGraphic[] _a = null;
        int _ct = 0;
        public bool IsElapsed { get; set; }
        public bool IsPaused { get; set; }
        public Animation(ITimedGraphic[] primitives)
        {
            _a = primitives;
            foreach (ITimedGraphic p in primitives)
            {
                if (!(p is Graphic))
                    throw new NotSupportedException("Use only Rogue.NET Graphic in the Animation constructor");

                p.TimeElapsed += new TimerElapsedHandler(OnChildAnimationCompleted);
            }
        }
        public void CleanUp()
        {
            for (int i = _a.Length - 1; i >= 0; i--)
            {
                _a[i].CleanUp();
                _a[i] = null;
            }

            _a = null;
        }
        public void Start()
        {
            this.IsPaused = false;
            this.IsElapsed = false;
            StartAnimation();
        }
        public void Stop()
        {
            foreach (ITimedGraphic p in _a)
                p.Stop();
        }
        public void Pause()
        {
            this.IsPaused = true;
            foreach (ITimedGraphic p in _a)
                p.Pause();
        }
        public void Seek(int milliseconds)
        {
            foreach (ITimedGraphic p in _a)
                p.Seek(milliseconds);
        }
        public void Resume()
        {
            this.IsPaused = false;
            foreach (ITimedGraphic p in _a)
                p.Resume();
        }
        public void SetStartupDelay(int delay)
        {
            foreach (ITimedGraphic p in _a)
                p.SetStartupDelay(delay);
        }
        public Graphic[] GetGraphics()
        {
            List<Graphic> list = new List<Graphic>();
            foreach (ITimedGraphic g in _a)
                list.Add(g as Graphic);

            return list.ToArray();
        }
        private void StartAnimation()
        {
            _ct = 0;
            foreach (ITimedGraphic p in _a)
            {
                p.AnimationTimeChanged += (obj, e) =>
                {
                    if (AnimationTimeChanged != null)
                        AnimationTimeChanged(this, e);
                };
                p.Start();
            }
        }
        private void OnChildAnimationCompleted(ITimedGraphic sender)
        {
            _ct++;
            if (_ct == _a.Length)
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
