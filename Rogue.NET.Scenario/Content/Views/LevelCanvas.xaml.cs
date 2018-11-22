using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel.Composition;

using Prism.Events;

using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas;
using Rogue.NET.Core.Media.Interface;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Scenario.Events.Content;

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
        const int SHIFT_AMOUNT = 60;

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

            }, ThreadOption.UIThread, true);

            eventAggregator.GetEvent<ShiftDisplayEvent>().Subscribe(type =>
            {
                ShiftDisplay(type, viewModel);
            }, ThreadOption.UIThread, true);
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
                CenterOnLocation(viewModel.PlayerLocation);
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

        private void ShiftDisplay(ShiftDisplayType type, LevelCanvasViewModel viewModel)
        {
            switch (type)
            {
                case ShiftDisplayType.Left:
                    _translateXform.X -= SHIFT_AMOUNT;
                    break;
                case ShiftDisplayType.Right:
                    _translateXform.X += SHIFT_AMOUNT;
                    break;
                case ShiftDisplayType.Up:
                    _translateXform.Y -= SHIFT_AMOUNT;
                    break;
                case ShiftDisplayType.Down:
                    _translateXform.Y += SHIFT_AMOUNT;
                    break;
                case ShiftDisplayType.CenterOnPlayer:
                    CenterOnLocation(viewModel.PlayerLocation);
                    break;
                default:
                    break;
            }
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
    }
}

