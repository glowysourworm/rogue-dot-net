using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Data;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;
using System.Collections;
using System.Threading;
using System.ComponentModel;
using Rogue.NET.Common;
using Rogue.NET.Model.Media;
using Rogue.NET.Model;
using Rogue.NET.Model.Scenario;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using Rogue.NET.Common.Views;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Model.Events;
using Microsoft.Practices.Prism.PubSubEvents;

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

                model.Level.RenderTransform = xform;
                model.Player.RenderTransform = xform;

                this.Children.Add(model.Level);
                this.Children.Add(model.Player);

                model.Player.PropertyChanged += (obj, ev) =>
                {
                    if (ev.PropertyName == "Location")
                        OnPlayerLocationChanged();
                };

                model.TargetedEnemies.CollectionChanged += (obj, ev) =>
                {
                    var offset = xform.Transform(new Point(0, 0));
                    var offsetVector = new Vector(offset.X, offset.Y);
                    var points = model.TargetedEnemies.Select(x => Point.Add(new Point(x.Margin.Left, x.Margin.Top), offsetVector)).ToArray();
                    PlayTargetAnimation(points);
                };

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
            var origin = data.Player.RenderTransform.Transform(new Point(0, 0));

            _translateXform.X += midpt.X - (data.Player.Margin.Left + origin.X);
            _translateXform.Y += midpt.Y - (data.Player.Margin.Top + origin.Y);
        }

        private Point GetPlayerLocation()
        {
            var data = this.DataContext as LevelData;
            if (data != null)
                return Point.Add(data.Player.RenderTransform.Transform(new Point(0, 0)), new Vector(data.Player.Margin.Left, data.Player.Margin.Top));

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

        #region Roaming Light Source
        CellPoint _roamingLightSourcePoint = null;
        AnimationGroup _roamingLightSource = null;
        public void InitializeRoamingLightSource()
        {
            //Level lvl = this.DataContext as Level;
            //if (lvl == null)
            //    return;

            //if (_roamingLightSourcePoint == null)
            //    _roamingLightSourcePoint = new CellPoint(lvl.Player1.Location);

            //Point p = _layoutXform.Transform(Rogue2GlobalHelper.Cell2UI(_roamingLightSourcePoint.Column, _roamingLightSourcePoint.Row));
            //p.X += Config.CELLWIDTH_CONST / 2;
            //p.Y += Config.CELLHEIGHT_CONST / 2;

            ////Fuzzy Aura Brush
            //RadialGradientBrush r = new RadialGradientBrush();
            //r.GradientOrigin = new Point(0.5, 0.5);
            //r.GradientStops = new GradientStopCollection(new GradientStop[]{
            //new GradientStop(lvl.Player1.SmileyBodyColor, 0),
            //new GradientStop(Colors.Transparent, 0.8)});

            //_roamingLightSource = Rogue2AnimationGenerator.CreateAuraModulation(
            //    p, Brushes.White, r, 0.3, 1, 0.5, new Size(8, 8), new Size(7, 7), 1500, int.MaxValue, true);

            //StartTimedGraphicalEvent(_roamingLightSource);
        }
        public void MoveRoamingLightSource(Compass c)
        {
            //Level lvl = this.DataContext as Level;
            //if (lvl == null)
            //    return;

            //CellPoint next = CharacterMovementEngine.AdvanceToCell(_roamingLightSourcePoint, c);
            //if (!CharacterMovementEngine.IsPathToAdjacentCellBlocked(_roamingLightSourcePoint, next, _lvl, true, false))
            //{
            //    //Advance light source
            //    _roamingLightSourcePoint = next;

            //    //Process Visibility
            //    List<Cell> list1 = CharacterMovementEngine.GetLogicallyVisibleCells(lvl.Layout, _roamingLightSourcePoint, lvl.Player1.LightRadius);
            //    List<Cell> list2 = CharacterMovementEngine.GetLogicallyVisibleCells(lvl.Layout, lvl.Player1.Location, lvl.Player1.LightRadius);
            //    Cell[] list = list1.Union(list2).ToArray();
            //    //Contents Visibility
            //    lvl.Content.ProcessVisibility(list);

            //    //Layout Visibility
            //    foreach (Cell ce in lvl.Layout.Grid.GetVisibleCells())
            //        ce.IsPhysicallyVisible = false;

            //    foreach (Cell ce in list)
            //    {
            //        //SHOULDN'T BE NULL!!
            //        if (ce == null)
            //            continue;

            //        ce.IsPhysicallyVisible = true;
            //        ce.IsExplored = true;

            //        //No longer have to highlight revealed cells
            //        ce.IsRevealed = false;
            //    }

            //    OnPlayerLocationChanged(next);

            //    //Move Animated Bubble
            //    DestroyRoamingLightSource();
            //    InitializeRoamingLightSource();
            //}
        }
        private void DestroyRoamingLightSource()
        {
            if (_roamingLightSource != null)
            {
                Graphic[] graphics = _roamingLightSource.GetGraphics();
                foreach (Graphic g in graphics)
                    this.Children.Remove(g);
                _roamingLightSource.Stop();
                _roamingLightSource.CleanUp();
                _roamingLightSource = null;
            }
        }
        public void EndRoamingLightSourceMode()
        {
            DestroyRoamingLightSource();
            _roamingLightSourcePoint = null;
        }
        #endregion

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

                Animation a1 = AnimationGenerator.GenerateProjectilePath(new Point[] { nw, r.TopLeft }, s, s2, Brushes.Magenta, Brushes.Magenta, 1, 0, 0, 0, 1, 300, 1, int.MaxValue, true, false);
                Animation a2 = AnimationGenerator.GenerateProjectilePath(new Point[] { ne, r.TopRight }, s, s2, Brushes.Magenta, Brushes.Magenta, 1, 0, 0, 0, 1, 300, 1, int.MaxValue, true, false);
                Animation a3 = AnimationGenerator.GenerateProjectilePath(new Point[] { se, r.BottomRight }, s, s2, Brushes.Magenta, Brushes.Magenta, 1, 0, 0, 0, 1, 300, 1, int.MaxValue, true, false);
                Animation a4 = AnimationGenerator.GenerateProjectilePath(new Point[] { sw, r.BottomLeft }, s, s2, Brushes.Magenta, Brushes.Magenta, 1, 0, 0, 0, 1, 300, 1, int.MaxValue, true, false);
                Animation a5 = AnimationGenerator.GenerateTimedFigure(new RectangleGeometry(r), Brushes.LimeGreen, Brushes.Magenta, 1, 0, 1, 1, 300, int.MaxValue, true);
                AnimationGroup group = new AnimationGroup(new Animation[] { a1, a2, a3, a4, a5 });
                _targetAnimationGroupList.Add(group);
                StartTimedGraphicalEvent(group);
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
        AnimationStartEvent _animationData = null;

        /// <summary>
        /// Creates IRogue2TimedGraphic set for each of the animation templates and returns the
        /// last one as a handle
        /// </summary>
        public ITimedGraphic PlayAnimationSeries(AnimationStartEvent e)
        {
            _animationData = e;

            var levelData = this.DataContext as LevelData;
            var xform = levelData.Player.RenderTransform;
            var offset = xform.Transform(new Point(0,0));
            var offsetVector = new Vector(offset.X + ScenarioConfiguration.CELLWIDTH / 2, offset.Y + ScenarioConfiguration.CELLHEIGHT / 2);

            //Create animations
            foreach (AnimationTemplate t in e.Animations)
                _animationList.Add(
                    AnimationGenerator.CreateAnimation(
                    t, 
                    new Rect(this.RenderSize), 
                    Point.Add(new Point(e.Source.Margin.Left, e.Source.Margin.Top), offsetVector),
                    e.Targets.Select(x => Point.Add(new Point(x.Margin.Left, x.Margin.Top), offsetVector)).ToArray()));

            //Start the first and return a handle to the last one
            ITimedGraphic g = _animationList[_animationList.Count - 1];
            StartNextAnimation();
            return g;
        }
        private void StartNextAnimation()
        {
            if (_animationList.Count <= 0)
            {
                _eventAggregator.GetEvent<AnimationCompletedEvent>().Publish(new AnimationCompletedEvent()
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

