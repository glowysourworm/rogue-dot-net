using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel.Composition;
using System.Linq;

using Prism.Events;

using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Model.Events;
using Rogue.NET.Core.Media;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Media.Interface;
using Rogue.NET.Core.Graveyard;
using Rogue.NET.Core.Model;

namespace Rogue.NET.Scenario.Content.Views
{
    [Export]
    public partial class LevelCanvas : UserControl
    {
        readonly IAnimationGenerator _animationGenerator;
        readonly IEventAggregator _eventAggregator;

        TranslateTransform _translateXform = new TranslateTransform(0,0);
        ScaleTransform _scaleXform = new ScaleTransform(1,1);

        bool _mouseDownWithControl = false;
        Point _mouseDownWithControlPoint = new Point();

        const int SCREEN_BUFFER = 120;

        [ImportingConstructor]
        public LevelCanvas(
            LevelCanvasViewModel viewModel, 
            IEventAggregator eventAggregator, 
            IAnimationGenerator animationGenerator)
        {
            _animationGenerator = animationGenerator;
            _eventAggregator = eventAggregator;

            this.DataContext = viewModel;

            InitializeComponent();

            // allows capturing of mouse events
            this.Background = Brushes.Transparent;

            var transform = new TransformGroup();
            transform.Children.Add(_scaleXform);
            transform.Children.Add(_translateXform);

            this.RenderTransform = transform;

            Initialize();
        }
         
        private void LevelCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            // TODO
            //var view = this.DataContext as LevelData;
            //if (view != null)
            //{
            //    CenterOnPlayer();
            //}
        }
        private void LevelCanvas_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;

            // TODO
            //var model = e.NewValue as LevelData;
            //if (model != null)
            //{
            //    this.Children.Clear();

            //    var xform = new TransformGroup();
            //    xform.Children.Add(_scaleXform);
            //    xform.Children.Add(_translateXform);

            //    //model.Level.RenderTransform = xform;
            //    //model.Player.RenderTransform = xform;

            //    //this.Children.Add(model.Level);
            //    //this.Children.Add(model.Player);

            //    //model.Player.PropertyChanged += (obj, ev) =>
            //    //{
            //    //    if (ev.PropertyName == "Location")
            //    //        OnPlayerLocationChanged();
            //    //};

            //    //model.TargetedEnemies.CollectionChanged += (obj, ev) =>
            //    //{
            //    //    var offset = xform.Transform(new Point(0, 0));
            //    //    var offsetVector = new Vector(offset.X, offset.Y);
            //    //    var points = model.TargetedEnemies.Select(x => Point.Add(new Point(x.Margin.Left, x.Margin.Top), offsetVector)).ToArray();
            //    //    PlayTargetAnimation(points);
            //    //};

            //    CenterOnPlayer();
            //    InvalidateVisual();
            //}
        }

        private void Initialize()
        {
            // subscribe to events TODO
            //_eventAggregator.GetEvent<UserCommandEvent>().Subscribe((e) =>
            //{
            //    // recenter display if player is off screen
            //    Point p = GetPlayerLocation();
            //    Rect r = new Rect(this.RenderSize);

            //    if (!r.Contains(p) && !(p.X == 0 && p.Y == 0))
            //        CenterOnPlayer();

            //    StopTargetAnimation();
            //});

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

            // TODO
            // var data  = this.DataContext as LevelData;
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
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (Keyboard.Modifiers == ModifierKeys.Control)
                this.Cursor = Cursors.ScrollAll;

            else
                this.Cursor = Cursors.Arrow;

            if (Keyboard.Modifiers == ModifierKeys.Control && _mouseDownWithControl)
            {
                _translateXform.X += (int)(Mouse.GetPosition(this).X - _mouseDownWithControlPoint.X);
                _translateXform.Y += (int)(Mouse.GetPosition(this).Y - _mouseDownWithControlPoint.Y);
                _mouseDownWithControlPoint = Mouse.GetPosition(this);
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (Keyboard.Modifiers == ModifierKeys.Control && e.LeftButton == MouseButtonState.Pressed)
            {
                _mouseDownWithControl = true;
                _mouseDownWithControlPoint = Mouse.GetPosition(this);
            }
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            this.Cursor = Cursors.Arrow;
            _mouseDownWithControl = false;
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);
            _mouseDownWithControl = false;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            var scale = (e.Delta > 0) ? 1.05 : 0.95;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                _scaleXform.ScaleX *= scale;
                _scaleXform.ScaleY *= scale;

                // TODO
                //_translateXform.X += (scale > 1 ? 1 : -1) * (scale * 0.05 * this.RenderSize.Width);
                //_translateXform.Y += (scale > 1 ? 1 : -1) * (scale * 0.05 * this.RenderSize.Height);
            }
        }
    }
}

