using Prism.Events;

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
using System.Collections.ObjectModel;
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

namespace Rogue.NET.Scenario.Content.ViewModel.LevelCanvas
{
    [Export(typeof(LevelCanvasViewModel))]
    public class LevelCanvasViewModel : NotifyViewModel
    {
        readonly IScenarioUIGeometryService _scenarioUIGeometryService;
        readonly IScenarioResourceService _resourceService;
        readonly IModelService _modelService;
        readonly ICharacterProcessor _characterProcessor;
        readonly IAnimationGenerator _animationGenerator;

        // Identifies the layout entry in the content dictionary
        const string WALLS_KEY = "Layout";
        const string DOORS_KEY = "Doors";
        const string AURA_EXT = "-Aura";

        ObservableCollection<FrameworkElement> _content;
        int _levelWidth;
        int _levelHeight;
        Point _playerLocation = new Point(0, 0);

        // Targeting animation (singular)
        IList<ITimedGraphic> _targetingAnimations;

        // Used for quick access to elements
        IDictionary<string, FrameworkElement> _contentDict;

        [ImportingConstructor]
        public LevelCanvasViewModel(
            IScenarioUIGeometryService scenarioUIGeometryService,
            IScenarioResourceService resourceService, 
            IEventAggregator eventAggregator, 
            IAnimationGenerator animationGenerator,
            ICharacterProcessor characterProcessor,
            IModelService modelService)
        {
            _scenarioUIGeometryService = scenarioUIGeometryService;
            _resourceService = resourceService;
            _modelService = modelService;
            _animationGenerator = animationGenerator;
            _characterProcessor = characterProcessor;

            this.Contents = new ObservableCollection<FrameworkElement>();
            _contentDict = new Dictionary<string, FrameworkElement>();

            _targetingAnimations = new List<ITimedGraphic>();

            // Defaults for canvas size
            this.LevelHeight = 500;
            this.LevelWidth = 500;

            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                _content.Clear();
                _contentDict.Clear();
                _targetingAnimations.Clear();

                DrawLayout();
                DrawContent();
                UpdateLayoutVisibility();

            }, true);

            eventAggregator.GetEvent<LevelUpdateEvent>().Subscribe((update) =>
            {
                OnLevelUpdate(update);
            }, true);

            eventAggregator.GetEvent<AnimationStartEvent>().Subscribe(async update =>
            {
                await PlayAnimationSeries(update);

                eventAggregator.GetEvent<AnimationCompletedEvent>().Publish(update);

            }, true);
        }

        #region (public) Properties
        public ObservableCollection<FrameworkElement> Contents
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged("Contents");
            }
        }
        public int LevelWidth
        {
            get { return _levelWidth; }
            set
            {
                _levelWidth = value;
                OnPropertyChanged("LevelWidth");
                OnPropertyChanged("LevelContainerWidth");
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
                    UpdateLayoutVisibility(); // Opacity Mask for Auras
                    break;
                case LevelUpdateType.ContentVisible:
                    DrawContent();
                    UpdateLayoutVisibility(); // Opacity Mask for Auras
                    break;
                case LevelUpdateType.ContentReveal:
                    // TODO
                    break;
                case LevelUpdateType.ContentRemove:
                    foreach (var contentId in levelUpdate.ContentIds)
                        RemoveContent(contentId);
                    break;
                case LevelUpdateType.ContentAdd:
                    // TODO
                    break;
                case LevelUpdateType.ContentMove:
                    foreach (var contentId in levelUpdate.ContentIds)
                        UpdateObject(_contentDict[contentId], _modelService.Level.GetContent(contentId));
                        break;
                case LevelUpdateType.LayoutAll:
                    DrawLayout();
                    UpdateLayoutVisibility();
                    break;
                case LevelUpdateType.LayoutVisible:
                    UpdateLayoutVisibility();
                    break;
                case LevelUpdateType.LayoutReveal:
                    // TODO
                    break;
                case LevelUpdateType.LayoutTopology:
                    DrawLayout();
                    UpdateLayoutVisibility();
                    break;
                case LevelUpdateType.PlayerLocation:
                    UpdateObject(_contentDict[_modelService.Player.Id], _modelService.Player);
                    UpdateLayoutVisibility();
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
            var bounds = _scenarioUIGeometryService.Cell2UIRect(level.Grid.GetBounds());

            this.LevelWidth = (int)bounds.Width;
            this.LevelHeight = (int)bounds.Height;

            var wallsGeometry = new StreamGeometry();
            var doorsGeometry = new StreamGeometry();

            // Draw Walls
            using (var stream = wallsGeometry.Open())
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

            var wallsPath = new Path();
            wallsPath.Data = wallsGeometry;
            wallsPath.Fill = Brushes.Transparent;
            wallsPath.Stroke = Brushes.Blue;
            wallsPath.StrokeThickness = 2;

            var doorsPath = new Path();
            doorsPath.Data = doorsGeometry;
            doorsPath.Fill = Brushes.Transparent;
            doorsPath.Stroke = Brushes.Magenta;
            doorsPath.StrokeThickness = 2;

            // Update collections
            UpdateOrAddContent(WALLS_KEY, wallsPath);
            UpdateOrAddContent(DOORS_KEY, doorsPath);
        }

        private void DrawContent()
        {
            var level = _modelService.Level;
            var player = _modelService.Player;
            var visibleContents = _modelService.GetVisibleEnemies();

            // Create contents for all ScenarioObjects + Player
            DrawCollection(level.DoodadsNormal);
            DrawCollection(level.Doodads);
            DrawCollection(level.Consumables);
            DrawCollection(level.Equipment);
            DrawCollection(level.Enemies);
            DrawCollection(new ScenarioObject[] { player });
        }

        private void DrawCollection(IEnumerable<ScenarioObject> collection)
        {
            foreach (var scenarioObject in collection)
            {
                // Update
                if (_contentDict.ContainsKey(scenarioObject.Id))
                    UpdateObject(_contentDict[scenarioObject.Id], scenarioObject);

                // Add
                else
                {
                    Rectangle aura = null;
                    var contentObject = CreateObject(scenarioObject, out aura);

                    UpdateOrAddContent(scenarioObject.Id, contentObject);
                    if (aura != null)
                        UpdateOrAddContent(scenarioObject.Id + AURA_EXT, aura);
                }
            }
        }

        /// <summary>
        /// Draws visibility visual used as an opacity mask for the level
        /// </summary>
        private void UpdateLayoutVisibility()
        {
            var level = _modelService.Level;
            var exploredLocations = _modelService.GetExploredLocations();
            var visibleLocations = _modelService.GetVisibleLocations();

            var exploredLocationsOpacityMask = new StreamGeometry();
            var visibleLocationsOpacityMask = new StreamGeometry();

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

            var exploredDrawing = new GeometryDrawing(Brushes.White, new Pen(Brushes.White, 2), exploredLocationsOpacityMask);
            var visibleDrawing = new GeometryDrawing(Brushes.White, new Pen(Brushes.White, 2), visibleLocationsOpacityMask);
            var exploredDrawingBrush = new DrawingBrush(exploredDrawing);
            var visibleDrawingBrush = new DrawingBrush(visibleDrawing);

            exploredDrawingBrush.Viewport = new Rect(0, 0, this.LevelWidth, this.LevelHeight);
            exploredDrawingBrush.ViewportUnits = BrushMappingMode.Absolute;

            exploredDrawingBrush.Viewbox = new Rect(0, 0, this.LevelWidth, this.LevelHeight);
            exploredDrawingBrush.ViewboxUnits = BrushMappingMode.Absolute;

            visibleDrawingBrush.Viewport = new Rect(0, 0, this.LevelWidth, this.LevelHeight);
            visibleDrawingBrush.ViewportUnits = BrushMappingMode.Absolute;

            visibleDrawingBrush.Viewbox = new Rect(0, 0, this.LevelWidth, this.LevelHeight);
            visibleDrawingBrush.ViewboxUnits = BrushMappingMode.Absolute;
            

            _contentDict[WALLS_KEY].OpacityMask = exploredDrawingBrush;
            _contentDict[DOORS_KEY].OpacityMask = exploredDrawingBrush;

            // Update Aura Opacity masks
            foreach (var key in _contentDict.Keys.Where(x => x.EndsWith(AURA_EXT)))
                _contentDict[key].OpacityMask = visibleDrawingBrush;
        }
        #endregion

        #region (private) Add / Update collections
        private LevelCanvasImage CreateObject(ScenarioObject scenarioObject, out Rectangle aura)
        {
            var image = new LevelCanvasImage();
            image.Source = _resourceService.GetImageSource(scenarioObject);
            image.ToolTip = scenarioObject.RogueName + "   Id: " + scenarioObject.Id;

            if (scenarioObject is DoodadBase)
                Canvas.SetZIndex(image, 2);

            else if (scenarioObject is ItemBase)
                Canvas.SetZIndex(image, 3);

            else if (scenarioObject is Character)
                Canvas.SetZIndex(image, 4);
            else
                throw new Exception("Unhandled ScenarioObject Type");

            // Set the special ZIndex property - used for binding
            image.ZIndex = Canvas.GetZIndex(image);

            var point = _scenarioUIGeometryService.Cell2UI(scenarioObject.Location);

            image.Visibility = scenarioObject.IsPhysicallyVisible ? Visibility.Visible : Visibility.Hidden;

            Canvas.SetLeft(image, point.X);
            Canvas.SetTop(image, point.Y);

            aura = null;

            // AURA
            if (scenarioObject is Character &&
                scenarioObject.SymbolType == SymbolTypes.Smiley)
            {
                // TODO: Put transform somewhere else
                var auraRadiusUI = _characterProcessor.GetAuraRadius(scenarioObject as Character) * ModelConstants.CELLHEIGHT;
                var cellOffset = new Point(ModelConstants.CELLWIDTH / 2, ModelConstants.CELLHEIGHT / 2);

                // Make the full size of the level - then apply the level opacity mask drawing
                aura = new Rectangle();
                aura.Height = this.LevelHeight;
                aura.Width = this.LevelWidth;

                var brush = new RadialGradientBrush(ColorUtility.Convert(scenarioObject.SmileyAuraColor), Colors.Transparent);
                brush.RadiusX = 0.7 * (auraRadiusUI / this.LevelWidth);
                brush.RadiusY = 0.7 * (auraRadiusUI / this.LevelHeight);
                brush.Center = new Point((point.X + cellOffset.X) / this.LevelWidth, (point.Y + cellOffset.Y) / this.LevelHeight);
                brush.GradientOrigin = new Point((point.X + cellOffset.X) / this.LevelWidth, (point.Y + cellOffset.Y) / this.LevelHeight);
                brush.Opacity = 0.3;

                Canvas.SetZIndex(aura, 1);

                aura.Fill = brush;
                aura.Stroke = null;
            }

            return image;
        }

        private void UpdateObject(FrameworkElement content, ScenarioObject scenarioObject)
        {
            var point = _scenarioUIGeometryService.Cell2UI(scenarioObject.Location);

            content.Visibility = scenarioObject.IsPhysicallyVisible ? Visibility.Visible : Visibility.Hidden;

            Canvas.SetLeft(content, point.X);
            Canvas.SetTop(content, point.Y);
            
            // Update related Aura
            if (_contentDict.ContainsKey(scenarioObject.Id + AURA_EXT))
            {
                // TODO: Put transform somewhere else
                var auraRadiusUI = _characterProcessor.GetAuraRadius(scenarioObject as Character) * ModelConstants.CELLHEIGHT;
                var cellOffset = new Point(ModelConstants.CELLWIDTH / 2, ModelConstants.CELLHEIGHT / 2);

                var aura = _contentDict[scenarioObject.Id + AURA_EXT] as Rectangle;
                aura.Height = this.LevelHeight;
                aura.Width = this.LevelWidth;

                var brush = aura.Fill as RadialGradientBrush;
                brush.Center = new Point((point.X + cellOffset.X) / this.LevelWidth, (point.Y + cellOffset.Y) / this.LevelHeight);
                brush.GradientOrigin = new Point((point.X + cellOffset.X) / this.LevelWidth, (point.Y + cellOffset.Y) / this.LevelHeight);
                brush.RadiusX = 0.7 * (auraRadiusUI / this.LevelWidth);
                brush.RadiusY = 0.7 * (auraRadiusUI / this.LevelHeight);
                //brush.GradientStops[0] = TODO
            }
        }

        // Updates an entry in the dictionary along with the observable collection
        private void UpdateOrAddContent(string key, FrameworkElement newElement)
        {
            if (_contentDict.Any(x => x.Key == key))
            {
                var existingElement = _contentDict[key];

                this.Contents.Remove(existingElement);

                _contentDict[key] = newElement;
                this.Contents.Add(newElement);
            }
            else
            {
                _contentDict[key] = newElement;
                this.Contents.Add(newElement);
            }
        }

        // Removes an entry from the dictionary
        private void RemoveContent(string key)
        {
            if (_contentDict.Any(x => x.Key == key))
            {
                var content = _contentDict[key];

                _contentDict.Remove(key);
                this.Contents.Remove(content);
            }

            if (_contentDict.Any(x => x.Key == key + AURA_EXT))
            {
                var contentAura = _contentDict[key + AURA_EXT];

                _contentDict.Remove(key + AURA_EXT);
                this.Contents.Remove(contentAura);
            }
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
            var animations = animationData.Animations.Select(x =>
            {
                return _animationGenerator.CreateAnimation(x, bounds, source, targets);
            });

            foreach (var animation in animations)
            {
                // Start
                foreach (var graphic in animation.GetGraphics())
                {
                    Canvas.SetZIndex(graphic, 100);
                    this.Contents.Add(graphic);
                }

                animation.TimeElapsed += new TimerElapsedHandler(OnAnimationTimerElapsed);
                animation.Start();
            }

            // Wait for completion
            var waitTime = animationData.Animations.Sum(x => x.AnimationTime * x.RepeatCount * (x.AutoReverse ? 2 : 1));

            await Task.Delay(waitTime);
        }
        private void OnAnimationTimerElapsed(ITimedGraphic sender)
        {
            foreach (var timedGraphic in sender.GetGraphics())
                this.Contents.Remove(timedGraphic);

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
            foreach (var animation in _animationGenerator.CreateTargetingAnimation(points))
            {
                foreach (var graphic in animation.GetGraphics())
                {
                    Canvas.SetZIndex(graphic, 100);
                    this.Contents.Add(graphic);
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
                        this.Contents.Remove(graphic);

                    animation.Stop();
                    animation.CleanUp();
                }

                _targetingAnimations.Clear();
            }
        }
        #endregion
    }
}
