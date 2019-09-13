using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel.Composition;

using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Processing.Event.Level;
using Rogue.NET.Scenario.Processing.Event.Content;
using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas.Inteface;
using Rogue.NET.Scenario.Content.ViewModel.Content;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class LevelCanvas : UserControl
    {
        readonly ILevelCanvasViewModel _viewModel;

        TranslateTransform _translateXform = new TranslateTransform(0,0);
        ScaleTransform _scaleXform = new ScaleTransform(1,1);

        bool _mouseDownWithControl = false;
        Point _mouseDownWithControlPoint = new Point();

        const int SCREEN_BUFFER = 300;
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
            ILevelCanvasViewModel viewModel,
            IRogueEventAggregator eventAggregator)
        {
            _viewModel = viewModel;

            this.DataContext = viewModel;

            InitializeComponent();

            // allows capturing of mouse events
            this.Background = Brushes.Transparent;

            var transform = new TransformGroup();
            transform.Children.Add(_scaleXform);
            transform.Children.Add(_translateXform);

            this.WallCanvas.RenderTransform = transform;
            this.DoorCanvas.RenderTransform = transform;
            this.RevealedCanvas.RenderTransform = transform;
            this.LightRadiiItemsControl.RenderTransform = transform;
            this.AuraItemsControl.RenderTransform = transform;
            this.DoodadItemsControl.RenderTransform = transform;
            this.ItemItemsControl.RenderTransform = transform;
            this.CharacterItemsControl.RenderTransform = transform;
            this.AnimationItemsControl.RenderTransform = transform;
            this.PlayerCanvas.RenderTransform = transform;

            this.Loaded += (sender, args) =>
            {
                CenterOnLocation(_viewModel.Player.Location);
            };

            // subscribe to event to center screen when level loaded
            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                CenterOnLocation(_viewModel.Player.Location);
            });
            
            // subscribe to event to update RenderTransform on player move
            eventAggregator.GetEvent<LevelEvent>().Subscribe(update =>
            {
                if (update.LevelUpdateType == LevelEventType.PlayerLocation)
                    CenterOnLocation(_viewModel.Player.Location);

            });

            eventAggregator.GetEvent<ShiftDisplayEvent>().Subscribe(type =>
            {
                ShiftDisplay(type);
            });
        }
        public void CenterOnLocation(Point location)
        {
            if (!this.IsLoaded)
                return;

            // Make measurements relative to the LevelView
            var window = Window.GetWindow(this);

            // Offset of this control relative to ContentControl parent
            var offsetLocation = this.PointToScreen(location);

            // Add offset to bounds and player location
            var bounds = new Rect(window.RenderSize);
            var midpt = new Point(bounds.Width / 2.0D, bounds.Height / 2.0D);

            _translateXform.X = midpt.X - (offsetLocation.X);
            _translateXform.Y = midpt.Y - (offsetLocation.Y);
        }

        private void ShiftDisplay(ShiftDisplayType type)
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
                    CenterOnLocation(_viewModel.Player.Location);
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

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            var viewModel = this.DataContext as LevelCanvasViewModel;

            if (viewModel != null)
                CenterOnLocation(_viewModel.Player.Location);
        }
    }
}

