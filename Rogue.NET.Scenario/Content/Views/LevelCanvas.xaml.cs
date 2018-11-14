﻿using System.Collections.Generic;
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
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing.Enum;

namespace Rogue.NET.Scenario.Content.Views
{
    [Export]
    public partial class LevelCanvas : UserControl
    {
        TranslateTransform _translateXform = new TranslateTransform(0,0);
        ScaleTransform _scaleXform = new ScaleTransform(1,1);

        bool _mouseDownWithControl = false;
        Point _mouseDownWithControlPoint = new Point();

        const int SCREEN_BUFFER = 120;

        public static readonly DependencyProperty PrimaryTransformProperty =
            DependencyProperty.Register("PrimaryTransform", typeof(Transform), typeof(LevelCanvas));

        public Transform PrimaryTransform
        {
            get { return (Transform)GetValue(PrimaryTransformProperty); }
            set { SetValue(PrimaryTransformProperty, value); }
        }

        [ImportingConstructor]
        public LevelCanvas(
            LevelCanvasViewModel viewModel, 
            IEventAggregator eventAggregator, 
            IAnimationGenerator animationGenerator)
        {
            this.DataContext = viewModel;

            InitializeComponent();

            // allows capturing of mouse events
            this.Background = Brushes.Transparent;

            var transform = new TransformGroup();
            transform.Children.Add(_scaleXform);
            transform.Children.Add(_translateXform);

            this.TheItemsControl.RenderTransform = transform;

            // subscribe to event to update RenderTransform on player move
            eventAggregator.GetEvent<LevelUpdateEvent>().Subscribe(update =>
            {
                if (update.LevelUpdateType == LevelUpdateType.PlayerLocation)
                    OnPlayerLocationChanged(viewModel);
            }, true);
        }

        public void OnPlayerLocationChanged(LevelCanvasViewModel viewModel)
        {
            // Offset of this control relative to ContentControl parent
            var offset = this.TheItemsControl.RenderTransform.Transform(new Point(0, 0));

            // Add offset to bounds and player location
            var location = Point.Add(viewModel.PlayerLocation, new Vector(offset.X, offset.Y));
            var bounds = new Rect((this.Parent as ContentControl).RenderSize);

            // recenter display if player is off screen
            if (!bounds.Contains(location))
            {
                //CenterOnLocation(location);
            }

            else if ((bounds.Bottom - location.Y) < SCREEN_BUFFER)
                _translateXform.Y -= SCREEN_BUFFER - (bounds.Bottom - location.Y);

            else if (location.Y < SCREEN_BUFFER)
                _translateXform.Y += SCREEN_BUFFER - location.Y;

            else if ((bounds.Right - location.X) < SCREEN_BUFFER)
                _translateXform.X -= SCREEN_BUFFER - (bounds.Right - location.X);

            else if (location.X < SCREEN_BUFFER)
                _translateXform.X += SCREEN_BUFFER - location.X;
        }
        public void CenterOnLocation(Point location)
        {
            // Offset of this control relative to ContentControl parent
            var offset = this.TheItemsControl.RenderTransform.Transform(new Point(0, 0));

            var bounds = new Rect((this.Parent as ContentControl).RenderSize);
            var midpt = new Point(bounds.Width / 2.0D, bounds.Height / 2.0D);

            _translateXform.X += midpt.X - (location.X + offset.X);
            _translateXform.Y += midpt.Y - (location.Y + offset.Y);
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

