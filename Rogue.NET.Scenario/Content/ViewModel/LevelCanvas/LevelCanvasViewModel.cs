using Prism.Events;

using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Graveyard;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
using Rogue.NET.Core.Model.Scenario.Content.Doodad;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Logic.Processing;

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
using System.Windows.Threading;

namespace Rogue.NET.Scenario.Content.ViewModel.LevelCanvas
{
    [Export(typeof(LevelCanvasViewModel))]
    public class LevelCanvasViewModel : NotifyViewModel
    {
        readonly IScenarioResourceService _resourceService;
        readonly IModelService _modelService;
        readonly IAnimationGenerator _animationGenerator;

        // Identifies the layout entry in the content dictionary
        const string WALLS_KEY = "Layout";
        const string DOORS_KEY = "Doors";

        ObservableCollection<FrameworkElement> _content;
        int _levelWidth;
        int _levelHeight;

        // Used for quick access to elements
        IDictionary<string, FrameworkElement> _contentDict;

        [ImportingConstructor]
        public LevelCanvasViewModel(
            IScenarioResourceService resourceService, 
            IEventAggregator eventAggregator, 
            IAnimationGenerator animationGenerator,
            IModelService modelService)
        {
            _resourceService = resourceService;
            _modelService = modelService;
            _animationGenerator = animationGenerator;

            this.Contents = new ObservableCollection<FrameworkElement>();
            _contentDict = new Dictionary<string, FrameworkElement>();

            // Defaults for canvas size
            this.LevelHeight = 500;
            this.LevelWidth = 500;

            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                // TODO: REMOVE THIS
                _modelService.UpdateVisibleLocations();
                _modelService.UpdateContents();

                DrawLayout();
                DrawContent();

                UpdateVisibility();

            }, true);

            eventAggregator.GetEvent<LevelUpdateEvent>().Subscribe((update) =>
            {
                OnLevelUpdate(update);
            }, true);

            eventAggregator.GetEvent<AnimationStartEvent>().Subscribe(async update =>
            {
                await PlayAnimationSeries(update);
            });
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
            }
        }
        public int LevelHeight
        {
            get { return _levelHeight; }
            set
            {
                _levelHeight = value;
                OnPropertyChanged("LevelHeight");
            }
        }
        #endregion

        private void OnLevelUpdate(ILevelUpdate levelUpdate)
        {
            // TODO: Make more granular
            switch (levelUpdate.LevelUpdateType)
            {
                case LevelUpdateType.Layout:
                    DrawLayout();
                    UpdateVisibility();
                    break;
                case LevelUpdateType.LayoutVisible:
                    UpdateVisibility();
                    break;
                case LevelUpdateType.AllContent:
                    DrawLayout();
                    DrawContent();
                    UpdateVisibility();
                    break;
                case LevelUpdateType.VisibleContent:
                    DrawContent();
                    UpdateVisibility();
                    break;
                case LevelUpdateType.Player:
                    DrawContent();
                    UpdateVisibility();
                    break;
                case LevelUpdateType.RemoveCharacter:
                    RemoveContent(levelUpdate.Id);
                    break;
                case LevelUpdateType.None:
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
            var level = _modelService.CurrentLevel;
            var bounds = DataHelper.Cell2UIRect(level.Grid.GetBounds());

            this.LevelWidth = (int)bounds.Width;
            this.LevelHeight = (int)bounds.Height;

            var wallsGeometry = new StreamGeometry();
            var doorsGeometry = new StreamGeometry();

            // Draw Walls
            using (var stream = wallsGeometry.Open())
            {
                foreach (var cell in level.Grid.GetCells())
                {
                    var rect = DataHelper.Cell2UIRect(cell.Location, false);
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
                    var rect = DataHelper.Cell2UIRect(cell.Location, false);
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
            var level = _modelService.CurrentLevel;
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
                    var contentObject = CreateObject(scenarioObject);
                    UpdateOrAddContent(scenarioObject.Id, contentObject);
                }
            }
        }

        /// <summary>
        /// Draws visibility visual used as an opacity mask for the level
        /// </summary>
        private void UpdateVisibility()
        {
            var level = _modelService.CurrentLevel;
            var visibleLocations = _modelService.GetVisibleLocations();

            var opacityMaskGeometry = new StreamGeometry();

            using (var stream = opacityMaskGeometry.Open())
            {
                foreach (var cellPoint in visibleLocations)
                {
                    var rect = DataHelper.Cell2UIRect(cellPoint, false);
                    stream.BeginFigure(rect.TopLeft, true, true);
                    stream.LineTo(rect.TopRight, true, false);
                    stream.LineTo(rect.BottomRight, true, false);
                    stream.LineTo(rect.BottomLeft, true, false);
                    stream.LineTo(rect.TopLeft, true, false);
                }
            }

            var drawing = new GeometryDrawing(Brushes.White, new Pen(Brushes.White, 2), opacityMaskGeometry);
            var drawingBrush = new DrawingBrush(drawing);

            drawingBrush.Viewport = new Rect(0, 0, this.LevelWidth, this.LevelHeight);
            drawingBrush.ViewportUnits = BrushMappingMode.Absolute;

            drawingBrush.Viewbox = new Rect(0, 0, this.LevelWidth, this.LevelHeight);
            drawingBrush.ViewboxUnits = BrushMappingMode.Absolute;

            _contentDict[WALLS_KEY].OpacityMask = drawingBrush;
            _contentDict[DOORS_KEY].OpacityMask = drawingBrush;
        }
        #endregion

        #region (private) Add / Update collections
        private FrameworkElement CreateObject(ScenarioObject scenarioObject)
        {
            var image = new Image();
            image.Source = _resourceService.GetImageSource(scenarioObject);
            image.ToolTip = scenarioObject.RogueName + "   Id: " + scenarioObject.Id;

            if (scenarioObject is DoodadBase)
                Canvas.SetZIndex(image, 1);

            else if (scenarioObject is ItemBase)
                Canvas.SetZIndex(image, 2);

            else if (scenarioObject is Character)
                Canvas.SetZIndex(image, 3);

            var point = DataHelper.Cell2UI(scenarioObject.Location);

            image.Visibility = scenarioObject.IsPhysicallyVisible ? Visibility.Visible : Visibility.Hidden;

            Canvas.SetLeft(image, point.X);
            Canvas.SetTop(image, point.Y);

            return image;
        }

        private FrameworkElement UpdateObject(FrameworkElement content, ScenarioObject scenarioObject)
        {
            var point = DataHelper.Cell2UI(scenarioObject.Location);

            content.Visibility = scenarioObject.IsPhysicallyVisible ? Visibility.Visible : Visibility.Hidden;

            Canvas.SetLeft(content, point.X);
            Canvas.SetTop(content, point.Y);

            return content;
        }

        // Updates an entry in the dictionary along with the observable collection
        private void UpdateOrAddContent(string key, FrameworkElement newElement)
        {
            if (_contentDict.Any(x => x.Key == key))
            {
                var existingLayout = _contentDict[key];

                this.Contents.Remove(existingLayout);

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
            var source = DataHelper.Cell2UI(animationData.SourceLocation, true);
            var targets = animationData.TargetLocations.Select(x =>  DataHelper.Cell2UI(x, true)).ToArray();
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

                // Wait for completion
                var waitTime = animationData.Animations.Sum(x => x.AnimationTime * x.RepeatCount * (x.AutoReverse ? 2 : 1));

                await Task.Delay(waitTime);
            }
        }

        private void OnAnimationTimerElapsed(ITimedGraphic sender)
        {
            foreach (var timedGraphic in sender.GetGraphics())
                this.Contents.Remove(timedGraphic);

            sender.TimeElapsed -= new TimerElapsedHandler(OnAnimationTimerElapsed);
            sender.CleanUp();
        }
        #endregion
    }
}
