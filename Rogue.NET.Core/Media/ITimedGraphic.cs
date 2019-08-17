using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rogue.NET.Core.Media
{
    public delegate void TimerElapsedHandler(ITimedGraphic sender);
    public interface ITimedGraphic
    {
        event TimerElapsedHandler TimeElapsed;
        event EventHandler<AnimationTimeChangedEventArgs> AnimationTimeChanged;
        int AnimationTime { get; }
        bool IsElapsed { get; set; }
        bool IsPaused { get; set; }
        void Start();
        void Stop();
        void Pause();
        void Resume();
        void Seek(int milliseconds);
        void CleanUp();
        void SetStartupDelay(int delay);
        Graphic[] GetGraphics();        
    }
    public class AnimationTimeChangedEventArgs : EventArgs
    {
        public AnimationTimeChangedEventArgs(int milliSeconds)
        {
            this.CurrentTimeMilliseconds = milliSeconds;
        }
        public int CurrentTimeMilliseconds { get; set; }
    }
}
