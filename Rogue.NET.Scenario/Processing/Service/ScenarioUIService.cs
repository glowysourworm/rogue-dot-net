using Rogue.NET.Core.Media;
using Rogue.NET.Core.Media.Interface;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Core.Processing.Model.Content.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas;
using Rogue.NET.Scenario.Processing.Service.Interface;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Processing.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioUIService))]
    public class ScenarioUIService : IScenarioUIService
    {
        readonly IScenarioUIGeometryService _scenarioUIGeometryService;
        readonly IScenarioResourceService _scenarioResourceService;
        readonly IAlterationProcessor _alterationProcessor;
        readonly IAnimationCreator _animationCreator;
        readonly IModelService _modelService;

        [ImportingConstructor]
        public ScenarioUIService(
                IScenarioUIGeometryService scenarioUIGeometryService,
                IScenarioResourceService scenarioResourceService,
                IAlterationProcessor alterationProcessor,
                IAnimationCreator animationCreator,
                IModelService modelService)
        {
            _scenarioUIGeometryService = scenarioUIGeometryService;
            _scenarioResourceService = scenarioResourceService;
            _alterationProcessor = alterationProcessor;
            _animationCreator = animationCreator;
            _modelService = modelService;
        }

        public Geometry CreateWallLayout(out Geometry revealedGeometry)
        {
            var wallsGeometry = new StreamGeometry();
            revealedGeometry = new StreamGeometry();

            using (var stream = wallsGeometry.Open())
            {
                using (var revealedStream = ((StreamGeometry)revealedGeometry).Open())
                {
                    foreach (var cell in _modelService.Level.Grid.GetCells())
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

            return wallsGeometry;
        }

        public Geometry CreateDoorLayout()
        {
            var doorsGeometry = new StreamGeometry();

            // Draw Doors
            using (var stream = doorsGeometry.Open())
            {
                foreach (var cell in _modelService.Level.Grid.GetDoors())
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

            return doorsGeometry;
        }

        public Geometry CreateGeometry(IEnumerable<GridLocation> locations)
        {
            var result = new StreamGeometry();

            using (var stream = result.Open())
            {
                foreach (var location in locations)
                {
                    var rect = _scenarioUIGeometryService.Cell2UIRect(location, false);
                    stream.BeginFigure(rect.TopLeft, true, true);
                    stream.LineTo(rect.TopRight, true, false);
                    stream.LineTo(rect.BottomRight, true, false);
                    stream.LineTo(rect.BottomLeft, true, false);
                    stream.LineTo(rect.TopLeft, true, false);
                }
            }

            return result;
        }

        public void UpdateContent(LevelCanvasImage content, ScenarioObject scenarioObject)
        {
            var isEnemyInvisible = false;                           // FOR ENEMY INVISIBILITY ONLY

            // Calculate effective symbol
            var effectiveSymbol = (ScenarioImage)scenarioObject;

            if (scenarioObject is NonPlayerCharacter)
            {
                // Calculate invisibility
                var character = (scenarioObject as NonPlayerCharacter);

                isEnemyInvisible = character.AlignmentType == CharacterAlignmentType.EnemyAligned && 
                                   character.Is(CharacterStateType.Invisible) && 
                                   !_modelService.Player.Alteration.CanSeeInvisible();

                effectiveSymbol = _alterationProcessor.CalculateEffectiveSymbol(character);
            }

            else if (scenarioObject is Player)
                effectiveSymbol = _alterationProcessor.CalculateEffectiveSymbol(scenarioObject as Player);

            content.Source = scenarioObject.IsRevealed ? _scenarioResourceService.GetDesaturatedImageSource(effectiveSymbol, 1.0) :
                                                         _scenarioResourceService.GetImageSource(effectiveSymbol, 1.0);

            content.ToolTip = scenarioObject.RogueName + "   Id: " + scenarioObject.Id;

            content.Visibility = (_modelService.CharacterContentInformation
                                               .GetVisibleContents(_modelService.Player)
                                               .Contains(scenarioObject) ||
                                scenarioObject == _modelService.Player ||
                                scenarioObject.IsRevealed) && !isEnemyInvisible ? Visibility.Visible : Visibility.Hidden;

            // Set Location (Canvas Location)
            content.Location = _scenarioUIGeometryService.Cell2UI(scenarioObject.Location);
        }

        public void UpdateLightRadius(LevelCanvasShape canvasShape, Player player, Rect levelUIBounds)
        {
            var point = _scenarioUIGeometryService.Cell2UI(player.Location, true);
            var lightRadiusUI = player.GetLightRadius() * ModelConstants.CellHeight;

            // Effective Character Symbol
            var effectiveSymbol = _alterationProcessor.CalculateEffectiveSymbol(player);

            // Make the full size of the level - then apply the level opacity mask drawing
            (canvasShape.RenderedGeometry as RectangleGeometry).Rect = levelUIBounds;

            // Create Brush
            var brush = new RadialGradientBrush(ColorFilter.Convert(effectiveSymbol.SmileyLightRadiusColor), Colors.Transparent);
            brush.RadiusX = 0.7 * (lightRadiusUI / levelUIBounds.Width);
            brush.RadiusY = 0.7 * (lightRadiusUI / levelUIBounds.Height);
            brush.Center = new Point(point.X / levelUIBounds.Width, point.Y / levelUIBounds.Height);
            brush.GradientOrigin = new Point(point.X / levelUIBounds.Width, point.Y / levelUIBounds.Height);
            brush.Opacity = 0.3;

            canvasShape.Fill = brush;
            canvasShape.Stroke = null;
        }

        public void UpdateAura(LevelCanvasShape aura, string auraColor, int auraRange, Character character, Rect levelUIBounds)
        {
            (aura.RenderedGeometry as RectangleGeometry).Rect = levelUIBounds;

            var auraUI = (double)auraRange * (double)ModelConstants.CellHeight;
            var point = _scenarioUIGeometryService.Cell2UI(character.Location, true);

            // Create Brush
            var brush = new RadialGradientBrush(new GradientStopCollection(new GradientStop[]
            {
                new GradientStop(Colors.Transparent, 0),
                new GradientStop(ColorFilter.Convert(auraColor), .8),
                new GradientStop(ColorFilter.Convert(auraColor), .9),
                new GradientStop(Colors.Transparent, 1)
            }));

            brush.RadiusX = 0.7 * (auraUI / levelUIBounds.Width);
            brush.RadiusY = 0.7 * (auraUI / levelUIBounds.Height);
            brush.Center = new Point(point.X / levelUIBounds.Width, point.Y / levelUIBounds.Height);
            brush.GradientOrigin = new Point(point.X / levelUIBounds.Width, point.Y / levelUIBounds.Height);

            brush.Opacity = 0.3;

            aura.Fill = brush;
            aura.Stroke = null;
        }

        public IEnumerable<IEnumerable<AnimationQueue>> CreateAnimations(AnimationEventData eventData, Rect levelUIBounds)
        {
            // Source / Target / Render bounds
            var source = _scenarioUIGeometryService.Cell2UI(eventData.SourceLocation, true);
            var targets = eventData.TargetLocations.Select(x => _scenarioUIGeometryService.Cell2UI(x, true)).ToArray();

            //Create animations
            return eventData.Animations.Select(x => _animationCreator.CreateAnimation(x, levelUIBounds, source, targets));
        }

        public AnimationQueue CreateTargetAnimation(GridLocation location, Color fillColor, Color strokeColor)
        {
            return _animationCreator.CreateTargetingAnimation(_scenarioUIGeometryService.Cell2UI(location), fillColor, strokeColor);
        }
    }
}
