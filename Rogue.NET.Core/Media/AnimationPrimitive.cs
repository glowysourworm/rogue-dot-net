using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Media.Animation;
using System.Timers;

namespace Rogue.NET.Core.Media
{
    public class AnimationPrimitive : Graphic, ITimedGraphic
    {
        public event TimerElapsedHandler TimeElapsed;
        public event EventHandler<AnimationTimeChangedEventArgs> AnimationTimeChanged;

        int _ct = 0;
        Timer _t = null;
        public bool IsElapsed { get; set; }
        public bool IsPaused { get; set; }
        public List<AnimationClock> Clocks { get; set; }
        public void SetAnimations(AnimationClock[] clocks)
        {
            this.Clocks = new List<AnimationClock>(clocks);

            foreach (AnimationClock s in clocks)
                s.Completed += new EventHandler(OnClockCompleted);
        }
        public void Start()
        {
            this.IsElapsed = false;
            this.IsPaused = false;
            StartAnimation();
        }
        public void CleanUp()
        {
            this.Clocks.Clear();
        }
        public void Stop()
        {
            this.IsPaused = false;

            if (_t != null)
                _t.Stop();

            foreach (AnimationClock c in this.Clocks)
                c.Controller.Stop();
        }
        public void Pause()
        {
            this.IsPaused = true;
            foreach (AnimationClock c in this.Clocks)
                c.Controller.Pause();
        }
        public void Seek(int milliSeconds)
        {
            foreach (AnimationClock c in this.Clocks)
                c.Controller.Seek(new TimeSpan(0, 0, 0, 0, milliSeconds), TimeSeekOrigin.BeginTime);
        }
        public void Resume()
        {
            this.IsPaused = false;
            foreach (AnimationClock c in this.Clocks)
                c.Controller.Resume();
        }
        public void SetStartupDelay(int delay)
        {
            _t = new Timer(delay);
            _t.Elapsed += new ElapsedEventHandler(OnStartupDelayElapsed);
            _t.Enabled = true;
        }
        private void OnStartupDelayElapsed(object sender, ElapsedEventArgs e)
        {
            _t.Elapsed -= new ElapsedEventHandler(OnStartupDelayElapsed);
            _t.Enabled = false;
            _t.Dispose();
            _t = null;
            StartAnimation();
        }
        public Graphic[] GetGraphics()
        {
            return new Graphic[] { this };
        }
        private void StartAnimation()
        {
            if (_t != null)
            {
                foreach (AnimationClock c in this.Clocks)
                {
                    if (c.IsPaused)
                        c.Controller.Resume();
                }
                return;
            }

            _ct = 0;
            foreach (AnimationClock c in this.Clocks)
                c.Controller.Resume();

            AnimationClock first = this.Clocks.FirstOrDefault();
            if (first != null)
            {
                first.CurrentTimeInvalidated += (obj, e) => 
                {
                    if (AnimationTimeChanged != null)
                        AnimationTimeChanged(this, 
                            new AnimationTimeChangedEventArgs(first.CurrentTime.HasValue ? (int)first.CurrentTime.Value.TotalMilliseconds : 0));
                };      
            }
        }
        private void OnClockCompleted(object sender, EventArgs e)
        {
            _ct++;
            if (_ct == this.Clocks.Count)
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
