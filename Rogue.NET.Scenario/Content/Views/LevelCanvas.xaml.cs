using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.GameRouter.GameEvent.Backend.Enum;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Level;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas.Inteface;
using Rogue.NET.Scenario.Processing.Event.Content;
using Rogue.NET.Scenario.Processing.Service.Interface;

using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class LevelCanvas : UserControl
    {
        public static readonly DependencyProperty LevelWidthProperty =
            DependencyProperty.Register("LevelWidth", typeof(int), typeof(LevelCanvas));

        public static readonly DependencyProperty LevelHeightProperty =
            DependencyProperty.Register("LevelHeight", typeof(int), typeof(LevelCanvas));

        public int LevelWidth
        {
            get { return (int)GetValue(LevelWidthProperty); }
            set { SetValue(LevelWidthProperty, value); }
        }

        public int LevelHeight
        {
            get { return (int)GetValue(LevelHeightProperty); }
            set { SetValue(LevelHeightProperty, value); }
        }

        readonly IScenarioUIGeometryService _scenarioUIGeometryService;
        readonly IScenarioBitmapSourceFactory _scenarioBitmapSourceFactory;
        readonly ILevelCanvasViewModel _viewModel;
        readonly IModelService _modelService;

        TranslateTransform _translateXform = new TranslateTransform(0, 0);
        ScaleTransform _scaleXform = new ScaleTransform(1, 1);

        const int SHIFT_AMOUNT = 60;
        const int LAYOUT_BITMAP_DPI = 96;
        

        // Grid Location for storing and tracking mouse cursor
        GridLocation _cursorGridLocation = null;

        [ImportingConstructor]
        public LevelCanvas(IScenarioUIGeometryService scenarioUIGeometryService,
                           IScenarioBitmapSourceFactory scenarioBitmapSourceFactory,
                           IScenarioUIService scenarioUIService,
                           ILevelCanvasViewModel viewModel,
                           IModelService modelService,  // TODO: Find a way to remove the model service
                           IRogueEventAggregator eventAggregator)
        {
            _scenarioUIGeometryService = scenarioUIGeometryService;
            _scenarioBitmapSourceFactory = scenarioBitmapSourceFactory;
            _modelService = modelService;
            _viewModel = viewModel;

            this.DataContext = viewModel;

            InitializeComponent();

            // allows capturing of mouse events
            this.Background = Brushes.Transparent;

            var transform = new TransformGroup();
            transform.Children.Add(_scaleXform);
            transform.Children.Add(_translateXform);

            this.LevelContainerBorder.RenderTransform = transform;

            this.Loaded += (sender, args) =>
            {
                CenterOnLocation(_viewModel.Player.Location);
            };

            this.MouseCanvas.MouseMove += (sender, e) =>
            {
                // Calculate location on the level grid
                var gridLocation = _scenarioUIGeometryService.UI2Cell(e.GetPosition(this.MouseCanvas));

                // Show Mouse Locator at calculated position
                if (!gridLocation.Equals(_cursorGridLocation))
                    UpdateCursorLocation(gridLocation);
            };

            this.MouseCanvas.MouseLeave += (sender, e) =>
            {
                // Hide Mouse Locator
                this.MouseRectangle.Visibility = Visibility.Hidden;
                this.MousePath.Visibility = Visibility.Hidden;
            };

            // Initialize Size / Rendering
            _viewModel.LayoutUpdated += () =>
            {
                this.LevelWidth = scenarioUIService.LevelUIWidth;
                this.LevelHeight = scenarioUIService.LevelUIHeight;

                RenderLayout();
            };

            _viewModel.VisibilityUpdated += () =>
            {
                RenderLayout();
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
            var adjustment = this.LevelContainerBorder.TransformToAncestor(window).Transform(location);

            _translateXform.X += (midpt.X - (adjustment.X));
            _translateXform.Y += (midpt.Y - (adjustment.Y));
        }

        private void ShiftDisplay(ShiftDisplayType type)
        {
            switch (type)
            {
                case ShiftDisplayType.Right:
                    _translateXform.X -= SHIFT_AMOUNT;
                    break;
                case ShiftDisplayType.Left:
                    _translateXform.X += SHIFT_AMOUNT;
                    break;
                case ShiftDisplayType.Down:
                    _translateXform.Y -= SHIFT_AMOUNT;
                    break;
                case ShiftDisplayType.Up:
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
            RenderLayout();
        }

        private void UpdateCursorLocation(GridLocation gridLocation)
        {
            //// Cache the location to prevent over-loading the path finder
            //_cursorGridLocation = gridLocation;

            //var playerGridLocation = _scenarioUIGeometryService.UI2Cell(_viewModel.Player.Location);

            //// Check to see if mouse is over a visible location
            //if (_modelService.Level.Grid[gridLocation.Column, gridLocation.Row] != null)
            //{
            //    // Find path from player to mouse location
            //    var pathLocations = _pathFinder.FindPath(playerGridLocation, gridLocation);

            //    // Create rectangles for the path
            //    var pathRects = pathLocations.Select(x => _scenarioUIGeometryService.Cell2UIRect(x, false));
            //    var pathGeometry = new PathGeometry();

            //    foreach (var rect in pathRects)
            //    {
            //        pathGeometry.Figures.Add(new PathFigure()
            //        {
            //            StartPoint = rect.Location,
            //            Segments = new PathSegmentCollection()
            //            {
            //                new LineSegment(rect.TopRight, true),
            //                new LineSegment(rect.BottomRight, true),
            //                new LineSegment(rect.BottomLeft, true)
            //            },
            //            IsClosed = true
            //        });
            //    }

            //    this.MousePath.Data = pathGeometry;
            //}

            //var location = _scenarioUIGeometryService.Cell2UI(gridLocation, false);

            //Canvas.SetLeft(this.MouseRectangle, location.X);
            //Canvas.SetTop(this.MouseRectangle, location.Y);

            //// Show Mouse Locator
            //this.MouseRectangle.Visibility = Visibility.Visible;
            //this.MousePath.Visibility = Visibility.Visible;
        }

        private void RenderLayout()
        {
            // Render layout layer to writeable bitmap
            var layoutBitmap = new WriteableBitmap((int)(this.LevelWidth * _modelService.ZoomFactor),
                                                   (int)(this.LevelHeight * _modelService.ZoomFactor),
                                                   LAYOUT_BITMAP_DPI,
                                                   LAYOUT_BITMAP_DPI,
                                                   PixelFormats.Pbgra32, null);

            using (var bitmapContext = layoutBitmap.GetBitmapContext())
            {
                _viewModel.VisibleLayer.Iterate((column, row) =>
                {
                    if (_viewModel.VisibleLayer[column, row] == null)
                        return;

                    // TODO: Only render the layout layer if nothing is on top
                    //if (_modelService.Level.Content[column, row].Any())
                    //    return;

                    // Fetch bitmap from cache
                    var bitmap = _scenarioBitmapSourceFactory.GetImageSource(_viewModel.VisibleLayer[column, row], _modelService.ZoomFactor);

                    // Calculate the rectangle in which to render the image
                    var renderRect = new Rect(column * ModelConstants.CellWidth * _modelService.ZoomFactor,
                                              row * ModelConstants.CellHeight * _modelService.ZoomFactor,
                                              ModelConstants.CellWidth * _modelService.ZoomFactor,
                                              ModelConstants.CellHeight * _modelService.ZoomFactor);

                    // Use WriteableBitmapEx extension method to overwrite pixels on the target
                    bitmapContext.WriteableBitmap.Blit(renderRect,
                                                       bitmap,
                                                       new Rect(new Size(bitmap.Width, bitmap.Height)));
                });
            }

            this.LayoutImage.Source = layoutBitmap;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (_viewModel != null)
            {
                CenterOnLocation(_viewModel.Player.Location);

                RenderLayout();
            }
        }
    }
}

