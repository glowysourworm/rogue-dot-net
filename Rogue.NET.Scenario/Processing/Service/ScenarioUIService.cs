using Rogue.NET.Common.Constant;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Media.Animation.Interface;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Core.Processing.Model.Content.Calculator.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas;
using Rogue.NET.Scenario.Processing.Service.Interface;
using System;
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
        readonly IAlterationCalculator _alterationCalculator;
        readonly IAnimationSequenceCreator _animationSequenceCreator;
        readonly IModelService _modelService;

        [ImportingConstructor]
        public ScenarioUIService(
                IScenarioUIGeometryService scenarioUIGeometryService,
                IScenarioResourceService scenarioResourceService,
                IAlterationCalculator alterationCalculator,
                IAnimationSequenceCreator animationSequenceCreator,
                IModelService modelService)
        {
            _scenarioUIGeometryService = scenarioUIGeometryService;
            _scenarioResourceService = scenarioResourceService;
            _alterationCalculator = alterationCalculator;
            _animationSequenceCreator = animationSequenceCreator;
            _modelService = modelService;
        }

        public void CreateLayoutDrawings(out DrawingGroup visibleDrawing,
                                         out DrawingGroup exploredDrawing,
                                         out DrawingGroup revealedDrawing,
                                         out DrawingGroup terrainDrawing)
        {
            visibleDrawing = new DrawingGroup();
            exploredDrawing = new DrawingGroup();
            revealedDrawing = new DrawingGroup();
            terrainDrawing = new DrawingGroup();

            var layoutTemplate = _modelService.Level.Layout.Asset;

            var exploredLight = new Light(0xFF, 0xFF, 0xFF, 0.1);
            var revealedLight = new Light(0xFF, 0x00, 0xFF, 1);

            var wallSymbolExplored = _scenarioResourceService.GetImageSource(new ScenarioImage(layoutTemplate.WallSymbol), 1.0, exploredLight);
            var cellSymbolExplored = _scenarioResourceService.GetImageSource(new ScenarioImage(layoutTemplate.CellSymbol), 1.0, exploredLight);
            var doorSymbolExplored = _scenarioResourceService.GetImageSource(new ScenarioImage(layoutTemplate.DoorSymbol), 1.0, exploredLight);
            var wallLightSymbolExplored = _scenarioResourceService.GetImageSource(new ScenarioImage(layoutTemplate.DoorSymbol), 1.0, exploredLight);

            var wallSymbolRevealed = _scenarioResourceService.GetImageSource(new ScenarioImage(layoutTemplate.WallSymbol), 1.0, revealedLight);
            var cellSymbolRevealed = _scenarioResourceService.GetImageSource(new ScenarioImage(layoutTemplate.CellSymbol), 1.0, revealedLight);
            var doorSymbolRevealed = _scenarioResourceService.GetImageSource(new ScenarioImage(layoutTemplate.DoorSymbol), 1.0, revealedLight);
            var wallLightSymbolRevealed = _scenarioResourceService.GetImageSource(new ScenarioImage(layoutTemplate.DoorSymbol), 1.0, revealedLight);

            foreach (var cell in _modelService.Level.Grid.GetCells())
            {
                var rect = _scenarioUIGeometryService.Cell2UIRect(cell.Location, false);

                var terrainNames = _modelService.Level.Grid.TerrainMaps.Where(terrainMap => terrainMap[cell.Location.Column, cell.Location.Row] != null)
                                                                       .Select(terrainMap => terrainMap.Name);

                var wallSymbol = _scenarioResourceService.GetImageSource(new ScenarioImage(layoutTemplate.WallSymbol), 1.0, cell.EffectiveLighting);
                var cellSymbol = _scenarioResourceService.GetImageSource(new ScenarioImage(layoutTemplate.CellSymbol), 1.0, cell.EffectiveLighting);
                var doorSymbol = _scenarioResourceService.GetImageSource(new ScenarioImage(layoutTemplate.DoorSymbol), 1.0, cell.EffectiveLighting);
                var wallLightSymbol = _scenarioResourceService.GetImageSource(new ScenarioImage(layoutTemplate.DoorSymbol), 1.0, cell.EffectiveLighting);

                // Doors
                if (cell.IsDoor)
                {
                    visibleDrawing.Children.Add(new ImageDrawing(doorSymbol, rect));
                    exploredDrawing.Children.Add(new ImageDrawing(doorSymbolExplored, rect));
                    revealedDrawing.Children.Add(new ImageDrawing(doorSymbolRevealed, rect));
                }

                else if (cell.IsWallLight)
                {
                    visibleDrawing.Children.Add(new ImageDrawing(wallLightSymbol, rect));
                    exploredDrawing.Children.Add(new ImageDrawing(wallLightSymbolExplored, rect));
                    revealedDrawing.Children.Add(new ImageDrawing(wallLightSymbolRevealed, rect));
                }

                // Walls - Add default wall to layers { Visible, Explored, Revealed } for rendering
                else if (cell.IsWall)
                {
                    visibleDrawing.Children.Add(new ImageDrawing(wallSymbol, rect));
                    exploredDrawing.Children.Add(new ImageDrawing(wallSymbolExplored, rect));
                    revealedDrawing.Children.Add(new ImageDrawing(wallSymbolRevealed, rect));
                }

                // Terrain - Render using the terrain template
                else if (terrainNames.Any())
                {
                    // TODO:TERRAIN - HANDLE MULTIPLE LAYERS
                    var layer = layoutTemplate.TerrainLayers.First(terrain => terrain.Name == terrainNames.First());

                    var terrainSymbolVisible = _scenarioResourceService.GetImageSource(new ScenarioImage(layer.TerrainLayer.SymbolDetails), 1.0, cell.EffectiveLighting);
                    var terrainSymbolExplored = _scenarioResourceService.GetImageSource(new ScenarioImage(layer.TerrainLayer.SymbolDetails), 1.0, exploredLight);
                    var terrainSymbolRevealed = _scenarioResourceService.GetImageSource(new ScenarioImage(layer.TerrainLayer.SymbolDetails), 1.0, revealedLight);

                    visibleDrawing.Children.Add(new ImageDrawing(terrainSymbolVisible, rect));
                    exploredDrawing.Children.Add(new ImageDrawing(terrainSymbolExplored, rect));
                    revealedDrawing.Children.Add(new ImageDrawing(terrainSymbolRevealed, rect));
                }

                // Room Cells - Add "The Dot" to layers { Visible, Explored, Revealed } for rendering
                else //if (isRoom || cell.IsCorridor)
                {
                    visibleDrawing.Children.Add(new ImageDrawing(cellSymbol, rect));
                    exploredDrawing.Children.Add(new ImageDrawing(cellSymbolExplored, rect));
                    revealedDrawing.Children.Add(new ImageDrawing(cellSymbolRevealed, rect));
                }
            }
        }

        public Geometry CreateDoorLayout()
        {
            var doorsGeometry = new StreamGeometry();

            // Draw Doors
            using (var stream = doorsGeometry.Open())
            {
                foreach (var cell in _modelService.Level.Grid.GetDoors())
                {
                    // TODO:TERRAIN
                    //var rect = _scenarioUIGeometryService.Cell2UIRect(cell.Location, false);
                    //var visibleDoors = cell.VisibleDoors;

                    //stream.BeginFigure(rect.TopLeft, false, false);
                    //stream.LineTo(rect.TopRight, (visibleDoors & Compass.N) != 0, true);
                    //stream.LineTo(rect.BottomRight, (visibleDoors & Compass.E) != 0, true);
                    //stream.LineTo(rect.BottomLeft, (visibleDoors & Compass.S) != 0, true);
                    //stream.LineTo(rect.TopLeft, (visibleDoors & Compass.W) != 0, true);
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
            // Calculate visible-to-player
            //
            // Content object is within player's sight radius
            var visibleToPlayer = _modelService.CharacterContentInformation
                                               .GetVisibleContents(_modelService.Player)
                                               .Contains(scenarioObject) ||

                                  // Content object is Player
                                  scenarioObject == _modelService.Player ||

                                  // Detected or Revealed
                                  scenarioObject.IsDetectedAlignment ||
                                  scenarioObject.IsDetectedCategory ||
                                  scenarioObject.IsRevealed;

            // Effective Lighting
            var lighting = _modelService.Level.Grid[scenarioObject.Location.Column, scenarioObject.Location.Row].EffectiveLighting;

            // "Invisible" status
            var isCharacterInVisibleToPlayer = false;

            // Calculate effective symbol
            var effectiveSymbol = (scenarioObject is Character) ? _alterationCalculator.CalculateEffectiveSymbol(scenarioObject as Character) :
                                                                  scenarioObject;

            // Non-Player Characters
            if (scenarioObject is NonPlayerCharacter)
            {
                // Calculate invisibility
                var character = (scenarioObject as NonPlayerCharacter);

                // Invisible: (Conditions)
                //
                // 1) Character must be enemy aligned
                // 2) Character is invisible
                // 3) Character can NOT be revealed OR detected
                // 4) Player can't "see invisible"
                //
                isCharacterInVisibleToPlayer = character.AlignmentType == CharacterAlignmentType.EnemyAligned &&
                                               character.Is(CharacterStateType.Invisible) &&
                                               !scenarioObject.IsRevealed &&
                                               !scenarioObject.IsDetectedCategory &&
                                               !scenarioObject.IsDetectedAlignment &&
                                               !_modelService.Player.Alteration.CanSeeInvisible();
            }

            // Detected, Revealed, or Normal image source
            if (scenarioObject.IsDetectedAlignment)
            {
                // TODO: The "RogueName" should probably be for the alteration category; but don't have that information for
                //       Alignment detection
                switch (scenarioObject.DetectedAlignmentType)
                {
                    case AlterationAlignmentType.Neutral:
                        content.Source = _scenarioResourceService.GetImageSource(ScenarioImage.CreateGameSymbol(scenarioObject.RogueName, GameSymbol.DetectMagicNeutral), 1.0, lighting);
                        break;
                    case AlterationAlignmentType.Good:
                        content.Source = _scenarioResourceService.GetImageSource(ScenarioImage.CreateGameSymbol(scenarioObject.RogueName, GameSymbol.DetectMagicGood), 1.0, lighting);
                        break;
                    case AlterationAlignmentType.Bad:
                        content.Source = _scenarioResourceService.GetImageSource(ScenarioImage.CreateGameSymbol(scenarioObject.RogueName, GameSymbol.DetectMagicBad), 1.0, lighting);
                        break;
                    default:
                        break;
                }
            }
            else if (scenarioObject.IsDetectedCategory)
            {
                content.Source = _scenarioResourceService.GetImageSource(scenarioObject.DetectedAlignmentCategory, 1.0, lighting);
            }
            else if (scenarioObject.IsRevealed)
            {
                content.Source = _scenarioResourceService.GetDesaturatedImageSource(effectiveSymbol, 1.0, lighting);
            }
            else
            {
                content.Source = _scenarioResourceService.GetImageSource(effectiveSymbol, 1.0, lighting);
            }

            // TODO: Design Content Tooltip
            content.ToolTip = scenarioObject.RogueName + "   Id: " + scenarioObject.Id;

            // Set visibility
            content.Visibility = visibleToPlayer && !isCharacterInVisibleToPlayer ? Visibility.Visible : Visibility.Hidden;

            // Set Location (Canvas Location)
            content.Location = _scenarioUIGeometryService.Cell2UI(scenarioObject.Location);
        }

        public void UpdateLightRadius(LevelCanvasShape canvasShape, Character character, Rect levelUIBounds)
        {
            if (character.SymbolType != SymbolType.Smiley)
                throw new Exception("Trying to create light radius for non-smiley symbol");

            var point = _scenarioUIGeometryService.Cell2UI(character.Location, true);
            var lightRadiusUI = character.GetLightRadius() * ModelConstants.CellHeight;

            // Effective Character Symbol
            var effectiveSymbol = _alterationCalculator.CalculateEffectiveSymbol(character);

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

        public IAnimationPlayer CreateAnimation(AnimationEventData eventData, Rect levelUIBounds)
        {
            // Source / Target / Render bounds
            var source = _scenarioUIGeometryService.Cell2UI(eventData.SourceLocation, true);
            var targets = eventData.TargetLocations.Select(x => _scenarioUIGeometryService.Cell2UI(x, true)).ToArray();

            //Create animations
            return _animationSequenceCreator.CreateAnimation(eventData.Animation, levelUIBounds, source, targets);
        }

        public IAnimationPlayer CreateAnimation(ProjectileAnimationEventData eventData, Rect levelUIBounds)
        {
            var source = _scenarioUIGeometryService.Cell2UI(eventData.SourceLocation, true);
            var target = _scenarioUIGeometryService.Cell2UI(eventData.TargetLocation, true);

            if (!eventData.OrientedImage)
                return _animationSequenceCreator.CreateThrowAnimation(eventData.ProjectileImage, source, target);
            else
                return _animationSequenceCreator.CreateAmmoAnimation(eventData.ProjectileImage, source, target);
        }

        public IAnimationPlayer CreateTargetAnimation(GridLocation location, Color fillColor, Color strokeColor)
        {
            return _animationSequenceCreator.CreateTargetingAnimation(_scenarioUIGeometryService.Cell2UI(location), fillColor, strokeColor);
        }
    }
}
