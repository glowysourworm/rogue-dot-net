using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Model.Events;
using Prism.Events;
using Rogue.NET.Core.Media;
using Rogue.NET.Core.Graveyard;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using Rogue.NET.Core.Model;

namespace Rogue.NET.Scenario.Views
{
    public class LevelCanvas : Canvas
    {
        IEventAggregator _eventAggregator;
        bool _eventsHooked;

        List<AnimationGroup> _targetAnimationGroupList = new List<AnimationGroup>();
        TranslateTransform _translateXform = new TranslateTransform(0,0);
        ScaleTransform _scaleXform = new ScaleTransform(1,1);

        bool _mouseDownWithControl = false;
        Point _mouseDownWithControlPoint = new Point();

        const int SCREEN_BUFFER = 120;

        public LevelCanvas()
        {
            // allows capturing of mouse events
            this.Background = Brushes.Transparent;

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(LevelCanvas_DataContextChanged);
            this.Loaded += new RoutedEventHandler(LevelCanvas_Loaded);
        }
        private void LevelCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            var view = this.DataContext as LevelData;
            if (view != null)
            {
                CenterOnPlayer();
            }
        }
        private void LevelCanvas_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            var model = e.NewValue as LevelData;
            if (model != null)
            {
                this.Children.Clear();

                var xform = new TransformGroup();
                xform.Children.Add(_scaleXform);
                xform.Children.Add(_translateXform);

                //model.Level.RenderTransform = xform;
                //model.Player.RenderTransform = xform;

                //this.Children.Add(model.Level);
                //this.Children.Add(model.Player);

                //model.Player.PropertyChanged += (obj, ev) =>
                //{
                //    if (ev.PropertyName == "Location")
                //        OnPlayerLocationChanged();
                //};

                //model.TargetedEnemies.CollectionChanged += (obj, ev) =>
                //{
                //    var offset = xform.Transform(new Point(0, 0));
                //    var offsetVector = new Vector(offset.X, offset.Y);
                //    var points = model.TargetedEnemies.Select(x => Point.Add(new Point(x.Margin.Left, x.Margin.Top), offsetVector)).ToArray();
                //    PlayTargetAnimation(points);
                //};

                CenterOnPlayer();
                InvalidateVisual();
            }
        }

        public void InitializeEvents(IEventAggregator eventAggregator)
        {
            if (_eventsHooked)
                return;

            _eventsHooked = true;
            _eventAggregator = eventAggregator;

            // subscribe to events
            eventAggregator.GetEvent<UserCommandEvent>().Subscribe((e) =>
            {
                // recenter display if player is off screen
                Point p = GetPlayerLocation();
                Rect r = new Rect(this.RenderSize);

                if (!r.Contains(p) && !(p.X == 0 && p.Y == 0))
                    CenterOnPlayer();

                StopTargetAnimation();
            });

            eventAggregator.GetEvent<AnimationStartEvent>().Subscribe((e) =>
            {
                PlayAnimationSeries(e);
            });
        }

        public void OnPlayerLocationChanged()
        {
            Point p = GetPlayerLocation();
            Rect r = new Rect(this.RenderSize);

            if (!r.Contains(p))
                CenterOnPlayer();

            else if ((r.Bottom - p.Y) < SCREEN_BUFFER)
                _translateXform.Y -= SCREEN_BUFFER - (r.Bottom - p.Y);

            else if (p.Y < SCREEN_BUFFER)
                _translateXform.Y += SCREEN_BUFFER - p.Y;

            else if ((r.Right - p.X) < SCREEN_BUFFER)
                _translateXform.X -= SCREEN_BUFFER - (r.Right - p.X);

            else if (p.X < SCREEN_BUFFER)
                _translateXform.X += SCREEN_BUFFER - p.X;
        }
        public void ShiftDisplay(Compass direction, bool largeShift)
        {
            switch (direction)
            {
                case Compass.N:
                    _translateXform.Y -= largeShift ? 60 : 15;
                    break;
                case Compass.S:
                    _translateXform.Y += largeShift ? 60 : 15;
                    break;
                case Compass.E:
                    _translateXform.X += largeShift ? 60 : 15;
                    break;
                case Compass.W:
                    _translateXform.X -= largeShift ? 60 : 15;
                    break;
            }
        }
        public void CenterOnPlayer()
        {
            Rect r = new Rect(this.RenderSize);
            Point midpt = new Point(r.Width / 2, r.Height / 2);

            var data  = this.DataContext as LevelData;
            //var origin = data.Player.RenderTransform.Transform(new Point(0, 0));

            //_translateXform.X += midpt.X - (data.Player.Margin.Left + origin.X);
            //_translateXform.Y += midpt.Y - (data.Player.Margin.Top + origin.Y);
        }

        private Point GetPlayerLocation()
        {
            //var data = this.DataContext as LevelData;
            //if (data != null)
            //    return Point.Add(data.Player.RenderTransform.Transform(new Point(0, 0)), new Vector(data.Player.Margin.Left, data.Player.Margin.Top));

            return new Point(0,0);
        }

        private void MouseMove(MouseEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
                this.Cursor = Cursors.ScrollAll;

            else
                this.Cursor = Cursors.Arrow;

            if (_mouseDownWithControl)
            {
                _translateXform.X += (int)(Mouse.GetPosition(this).X - _mouseDownWithControlPoint.X);
                _translateXform.Y += (int)(Mouse.GetPosition(this).Y - _mouseDownWithControlPoint.Y);
                _mouseDownWithControlPoint = Mouse.GetPosition(this);
            }
        }
        private void MouseDown(MouseEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.LeftButton == MouseButtonState.Pressed)
            {
                _mouseDownWithControl = true;
                _mouseDownWithControlPoint = Mouse.GetPosition(this);
            }
        }
        private void MouseUp(MouseEventArgs e)
        {
            _mouseDownWithControl = false;
        }
        private void MouseLeave(MouseEventArgs e)
        {
            this.Cursor = Cursors.Arrow;
            _mouseDownWithControl = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            MouseMove(e);
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            MouseDown(e);
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            MouseLeave(e);
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            MouseUp(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            // WONT WORK UNTIL PLAYER IS A CHILD OF LEVEL
            
            //double scale = (e.Delta > 0) ? 1.05 : 0.95;
            //if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            //{
            //    _scaleXform.ScaleX *= scale;
            //    _scaleXform.ScaleY *= scale;

            //    //_translateXform.X += (scale > 1 ? 1 : -1) * (scale * 0.1 * this.RenderSize.Width);
            //    //_translateXform.Y += (scale > 1 ? 1 : -1) * (scale * 0.1 * this.RenderSize.Height);
            //}
        }

        #region Animations
        public void PlayTargetAnimation(Point[] pts)
        {
            if (_targetAnimationGroupList.Count != 0)
                StopTargetAnimation();

            for (int i = 0; i < pts.Length;i++)
            {
                Point p = pts[i];
                int len = 20;
                Rect r = new Rect(p, new Size(10, 15));
                Size s2 = new Size(1, 8);
                Size s = new Size(1, 1);

                Point nw = new Point(p.X - len, p.Y - len);
                Point ne = new Point(r.TopRight.X + len, r.TopRight.Y - len);
                Point se = new Point(r.BottomRight.X + len, r.BottomRight.Y + len);
                Point sw = new Point(r.BottomLeft.X - len, r.BottomLeft.Y + len);

                //Animation a1 = AnimationGenerator.GenerateProjectilePath(new Point[] { nw, r.TopLeft }, s, s2, Brushes.Magenta, Brushes.Magenta, 1, 0, 0, 0, 1, 300, 1, int.MaxValue, true, false);
                //Animation a2 = AnimationGenerator.GenerateProjectilePath(new Point[] { ne, r.TopRight }, s, s2, Brushes.Magenta, Brushes.Magenta, 1, 0, 0, 0, 1, 300, 1, int.MaxValue, true, false);
                //Animation a3 = AnimationGenerator.GenerateProjectilePath(new Point[] { se, r.BottomRight }, s, s2, Brushes.Magenta, Brushes.Magenta, 1, 0, 0, 0, 1, 300, 1, int.MaxValue, true, false);
                //Animation a4 = AnimationGenerator.GenerateProjectilePath(new Point[] { sw, r.BottomLeft }, s, s2, Brushes.Magenta, Brushes.Magenta, 1, 0, 0, 0, 1, 300, 1, int.MaxValue, true, false);
                //Animation a5 = AnimationGenerator.GenerateTimedFigure(new RectangleGeometry(r), Brushes.LimeGreen, Brushes.Magenta, 1, 0, 1, 1, 300, int.MaxValue, true);
                //AnimationGroup group = new AnimationGroup(new Animation[] { a1, a2, a3, a4, a5 });
                //_targetAnimationGroupList.Add(group);
                //StartTimedGraphicalEvent(group);
            }
        }
        public void StopTargetAnimation()
        {
            if (_targetAnimationGroupList.Count != 0)
            {
                foreach (AnimationGroup group in _targetAnimationGroupList)
                {
                    Graphic[] graphics = group.GetGraphics();
                    foreach (Graphic g in graphics)
                        this.Children.Remove(g);
                    group.Stop();
                    group.CleanUp();
                }
                _targetAnimationGroupList.Clear();
            }
        }

        List<ITimedGraphic> _animationList = new List<ITimedGraphic>();
        AnimationStartEventArgs _animationData = null;

        /// <summary>
        /// Creates IRogue2TimedGraphic set for each of the animation templates and returns the
        /// last one as a handle
        /// </summary>
        public ITimedGraphic PlayAnimationSeries(AnimationStartEventArgs e)
        {
            _animationData = e;

            //var levelData = this.DataContext as LevelData;
            //var xform = levelData.Player.RenderTransform;
            //var offset = xform.Transform(new Point(0,0));
            //var offsetVector = new Vector(offset.X + ModelConstants.CELLWIDTH / 2, offset.Y + ModelConstants.CELLHEIGHT / 2);

            ////Create animations
            //foreach (AnimationTemplate t in e.Animations)
            //    _animationList.Add(
            //        AnimationGenerator.CreateAnimation(
            //        t, 
            //        new Rect(this.RenderSize), 
            //        Point.Add(new Point(e.Source.Margin.Left, e.Source.Margin.Top), offsetVector),
            //        e.Targets.Select(x => Point.Add(new Point(x.Margin.Left, x.Margin.Top), offsetVector)).ToArray()));

            //Start the first and return a handle to the last one
            ITimedGraphic g = _animationList[_animationList.Count - 1];
            StartNextAnimation();
            return g;
        }
        private void StartNextAnimation()
        {
            if (_animationList.Count <= 0)
            {
                _eventAggregator.GetEvent<AnimationCompletedEvent>().Publish(new AnimationCompletedEventArgs()
                {
                    Alteration = _animationData.Alteration,
                    ReturnAction = _animationData.ReturnAction,
                    Source = _animationData.Source,
                    Targets = _animationData.Targets
                });
                return;
            }

            ITimedGraphic t = _animationList[0];
            _animationList.RemoveAt(0);
            StartTimedGraphicalEvent(t);
        }
        private void StartTimedGraphicalEvent(ITimedGraphic e)
        {
            Graphic[] graphics = e.GetGraphics();
            foreach (Graphic g in graphics)
            {
                Canvas.SetZIndex(g, 100);
                this.Children.Add(g);
            }
            e.TimeElapsed += new TimerElapsedHandler(OnTimedEventElapsed);
            e.Start();
            UpdateLayout();
        }
        private void OnTimedEventElapsed(ITimedGraphic sender)
        {
            Graphic[] graphics = sender.GetGraphics();
            foreach (Graphic g in graphics)
                this.Children.Remove(g);
            sender.TimeElapsed -= new TimerElapsedHandler(OnTimedEventElapsed);
            sender.CleanUp();

            //Will play next animation if any left on the list
            StartNextAnimation();
        }
        #endregion
    }
}

