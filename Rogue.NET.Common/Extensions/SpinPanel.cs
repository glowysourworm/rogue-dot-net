using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Collections;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Rogue.NET.Common.Extensions
{
    public partial class SpinPanel : Panel
    {
        private SolidColorBrush _ellipseBrush = new SolidColorBrush(Color.FromArgb(0x0f, 0xff, 0xff, 0xff));

        public event EventHandler ObjectSelected;
        public event EventHandler ObjectCentered;

        /// <summary>
        /// Mouse to Animation velocity
        /// </summary>
        public int AnimationVelocity { get; set; }

        /// <summary>
        /// Padding for Path Geometry
        /// </summary>
        public Thickness Padding { get; set; }

        /// <summary>
        /// Set to true to show a hand cursor over the element
        /// and to allow ObjectSelected Event to fire
        /// </summary>
        public bool IsSelectable { get; set; }

        /// <summary>
        /// Specifies that the spin panel will seek to the nearest element after scrubbing
        /// </summary>
        public bool SeekToNearestBehavior { get; set; }

        public double FontSize
        {
            get { return _centerText.FontSize; }
            set { _centerText.FontSize = value; }
        }
        public Brush Foreground
        {
            get { return _centerText.Foreground; }
            set { _centerText.Foreground = value; }
        }
        public string CenterText
        {
            get { return _centerText.Text; }
            set { _centerText.Text = value; }
        }

        TextBlock _centerText = new TextBlock();

        private class AnimationBundle
        {
            AnimationClock _x;
            AnimationClock _y;
            AnimationClock _opacity;
            AnimationClock _sizex;
            AnimationClock _sizey;
            FrameworkElement _e;

            public FrameworkElement GetElement()
            {
                return _e;
            }
            public TimeSpan CurrentTime
            {
                get { return _x.CurrentTime.Value; }
            }

            public TimeSpan Duration
            {
                get { return _x.NaturalDuration.TimeSpan; }
            }
            public void Seek(TimeSpan s)
            {
                _x.Controller.Seek(s, TimeSeekOrigin.BeginTime);
                _y.Controller.Seek(s, TimeSeekOrigin.BeginTime);
                _opacity.Controller.Seek(s, TimeSeekOrigin.BeginTime);
                _sizex.Controller.Seek(s, TimeSeekOrigin.BeginTime);
                _sizey.Controller.Seek(s, TimeSeekOrigin.BeginTime);
            }
            public bool IsCentered { get; set; }

            public AnimationBundle(AnimationClock x, AnimationClock y, AnimationClock opacity, AnimationClock sx, AnimationClock sy, FrameworkElement e)
            {
                _x = x;
                _y = y;
                _opacity = opacity;
                _sizex = sx;
                _sizey = sy;
                _e = e;
                this.IsCentered = false;
            }
        }

        Point _mouseDownPoint = new Point(0, 0);
        List<AnimationBundle> _list = new List<AnimationBundle>();

        private const int _TOTAL_MILLISEC = 1000;
        public SpinPanel()
        {
            this.Children.Add(_centerText);
            _centerText.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            _centerText.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            Grid.SetColumnSpan(_centerText, 2);
            Panel.SetZIndex(_centerText, 0);
            this.Cursor = Cursors.ScrollWE;

            //INitialize to true
            this.SeekToNearestBehavior = true;

            this.Loaded += new RoutedEventHandler(SpinPanel_Loaded);
        }

        private void SpinPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Padding == null)
                this.Padding = new Thickness(16);
            this.AnimationVelocity = 1;
            InitializeElements();
        }
        private PathGeometry GetDefaultGeometry()
        {
            double maxHeight = double.MinValue;
            double maxWidth = double.MinValue;
            for (int i = 0; i < this.VisualChildrenCount; i++)
            {
                FrameworkElement e = GetVisualChild(i) as FrameworkElement;
                if (_centerText == e)
                    continue;

                if (e.Width > maxWidth)
                    maxWidth = e.Width;

                if (e.Height > maxHeight)
                    maxHeight = e.Height;
            }
            Rect r = new Rect(this.RenderSize);
            r.X += (maxWidth / 2) + this.Padding.Left;
            r.Y += (maxHeight / 2) + this.Padding.Top;
            r.Width = Math.Max(r.Width - (maxWidth + this.Padding.Left + this.Padding.Right), 10);
            r.Height = Math.Max(r.Height - (maxHeight + this.Padding.Top + this.Padding.Bottom), 10);
            return (new EllipseGeometry(r)).GetFlattenedPathGeometry();
        }

        public void InitializeElements()
        {
            _list.Clear();

            if (this.VisualChildrenCount == 1)
                return;

            //Calculate geometry based on element sizes
            PathGeometry pg = GetDefaultGeometry();

            //Set all elements
            for (int i = 0; i < this.VisualChildrenCount; i++)
            {
                FrameworkElement e = this.GetVisualChild(i) as FrameworkElement;
                if (e == _centerText)
                    continue;

                e.MouseDown += new MouseButtonEventHandler(OnElementMouseDown);
                ScaleTransform st = new ScaleTransform();
                TranslateTransform ttCenter = new TranslateTransform(-1 * e.Width / 2, -1 * e.Height / 2);
                TranslateTransform tt = new TranslateTransform();
                TransformGroup g = new TransformGroup();
                g.Children.Add(st);
                g.Children.Add(tt);
                g.Children.Add(ttCenter);
                e.RenderTransform = g;

                //Settings to allow container controlled rendering for elements
                e.HorizontalAlignment = HorizontalAlignment.Left;
                e.VerticalAlignment = VerticalAlignment.Top;

                DoubleAnimationUsingPath x = new DoubleAnimationUsingPath();
                x.PathGeometry = pg;
                x.Source = PathAnimationSource.X;
                x.Duration = TimeSpan.FromMilliseconds(_TOTAL_MILLISEC);
                x.BeginTime = TimeSpan.FromMilliseconds(0);

                DoubleAnimationUsingPath y = new DoubleAnimationUsingPath();
                y.PathGeometry = pg;
                y.Source = PathAnimationSource.Y;
                y.Duration = TimeSpan.FromMilliseconds(_TOTAL_MILLISEC);
                y.BeginTime = TimeSpan.FromMilliseconds(0);

                DoubleAnimationUsingKeyFrames opacity = new DoubleAnimationUsingKeyFrames();
                opacity.Duration = TimeSpan.FromMilliseconds(_TOTAL_MILLISEC);
                opacity.BeginTime = TimeSpan.FromMilliseconds(0);
                LinearDoubleKeyFrame e0 = new LinearDoubleKeyFrame(0.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)));
                LinearDoubleKeyFrame e1a = new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((_TOTAL_MILLISEC / 4) - 10)));
                LinearDoubleKeyFrame e1 = new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(_TOTAL_MILLISEC / 4)));
                LinearDoubleKeyFrame e1b = new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((_TOTAL_MILLISEC / 4) + 10)));
                LinearDoubleKeyFrame e2 = new LinearDoubleKeyFrame(0.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(_TOTAL_MILLISEC / 2)));
                LinearDoubleKeyFrame e3 = new LinearDoubleKeyFrame(0.1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((_TOTAL_MILLISEC * 3) / 4)));
                LinearDoubleKeyFrame e4 = new LinearDoubleKeyFrame(0.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(_TOTAL_MILLISEC)));
                opacity.KeyFrames.Add(e0);
                opacity.KeyFrames.Add(e1a);
                opacity.KeyFrames.Add(e1);
                opacity.KeyFrames.Add(e1b);
                opacity.KeyFrames.Add(e2);
                opacity.KeyFrames.Add(e3);
                opacity.KeyFrames.Add(e4);

                DoubleAnimationUsingKeyFrames sizex = new DoubleAnimationUsingKeyFrames();
                sizex.Duration = TimeSpan.FromMilliseconds(_TOTAL_MILLISEC);
                sizex.BeginTime = TimeSpan.FromMilliseconds(0);
                LinearDoubleKeyFrame x0 = new LinearDoubleKeyFrame(.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)));
                LinearDoubleKeyFrame x1a = new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((_TOTAL_MILLISEC / 4) - 10)));
                LinearDoubleKeyFrame x1 = new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(_TOTAL_MILLISEC / 4)));
                LinearDoubleKeyFrame x1b = new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((_TOTAL_MILLISEC / 4) + 10)));
                LinearDoubleKeyFrame x2 = new LinearDoubleKeyFrame(.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(_TOTAL_MILLISEC / 2)));
                LinearDoubleKeyFrame x3 = new LinearDoubleKeyFrame(.5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((_TOTAL_MILLISEC * 3) / 4)));
                LinearDoubleKeyFrame x4 = new LinearDoubleKeyFrame(.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(_TOTAL_MILLISEC)));
                sizex.KeyFrames.Add(x0);
                sizex.KeyFrames.Add(x1a);
                sizex.KeyFrames.Add(x1);
                sizex.KeyFrames.Add(x1b);
                sizex.KeyFrames.Add(x2);
                sizex.KeyFrames.Add(x3);
                sizex.KeyFrames.Add(x4);

                DoubleAnimationUsingKeyFrames sizey = new DoubleAnimationUsingKeyFrames();
                sizey.Duration = TimeSpan.FromMilliseconds(_TOTAL_MILLISEC);
                sizey.BeginTime = TimeSpan.FromMilliseconds(0);
                LinearDoubleKeyFrame y0 = new LinearDoubleKeyFrame(.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0)));
                LinearDoubleKeyFrame y1a = new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((_TOTAL_MILLISEC / 4) - 10)));
                LinearDoubleKeyFrame y1 = new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(_TOTAL_MILLISEC / 4)));
                LinearDoubleKeyFrame y1b = new LinearDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((_TOTAL_MILLISEC / 4) + 10)));
                LinearDoubleKeyFrame y2 = new LinearDoubleKeyFrame(.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(_TOTAL_MILLISEC / 2)));
                LinearDoubleKeyFrame y3 = new LinearDoubleKeyFrame(.5, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((_TOTAL_MILLISEC * 3) / 4)));
                LinearDoubleKeyFrame y4 = new LinearDoubleKeyFrame(.75, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(_TOTAL_MILLISEC)));
                sizey.KeyFrames.Add(y0);
                sizey.KeyFrames.Add(y1a);
                sizey.KeyFrames.Add(y1);
                sizey.KeyFrames.Add(y1b);
                sizey.KeyFrames.Add(y2);
                sizey.KeyFrames.Add(y3);
                sizey.KeyFrames.Add(y4);

                AnimationClock cx = x.CreateClock();
                AnimationClock cy = y.CreateClock();
                AnimationClock co = opacity.CreateClock();
                AnimationClock csx = sizex.CreateClock();
                AnimationClock csy = sizey.CreateClock();
                tt.ApplyAnimationClock(TranslateTransform.XProperty, cx);
                tt.ApplyAnimationClock(TranslateTransform.YProperty, cy);
                e.ApplyAnimationClock(FrameworkElement.OpacityProperty, co);
                st.ApplyAnimationClock(ScaleTransform.ScaleXProperty, csx);
                st.ApplyAnimationClock(ScaleTransform.ScaleYProperty, csy);
                cx.Controller.Begin();
                cy.Controller.Begin();
                co.Controller.Begin();
                csx.Controller.Begin();
                csy.Controller.Begin();
                cx.Controller.Pause();
                cy.Controller.Pause();
                co.Controller.Pause();
                csx.Controller.Pause();
                csy.Controller.Pause();
                cx.Controller.Seek(TimeSpan.FromMilliseconds(i * (_TOTAL_MILLISEC / (this.VisualChildrenCount - 1))), TimeSeekOrigin.BeginTime);
                cy.Controller.Seek(TimeSpan.FromMilliseconds(i * (_TOTAL_MILLISEC / (this.VisualChildrenCount - 1))), TimeSeekOrigin.BeginTime);
                co.Controller.Seek(TimeSpan.FromMilliseconds(i * (_TOTAL_MILLISEC / (this.VisualChildrenCount - 1))), TimeSeekOrigin.BeginTime);
                csx.Controller.Seek(TimeSpan.FromMilliseconds(i * (_TOTAL_MILLISEC / (this.VisualChildrenCount - 1))), TimeSeekOrigin.BeginTime);
                csy.Controller.Seek(TimeSpan.FromMilliseconds(i * (_TOTAL_MILLISEC / (this.VisualChildrenCount - 1))), TimeSeekOrigin.BeginTime);

                _list.Add(new AnimationBundle(cx, cy, co, csx, csy, e));
            }
            InvalidateVisual();
        }
        public void ClearElements()
        {
            this.Children.Clear();
            this.Children.Add(_centerText);
        }

        private void OnElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_list != null)
            {
                AnimationBundle b = _list.FirstOrDefault(z => z.GetElement() == sender);
                if (ObjectSelected != null && this.IsSelectable && b != null)
                {
                    if (b.IsCentered)
                        ObjectSelected(sender, new EventArgs());
                }
            }
        }
        private void Scrub(int dx)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                bool changedDiff = false;
                foreach (AnimationBundle animationBundle in _list)
                {
                    TimeSpan currentTime = animationBundle.CurrentTime;
                    TimeSpan next = TimeSpan.FromMilliseconds(currentTime.TotalMilliseconds + dx);
                    while (next.TotalMilliseconds < 0)
                        next = next.Add(animationBundle.Duration);

                    while (next.TotalMilliseconds > animationBundle.Duration.TotalMilliseconds)
                        next = next.Subtract(animationBundle.Duration);

                    animationBundle.Seek(next);
                    int diff = Math.Abs((int)next.TotalMilliseconds - _TOTAL_MILLISEC / 4);
                    bool on = animationBundle.IsCentered;
                    animationBundle.IsCentered = diff < 5;
                    changedDiff |= (on != animationBundle.IsCentered);

                    if (ObjectCentered != null && changedDiff && animationBundle.IsCentered)
                        ObjectCentered(animationBundle.GetElement(), null);

                    else if (animationBundle.IsCentered && this.IsSelectable)
                        animationBundle.GetElement().Cursor = Cursors.Hand;
                    else
                        animationBundle.GetElement().Cursor = Cursors.Arrow;
                }
            }));
        }
        public void SeekToNearest()
        { 
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                int centerTime = _TOTAL_MILLISEC / 4;
                int min = int.MaxValue;

                foreach (AnimationBundle b in _list)
                {
                    int s = (int)b.CurrentTime.TotalMilliseconds - centerTime;
                    if (Math.Abs(s) < Math.Abs(min))
                        min = s;
                }
                //Seek to this bundle
                if (min != int.MaxValue)
                    Scrub(-1 * min);
            }), DispatcherPriority.ApplicationIdle);
        }
        public void SeekNext()
        {
            int dx = (int)(_TOTAL_MILLISEC / (double)_list.Count);
            Scrub(dx);
        }
        public void SeekPrevious()
        {
            int dx = (int)(_TOTAL_MILLISEC / (double)_list.Count);
            Scrub(-1 * dx);
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            _mouseDownPoint = Mouse.GetPosition(this);
        }
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);
            if (_mouseDownPoint.X != 0)
            {
                Point p = Mouse.GetPosition(this);
                Vector v = Point.Subtract(p, _mouseDownPoint);
                Vector unitX = new Vector(-1, 0);
                int dx = (int)Vector.Multiply(v, unitX) * this.AnimationVelocity;
                Scrub(dx);
                _mouseDownPoint = p;
            }
        }
        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (_mouseDownPoint.X != 0)
            {
                if (this.SeekToNearestBehavior)
                    SeekToNearest();
            }
            _mouseDownPoint = new Point(0, 0);
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (_mouseDownPoint.X != 0)
            {
                if (this.SeekToNearestBehavior)
                    SeekToNearest();
            }
            _mouseDownPoint = new Point(0, 0);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InitializeElements();
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            for (int i = 0; i < this.VisualChildrenCount; i++)
            {
                FrameworkElement e = this.GetVisualChild(i) as FrameworkElement;
                e.Measure(this.RenderSize);
            }

            return base.MeasureOverride(availableSize);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            for (int i = 0; i < this.VisualChildrenCount; i++)
            {
                FrameworkElement e = this.GetVisualChild(i) as FrameworkElement;
                e.Arrange(new Rect(this.RenderSize));
            }

            return base.ArrangeOverride(finalSize);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            //To get mouse events over the whole control area
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(this.RenderSize));
        }
    }
}
