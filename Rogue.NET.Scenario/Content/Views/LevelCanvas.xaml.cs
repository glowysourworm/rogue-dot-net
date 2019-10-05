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
using Rogue.NET.Common.Extension;
using System;
using Rogue.NET.Core.Processing.Service.Interface;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class LevelCanvas : UserControl
    {
        readonly ILevelCanvasViewModel _viewModel;

        TranslateTransform _translateXform = new TranslateTransform(0,0);
        ScaleTransform _scaleXform = new ScaleTransform(1,1);

        const int SHIFT_AMOUNT = 60;

        [ImportingConstructor]
        public LevelCanvas(
            ILevelCanvasViewModel viewModel,
            IModelService modelService,             // TODO: Find a way to remove the model service
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
            this.RevealedCanvas.RenderTransform = transform;
            this.DoorCanvas.RenderTransform = transform;
            this.LightRadiiItemsControl.RenderTransform = transform;
            this.AuraItemsControl.RenderTransform = transform;
            this.DoodadItemsControl.RenderTransform = transform;
            this.ItemItemsControl.RenderTransform = transform;
            this.CharacterItemsControl.RenderTransform = transform;
            this.PlayerCanvas.RenderTransform = transform;
            this.AnimationItemsControl.RenderTransform = transform;

            this.Loaded += (sender, args) =>
            {
                CenterOnLocation(_viewModel.Player.Location);
            };

            // subscribe to event to center screen when level loaded
            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                // Zoom() -> CenterOnLocation( player )
                Zoom(modelService.ZoomFactor);
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

            eventAggregator.GetEvent<ZoomEvent>().Subscribe(eventData =>
            {
                Zoom(eventData.NewZoomFactor); 
            });
        }
        public void CenterOnLocation(Point location)
        {
            if (!this.IsLoaded)
                return;

            // Make measurements relative to the LevelView
            var window = Window.GetWindow(this);

            // Add offset to bounds and player location
            var bounds = new Rect(window.RenderSize);
            var midpt = new Point(bounds.Width / 2.0D, bounds.Height / 2.0D);

            // Transform LevelCanvas -> Window
            var adjustment = this.PlayerCanvas.TransformToAncestor(window).Transform(location);

            _translateXform.X += (midpt.X - (adjustment.X));
            _translateXform.Y += (midpt.Y - (adjustment.Y));
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

        public void Zoom(double zoomFactor)
        {
            _scaleXform.ScaleX = zoomFactor.Clip(1.0, 3.0);
            _scaleXform.ScaleY = zoomFactor.Clip(1.0, 3.0);

            CenterOnLocation(_viewModel.Player.Location);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (_viewModel != null)
                CenterOnLocation(_viewModel.Player.Location);
        }
    }
}

