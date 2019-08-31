using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing.Interface;

using System.Linq;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Media;
using Rogue.NET.Core.Media.Interface;
using Rogue.NET.Core.Model;
using System.Threading.Tasks;
using System;
using Rogue.NET.Core.Utility;
using Rogue.NET.Core.Logic.Content.Interface;
using Rogue.NET.Scenario.Service.Interface;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using System.Collections.ObjectModel;
using Rogue.NET.Common.Extension;

namespace Rogue.NET.Scenario.Content.ViewModel.LevelCanvas
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(LevelCanvasViewModel))]
    public class LevelCanvasViewModel : NotifyViewModel
    {
        readonly IScenarioUIGeometryService _scenarioUIGeometryService;
        readonly IScenarioResourceService _resourceService;
        readonly IModelService _modelService;
        readonly IAnimationCreator _animationCreator;
        readonly IAlterationProcessor _alterationProcessor;

        const int DOODAD_ZINDEX = 1;
        const int ITEM_ZINDEX = 2;
        const int CHARACTER_ZINDEX = 3;

        int _levelWidth;
        int _levelHeight;
        Point _playerLocation = new Point(0, 0);

        // Targeting animation (singular)
        IList<ITimedGraphic> _targetingAnimations;

        // Elements for the layout
        Path _wallElement;
        Path _doorElement;
        Path _revealedElement;

        // Opacity Masks
        DrawingBrush _exploredDrawingBrush;
        DrawingBrush _revealedDrawingBrush;
        DrawingBrush _visibleDrawingBrush;

        [ImportingConstructor]
        public LevelCanvasViewModel(
            IScenarioUIGeometryService scenarioUIGeometryService,
            IScenarioResourceService resourceService,
            IRogueEventAggregator eventAggregator, 
            IAnimationCreator animationCreator,
            IModelService modelService,
            IAlterationProcessor alterationProcessor)
        {
            _scenarioUIGeometryService = scenarioUIGeometryService;
            _resourceService = resourceService;
            _modelService = modelService;
            _animationCreator = animationCreator;
            _alterationProcessor = alterationProcessor;

            this.WallLayout = new Path();
            this.DoorLayout = new Path();
            this.RevealedLayout = new Path();

            this.Animations = new ObservableCollection<FrameworkElement>();
            this.Auras = new ObservableCollection<LevelCanvasShape>();
            this.Contents = new ObservableCollection<LevelCanvasImage>();
            this.LightRadii = new ObservableCollection<LevelCanvasShape>();

            this.ExploredOpacityMask = new DrawingBrush();
            this.RevealedOpacityMask = new DrawingBrush();
            this.VisibleOpacityMask = new DrawingBrush();

            this.ExploredOpacityMask.ViewboxUnits = BrushMappingMode.Absolute;
            this.VisibleOpacityMask.ViewboxUnits = BrushMappingMode.Absolute;
            this.RevealedOpacityMask.ViewboxUnits = BrushMappingMode.Absolute;

            this.ExploredOpacityMask.ViewportUnits = BrushMappingMode.Absolute;
            this.VisibleOpacityMask.ViewportUnits = BrushMappingMode.Absolute;
            this.RevealedOpacityMask.ViewportUnits = BrushMappingMode.Absolute;

            _targetingAnimations = new List<ITimedGraphic>();

            // Defaults for canvas size
            this.LevelHeight = 500;
            this.LevelWidth = 500;

            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                this.Animations.Clear();
                this.Contents.Clear();

                _targetingAnimations.Clear();

                DrawLayout();
                DrawContent();
                UpdateLayoutVisibility();
            });

            eventAggregator.GetEvent<LevelUpdateEvent>().Subscribe((update) =>
            {
                OnLevelUpdate(update);
            });

            eventAggregator.GetEvent<AnimationStartEvent>().Subscribe(async update =>
            {
                await PlayAnimationSeries(update);
            });
        }

        #region (public) Properties
        /// <summary>
        /// Wall layer of the layout
        /// </summary>
        public Path WallLayout
        {
            get { return _wallElement; }
            set { this.RaiseAndSetIfChanged(ref _wallElement, value); }
        }

        /// <summary>
        /// Door layer of the layout
        /// </summary>
        public Path DoorLayout
        {
            get { return _doorElement; }
            set { this.RaiseAndSetIfChanged(ref _doorElement, value); }
        }

        /// <summary>
        /// Revealed layer of the layout
        /// </summary>
        public Path RevealedLayout
        {
            get { return _revealedElement; }
            set { this.RaiseAndSetIfChanged(ref _revealedElement, value); }
        }

        /// <summary>
        /// Opacity DrawingBrush for explored portion of the layout
        /// </summary>
        public DrawingBrush ExploredOpacityMask
        {
            get { return _exploredDrawingBrush; }
            set { this.RaiseAndSetIfChanged(ref _exploredDrawingBrush, value); }
        }

        /// <summary>
        /// Opacity DrawingBrush for revealed portion of the layout
        /// </summary>
        public DrawingBrush RevealedOpacityMask
        {
            get { return _revealedDrawingBrush; }
            set { this.RaiseAndSetIfChanged(ref _revealedDrawingBrush, value); }
        }

        /// <summary>
        /// Opacity DrawingBrush for visible portion of the layout
        /// </summary>
        public DrawingBrush VisibleOpacityMask
        {
            get { return _visibleDrawingBrush; }
            set { this.RaiseAndSetIfChanged(ref _visibleDrawingBrush, value); }
        }

        /// <summary>
        /// Layer of Visuals for the Light Radii
        /// </summary>
        public ObservableCollection<LevelCanvasShape> LightRadii { get; set; }

        /// <summary>
        /// Layer of Visuals for the Auras
        /// </summary>
        public ObservableCollection<LevelCanvasShape> Auras { get; set; }

        /// <summary>
        /// Layer of Visuals for the Contents
        /// </summary>
        public ObservableCollection<LevelCanvasImage> Contents { get; set; }

        /// <summary>
        /// Layer of Visuals for the Animations
        /// </summary>
        public ObservableCollection<FrameworkElement> Animations { get; set; }
        
        public int LevelWidth
        {
            get { return _levelWidth; }
            set
            {
                _levelWidth = value;
                OnPropertyChanged("LevelWidth");
                OnPropertyChanged("LevelContainerWidth");
                OnLevelDimensionChange();
            }
        }
        public int LevelHeight
        {
            get { return _levelHeight; }
            set
            {
                _levelHeight = value;
                OnPropertyChanged("LevelHeight");
                OnPropertyChanged("LevelContainerHeight");
                OnLevelDimensionChange();
            }
        }
        public int LevelContainerWidth
        {
            get { return _levelWidth + 200; }
        }
        public int LevelContainerHeight
        {
            get { return _levelHeight + 200; }
        }
        public Point PlayerLocation
        {
            get
            {
                if (_modelService == null)
                    return new Point(0, 0);

                return _scenarioUIGeometryService.Cell2UI(_modelService.Player.Location);
            }
        }
        #endregion

        private void OnLevelUpdate(ILevelUpdate levelUpdate)
        {
            switch (levelUpdate.LevelUpdateType)
            {
                case LevelUpdateType.ContentAll:
                    DrawContent();
                    UpdateLayoutVisibility(); // Opacity Mask for Light Radius
                    break;
                case LevelUpdateType.ContentVisible:
                    DrawContent();
                    UpdateLayoutVisibility(); // Opacity Mask for Light Radius
                    break;
                case LevelUpdateType.ContentReveal:
                    DrawContent();
                    break;
                case LevelUpdateType.ContentRemove:
                    // Filter out contents with matching id's
                    this.Contents.Filter(x => levelUpdate.ContentIds.Contains(x.ScenarioObjectId));
                    this.LightRadii.Filter(x => levelUpdate.ContentIds.Contains(x.ScenarioObjectId));
                    this.Auras.Filter(x => levelUpdate.ContentIds.Contains(x.ScenarioObjectId));
                    break;
                case LevelUpdateType.ContentAdd:
                    DrawContent();
                    break;
                case LevelUpdateType.ContentMove:
                case LevelUpdateType.ContentUpdate:
                    foreach (var contentId in levelUpdate.ContentIds)
                    {
                        // Performance Hit - Have to look up keys instead of use dictionaries for fast access...
                        //                   Problem is a design flaw with the backend queues. However, this is really
                        //                   not a performance issue. Just frustrating.

                        var content = this.Contents.FirstOrDefault(x => x.ScenarioObjectId == contentId);

                        if (_modelService.Level.HasContent(contentId) && content != null)
                            UpdateContent(content, _modelService.Level.GetContent(contentId));
                    }
                    break;
                case LevelUpdateType.LayoutAll:
                    DrawLayout();
                    UpdateLayoutVisibility();
                    break;
                case LevelUpdateType.LayoutVisible:
                    UpdateLayoutVisibility();
                    break;
                case LevelUpdateType.LayoutReveal:
                    UpdateLayoutVisibility();
                    break;
                case LevelUpdateType.LayoutTopology:
                    DrawLayout();
                    UpdateLayoutVisibility();
                    break;
                case LevelUpdateType.PlayerLocation:
                    {
                        var player = this.Contents.FirstOrDefault(x => x.ScenarioObjectId == _modelService.Player.Id);

                        if (player == null)
                            throw new Exception("Level Canvas View Model doens't contain player Id");

                        UpdateContent(player, _modelService.Player);
                        //UpdateLayoutVisibility();
                    }
                    break;
                case LevelUpdateType.TargetingStart:
                    PlayTargetAnimation();
                    break;
                case LevelUpdateType.TargetingEnd:
                    StopTargetAnimation();
                    break;
                default:
                    break;
            }
        }

        #region (private) Drawing Methods
        /// <summary>
        /// Draws entire layout and applies visibility
        /// </summary>
        private void DrawLayout()
        {
            var level = _modelService.Level;
            var bounds = _scenarioUIGeometryService.Cell2UIRect(level.Grid.Bounds);

            this.LevelWidth = (int)bounds.Width;
            this.LevelHeight = (int)bounds.Height;

            var wallsGeometry = new StreamGeometry();
            var doorsGeometry = new StreamGeometry();
            var revealedGeometry = new StreamGeometry();

            // Draw Walls
            using (var stream = wallsGeometry.Open())
            {
                // Also Draw Revealed Walls
                using (var revealedStream = revealedGeometry.Open())
                {
                    foreach (var cell in level.Grid.GetCells())
                    {
                        var rect = _scenarioUIGeometryService.Cell2UIRect(cell.Location, false);
                        var invisibleDoors = cell.InVisibleDoors;

                        stream.BeginFigure(rect.TopLeft, false, false);
                        stream.LineTo(rect.TopRight, (cell.Walls & Compass.N) != 0 || (invisibleDoors & Compass.N) != 0, true);
                        stream.LineTo(rect.BottomRight, (cell.Walls & Compass.E) != 0 || (invisibleDoors & Compass.E) != 0, true);
                        stream.LineTo(rect.BottomLeft, (cell.Walls & Compass.S) != 0 || (invisibleDoors & Compass.S) != 0, true);
                        stream.LineTo(rect.TopLeft, (cell.Walls & Compass.W) != 0 || (invisibleDoors & Compass.W) != 0, true);

                        revealedStream.BeginFigure(rect.TopLeft, false, false);
                        revealedStream.LineTo(rect.TopRight, (cell.Walls & Compass.N) != 0 || (invisibleDoors & Compass.N) != 0, true);
                        revealedStream.LineTo(rect.BottomRight, (cell.Walls & Compass.E) != 0 || (invisibleDoors & Compass.E) != 0, true);
                        revealedStream.LineTo(rect.BottomLeft, (cell.Walls & Compass.S) != 0 || (invisibleDoors & Compass.S) != 0, true);
                        revealedStream.LineTo(rect.TopLeft, (cell.Walls & Compass.W) != 0 || (invisibleDoors & Compass.W) != 0, true);
                    }
                }
            }
            
            // Draw Doors
            using (var stream = doorsGeometry.Open())
            {
                foreach (var cell in level.Grid.GetDoors())
                {
                    var rect = _scenarioUIGeometryService.Cell2UIRect(cell.Location, false);
                    var visibleDoors = cell.VisibleDoors;

                    stream.BeginFigure(rect.TopLeft, false, false);
                    stream.LineTo(rect.TopRight, (visibleDoors & Compass.N) != 0, true);
                    stream.LineTo(rect.BottomRight, (visibleDoors & Compass.E) != 0, true);
                    stream.LineTo(rect.BottomLeft, (visibleDoors & Compass.S) != 0, true);
                    stream.LineTo(rect.TopLeft, (visibleDoors & Compass.W) != 0, true);
                }
            }

            this.WallLayout.Data = wallsGeometry;
            this.WallLayout.Fill = Brushes.Transparent;
            this.WallLayout.Stroke = new SolidColorBrush(ColorUtility.Convert(_modelService.Level.WallColor));
            this.WallLayout.StrokeThickness = 2;

            this.DoorLayout.Data = doorsGeometry;
            this.DoorLayout.Fill = Brushes.Transparent;
            this.DoorLayout.Stroke = new SolidColorBrush(ColorUtility.Convert(_modelService.Level.DoorColor));
            this.DoorLayout.StrokeThickness = 3;

            this.RevealedLayout.Data = revealedGeometry;
            this.RevealedLayout.Fill = Brushes.Transparent;
            this.RevealedLayout.Stroke = Brushes.White;
            this.RevealedLayout.StrokeThickness = 2;
        }

        private void DrawContent()
        {
            var level = _modelService.Level;
            var player = _modelService.Player;
            var allContents = level.GetContents()
                                   .Union(new ScenarioObject[] { player });

            // Remove
            this.Contents.Filter(x => !allContents.Any(z => z.Id == x.ScenarioObjectId));
            this.LightRadii.Filter(x => !allContents.Any(z => z.Id == x.ScenarioObjectId));
            this.Auras.Filter(x => !allContents.Any(z => z.Id == x.ScenarioObjectId));

            // Update / Add
            foreach (var scenarioObject in allContents)
            {
                var content = this.Contents.FirstOrDefault(x => x.ScenarioObjectId == scenarioObject.Id);
                var lightRadius = this.LightRadii.FirstOrDefault(x => x.Id == scenarioObject.Id);

                // Update Content
                if (content != null)
                    UpdateContent(content, scenarioObject);

                else
                    this.Contents.Add(CreateContent(scenarioObject));

                // Update Light Radius
                if ((lightRadius != null) && (scenarioObject is Player))
                    UpdateLightRadius(lightRadius, scenarioObject as Player);

                // TODO: Can support for Character if performance is good enough. Have to understand
                //       Opacity masks better. Maybe because the drawing is level-sized? Could try 
                //       putting all light radii on one drawing? etc...
                else if (scenarioObject is Player)
                    this.LightRadii.Add(CreateLightRadius(scenarioObject as Player));

                // Update / Add Auras
                if (scenarioObject is Character)
                {
                    // Auras
                    var character = scenarioObject as Character;
                    var characterAuras = character.Alteration.GetAuraSourceParameters();
                    var auraUpdates = this.Auras.Where(x => characterAuras.Select(z => z.Item1).Contains(x.Id));
                    var auraAdditions = characterAuras.Where(x => !auraUpdates.Any(z => z.Id == x.Item1));

                    // Update Auras
                    foreach (var aura in auraUpdates)
                        UpdateAura(aura, characterAuras.First(x => x.Item1 == aura.Id).Item2.AuraColor,
                                         characterAuras.First(x => x.Item1 == aura.Id).Item2.AuraRange, character);

                    // Add Auras
                    foreach (var aura in auraAdditions)
                        this.Auras.Add(CreateAura(character, aura.Item1, aura.Item2.AuraColor, aura.Item2.AuraRange));

                    // Remove Auras*** This has to be checked because there may be characters that have had their
                    //                 equipment removed
                    this.Auras.Filter(x => x.ScenarioObjectId == character.Id &&
                                           !characterAuras.Any(z => z.Item1 == x.Id));
                }
            }
        }

        /// <summary>
        /// Draws visibility visual used as an opacity mask for the level
        /// </summary>
        private void UpdateLayoutVisibility()
        {
            var exploredLocations = _modelService.GetExploredLocations();
            var visibleLocations = _modelService.GetVisibleLocations(_modelService.Player);
            var revealedLocations = _modelService.GetRevealedLocations();

            var exploredLocationsOpacityMask = new StreamGeometry();
            var visibleLocationsOpacityMask = new StreamGeometry();
            var revealedLocationsOpacityMask = new StreamGeometry();

            // Explored Locations
            using (var stream = exploredLocationsOpacityMask.Open())
            {
                foreach (var cellPoint in exploredLocations)
                {
                    var rect = _scenarioUIGeometryService.Cell2UIRect(cellPoint, false);
                    stream.BeginFigure(rect.TopLeft, true, true);
                    stream.LineTo(rect.TopRight, true, false);
                    stream.LineTo(rect.BottomRight, true, false);
                    stream.LineTo(rect.BottomLeft, true, false);
                    stream.LineTo(rect.TopLeft, true, false);
                }
            }

            // Visible Locations
            using (var stream = visibleLocationsOpacityMask.Open())
            {
                foreach (var cellPoint in visibleLocations)
                {
                    var rect = _scenarioUIGeometryService.Cell2UIRect(cellPoint, false);
                    stream.BeginFigure(rect.TopLeft, true, true);
                    stream.LineTo(rect.TopRight, true, false);
                    stream.LineTo(rect.BottomRight, true, false);
                    stream.LineTo(rect.BottomLeft, true, false);
                    stream.LineTo(rect.TopLeft, true, false);
                }
            }

            // Revealed Locations
            using (var stream = revealedLocationsOpacityMask.Open())
            {
                foreach (var cellPoint in revealedLocations)
                {
                    var rect = _scenarioUIGeometryService.Cell2UIRect(cellPoint, false);
                    stream.BeginFigure(rect.TopLeft, true, true);
                    stream.LineTo(rect.TopRight, true, false);
                    stream.LineTo(rect.BottomRight, true, false);
                    stream.LineTo(rect.BottomLeft, true, false);
                    stream.LineTo(rect.TopLeft, true, false);
                }
            }

            this.ExploredOpacityMask.Drawing = new GeometryDrawing(Brushes.White, new Pen(Brushes.White, 2), exploredLocationsOpacityMask);
            this.VisibleOpacityMask.Drawing = new GeometryDrawing(Brushes.White, new Pen(Brushes.White, 2), visibleLocationsOpacityMask);
            this.RevealedOpacityMask.Drawing = new GeometryDrawing(Brushes.White, new Pen(Brushes.White, 2), revealedLocationsOpacityMask);

            OnPropertyChanged("ExploredOpacityMask");
            OnPropertyChanged("VisibleOpacityMask");
            OnPropertyChanged("RevealedOpacityMask");

            //_contentDict[WALLS_KEY].OpacityMask = exploredDrawingBrush;
            //_contentDict[DOORS_KEY].OpacityMask = exploredDrawingBrush;
            //_contentDict[REVEALED_KEY].OpacityMask = revealedDrawingBrush;

            // PERFORMANCE ISSUE - OPACITY MASKS VERY VERY VERY SLOW. CONSIDER TRYING TO 
            // DRAW LIGHT RADIUS BEFORE RENDERING... INSTEAD OF USING OPACITY MASKS
            // Update Light Radius Opacity masks

            // JUST UPDATE PLAYER ONLY - OTHER MASKS SHOULD BE OMITTED
            //foreach (var key in _contentDict.Keys.Where(x => x.EndsWith(LIGHT_RADIUS_EXT)))
            //    _contentDict[key].OpacityMask = visibleDrawingBrush;
        }
        #endregion

        #region (private) Add / Update collections
        private LevelCanvasImage CreateContent(ScenarioObject scenarioObject)
        {
            var image = new LevelCanvasImage(scenarioObject.Id);

            UpdateContent(image, scenarioObject);

            return image;
        }

        private LevelCanvasShape CreateLightRadius(Player player)
        {
            var canvasShape = new LevelCanvasShape(player.Id, player.Id, new RectangleGeometry());

            UpdateLightRadius(canvasShape, player);

            return canvasShape;
        }

        private LevelCanvasShape CreateAura(Character character, string alterationEffectId, string auraColor, int auraRange)
        {
            var canvasShape = new LevelCanvasShape(alterationEffectId, character.Id, new RectangleGeometry());

            UpdateAura(canvasShape, auraColor, auraRange, character);

            return canvasShape;
        }

        private void UpdateContent(LevelCanvasImage content, ScenarioObject scenarioObject)
        {
            var isEnemyInvisible = false;                           // FOR ENEMY INVISIBILITY ONLY

            // Calculate effective symbol
            var effectiveSymbol = (ScenarioImage)scenarioObject;

            if (scenarioObject is Enemy)
            {
                // Calculate invisibility
                var enemy = (scenarioObject as Enemy);

                isEnemyInvisible = (enemy.IsInvisible || enemy.Is(CharacterStateType.Invisible)) && !_modelService.Player.Alteration.CanSeeInvisible();
                effectiveSymbol = _alterationProcessor.CalculateEffectiveSymbol(scenarioObject as Enemy);
            }

            else if (scenarioObject is Player)
                effectiveSymbol = _alterationProcessor.CalculateEffectiveSymbol(scenarioObject as Player);

            content.Source = scenarioObject.IsRevealed ? _resourceService.GetDesaturatedImageSource(effectiveSymbol, 1.0) :
                                                         _resourceService.GetImageSource(effectiveSymbol, 1.0);

            content.ToolTip = scenarioObject.RogueName + "   Id: " + scenarioObject.Id;

            if (scenarioObject is DoodadBase)
                Canvas.SetZIndex(content, DOODAD_ZINDEX);

            else if (scenarioObject is ItemBase)
                Canvas.SetZIndex(content, ITEM_ZINDEX);

            else if (scenarioObject is Character)
                Canvas.SetZIndex(content, CHARACTER_ZINDEX);
            else
                throw new Exception("Unhandled ScenarioObject Type");

            var point = _scenarioUIGeometryService.Cell2UI(scenarioObject.Location);

            content.Visibility = (_modelService.IsVisibleTo(_modelService.Player, scenarioObject) ||
                                scenarioObject == _modelService.Player ||
                                scenarioObject.IsRevealed) && !isEnemyInvisible ? Visibility.Visible : Visibility.Hidden;

            Canvas.SetLeft(content, point.X);
            Canvas.SetTop(content, point.Y);
        }

        private void UpdateLightRadius(LevelCanvasShape canvasShape, Player player)
        {
            var point = _scenarioUIGeometryService.Cell2UI(player.Location, true);
            var lightRadiusUI = player.GetLightRadius() * ModelConstants.CellHeight;

            // Effective Character Symbol
            var effectiveSymbol = _alterationProcessor.CalculateEffectiveSymbol(player);

            // Make the full size of the level - then apply the level opacity mask drawing
            (canvasShape.RenderedGeometry as RectangleGeometry).Rect = new Rect(0, 0, this.LevelWidth, this.LevelHeight);

            // Create Brush
            var brush = new RadialGradientBrush(ColorUtility.Convert(effectiveSymbol.SmileyLightRadiusColor), Colors.Transparent);
            brush.RadiusX = 0.7 * (lightRadiusUI / this.LevelWidth);
            brush.RadiusY = 0.7 * (lightRadiusUI / this.LevelHeight);
            brush.Center = new Point(point.X / this.LevelWidth, point.Y / this.LevelHeight);
            brush.GradientOrigin = new Point(point.X / this.LevelWidth, point.Y / this.LevelHeight);
            brush.Opacity = 0.3;

            canvasShape.Fill = brush;
            canvasShape.Stroke = null;
        }

        private void UpdateAura(LevelCanvasShape aura, string auraColor, int auraRange, Character character)
        {
            // Create a Path per character aura
            //var path = new PathGeometry();

            //foreach (var location in _modelService.GetAuraLocations(character, aura.Id))
            //{
            //    // Would be nice to make this geometry service more useful
            //    var cellRectangle = _scenarioUIGeometryService.Cell2UIRect(location, false);

            //    // Add rectangle to the path geometry data
            //    path.AddGeometry(new RectangleGeometry(cellRectangle));
            //}

            //var renderedGeometry = path.GetOutlinedPathGeometry();
            //var renderedBounds = renderedGeometry.GetRenderBounds(new Pen(Brushes.Transparent, 0));

            (aura.RenderedGeometry as RectangleGeometry).Rect = new Rect(0, 0, this.LevelWidth, this.LevelHeight);
            //(aura.RenderedGeometry as PathGeometry).AddGeometry(renderedGeometry);

            //aura.Stroke = new SolidColorBrush(ColorUtility.Convert(auraColor))
            //{
            //    Opacity = 0.5
            //};
            //aura.StrokeThickness = 0.75;
            //aura.Fill = new SolidColorBrush(ColorUtility.Convert(auraColor))
            //{
            //    Opacity = 0.05
            //};

            var auraUI = (double)auraRange * (double)ModelConstants.CellHeight;
            var point = _scenarioUIGeometryService.Cell2UI(character.Location, true);

            // Create Brush
            var brush = new RadialGradientBrush(new GradientStopCollection(new GradientStop[]
            {
                new GradientStop(Colors.Transparent, 0),
                new GradientStop(ColorUtility.Convert(auraColor), .8),
                new GradientStop(ColorUtility.Convert(auraColor), .9),
                new GradientStop(Colors.Transparent, 1)
            }));

            brush.RadiusX = 0.7 * (auraUI / this.LevelWidth);
            brush.RadiusY = 0.7 * (auraUI / this.LevelHeight);
            brush.Center = new Point(point.X / this.LevelWidth, point.Y / this.LevelHeight);
            brush.GradientOrigin = new Point(point.X / this.LevelWidth, point.Y / this.LevelHeight);

            //brush.RadiusX = 0.7 * (renderedBounds.Height / renderedBounds.Width); 
            //brush.RadiusY = 0.7;
            //brush.Center = new Point((point.X - renderedBounds.Left) / renderedBounds.Width, 
            //                         (point.Y - renderedBounds.Top) / renderedBounds.Height);
            //brush.GradientOrigin = new Point((point.X - renderedBounds.Left) / renderedBounds.Width,
            //                                 (point.Y - renderedBounds.Top) / renderedBounds.Height);

            brush.Opacity = 0.3;

            aura.Fill = brush;
            aura.Stroke = null;
        }

        protected void OnLevelDimensionChange()
        {
            this.ExploredOpacityMask.Viewport = new Rect(0, 0, this.LevelWidth, this.LevelHeight);
            this.VisibleOpacityMask.Viewport = new Rect(0, 0, this.LevelWidth, this.LevelHeight);
            this.RevealedOpacityMask.Viewport = new Rect(0, 0, this.LevelWidth, this.LevelHeight);

            this.ExploredOpacityMask.Viewbox = new Rect(0, 0, this.LevelWidth, this.LevelHeight);
            this.VisibleOpacityMask.Viewbox = new Rect(0, 0, this.LevelWidth, this.LevelHeight);
            this.RevealedOpacityMask.Viewbox = new Rect(0, 0, this.LevelWidth, this.LevelHeight);
        }
        #endregion

        #region (private) Animations
        /// <summary>
        /// Creates IRogue2TimedGraphic set for each of the animation templates and returns the
        /// last one as a handle
        /// </summary>
        public async Task PlayAnimationSeries(IAnimationUpdate animationData)
        {
            // Source / Target / Render bounds
            var source = _scenarioUIGeometryService.Cell2UI(animationData.SourceLocation, true);
            var targets = animationData.TargetLocations.Select(x => _scenarioUIGeometryService.Cell2UI(x, true)).ToArray();
            var bounds = new Rect(0, 0, _levelWidth, _levelHeight);

            //Create animations
            var animationList = animationData.Animations.Select(x =>
            {
                return new
                {
                    Animations = _animationCreator.CreateAnimation(x, bounds, source, targets),
                    AnimationData = x
                };
            });

            foreach (var animation in animationList)
            {
                // Start
                foreach (var animationQueue in animation.Animations)
                {
                    foreach (var graphic in animationQueue.GetGraphics())
                    {
                        Canvas.SetZIndex(graphic, 100);
                        this.Animations.Add(graphic);
                    }

                    animationQueue.TimeElapsed += new TimerElapsedHandler(OnAnimationTimerElapsed);
                    animationQueue.Start();

                    await Task.Delay(animationQueue.AnimationTime);
                }
            }
        }
        private void OnAnimationTimerElapsed(ITimedGraphic sender)
        {
            foreach (var timedGraphic in sender.GetGraphics())
                this.Animations.Remove(timedGraphic);

            sender.TimeElapsed -= new TimerElapsedHandler(OnAnimationTimerElapsed);
            sender.CleanUp();
        }

        private void PlayTargetAnimation()
        {
            if (_targetingAnimations.Count != 0)
                StopTargetAnimation();

            var points = _modelService.GetTargetedEnemies()
                                      .Select(x => _scenarioUIGeometryService.Cell2UI(x.Location))
                                      .ToArray();

            // Start the animation group
            foreach (var animation in _animationCreator.CreateTargetingAnimation(points))
            {
                foreach (var graphic in animation.GetGraphics())
                {
                    Canvas.SetZIndex(graphic, 100);
                    this.Animations.Add(graphic);
                }

                animation.Start();

                // Add animation to list to clear it in a separate call and stop the animation
                _targetingAnimations.Add(animation);
            }
        }
        public void StopTargetAnimation()
        {
            if (_targetingAnimations.Count != 0)
            {
                foreach (var animation in _targetingAnimations)
                {
                    foreach (var graphic in animation.GetGraphics())
                        this.Animations.Remove(graphic);

                    animation.Stop();
                    animation.CleanUp();
                }

                _targetingAnimations.Clear();
            }
        }
        #endregion
    }
}
