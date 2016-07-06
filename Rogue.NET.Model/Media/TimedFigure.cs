using System;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Timers;

namespace Rogue.NET.Model.Media
{
    public class TimedFigure : Graphic, ITimedGraphic
    {
        public event EventHandler<AnimationTimeChangedEventArgs> AnimationTimeChanged;
        public event TimerElapsedHandler TimeElapsed;
 
        Geometry _g;
        Timer _t;
        Timer _startupTimer = null;
        public bool IsElapsed { get; set; }
        public bool IsPaused { get; set; }
        protected override System.Windows.Media.Geometry DefiningGeometry
        {
            get { return _g; }
        }
        public TimedFigure(Geometry g, Brush fill, Brush stroke, double strokeThickness, int time)
        {
            _g = g;
            _t = new Timer(time);
            this.Fill = fill;
            this.Stroke = stroke;
            this.StrokeThickness = StrokeThickness;
        }
        public void Start()
        {
            if (_startupTimer != null)
                return;

            this.IsElapsed = false;
            _t.Elapsed += new ElapsedEventHandler(OnElapsed);
            _t.Enabled = true;
        }
        public void CleanUp()
        {
            _t.Elapsed -= new ElapsedEventHandler(OnElapsed);
            _t.Dispose();
            _t = null;
        }
        public void Stop()
        {
            _t.Stop();
            if (_startupTimer != null)
                _startupTimer.Stop();
        }
        public void Pause()
        {
            throw new NotSupportedException();
        }
        public void Seek(int milliSeconds)
        {
            throw new NotSupportedException();
        }
        public void Resume()
        {
            throw new NotSupportedException();
        }
        public void SetStartupDelay(int delay)
        {
            _startupTimer = new Timer(delay);
            _startupTimer.Elapsed += new ElapsedEventHandler(OnStartupTimerElapsed);
            _startupTimer.Enabled = true;
            _startupTimer.Start();
        }
        public Graphic[] GetGraphics()
        {
            return new Graphic[] { this };
        }
        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            if (TimeElapsed != null)
            {
                this.IsElapsed = true;
                TimeElapsed(this);
            }
        }
        void OnStartupTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _startupTimer.Elapsed -= new ElapsedEventHandler(OnStartupTimerElapsed);
            _startupTimer.Enabled = false;
            _startupTimer.Dispose();
            _startupTimer = null;
            Start();
        }
    }
}
