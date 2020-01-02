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

        public int LevelWidth { get { return _modelService.Level.Grid.Bounds.Width; } }
        public int LevelHeight { get { return _modelService.Level.Grid.Bounds.Height; } }
        public int LevelUIWidth
        {
            get { return (int)_scenarioUIGeometryService.Cell2UIRect(_modelService.Level.Grid.Bounds).Width; }
        }
        public int LevelUIHeight
        {
            get { return (int)_scenarioUIGeometryService.Cell2UIRect(_modelService.Level.Grid.Bounds).Height; }
        }

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

        public void CreateLayoutDrawings(DrawingImage[,] visibleLayer,
                                         DrawingImage[,] exploredLayer,
                                         DrawingImage[,] revealedLayer)
        {
            var layoutTemplate = _modelService.GetLayoutTemplate();

            foreach (var location in _modelService.Level.Grid.FullMap.GetLocations())
            {
                var cell = _modelService.Level.Grid[location.Column, location.Row];

                var visibleLight = cell.BaseLight;
                var exploredLight = CreateExploredLight(cell.BaseLight);
                var revealedLight = CreateRevealedLight(cell.BaseLight);

                var isCorridor = _modelService.Level.Grid.CorridorMap[cell.Location.Column, cell.Location.Row] != null;
                var terrainNames = _modelService.Level.Grid.TerrainMaps.Where(terrainMap => terrainMap[cell.Location.Column, cell.Location.Row] != null)
                                                                       .Select(terrainMap => terrainMap.Name);

                // Terrain - Render using the terrain template
                if (terrainNames.Any())
                {
                    // TODO:TERRAIN - HANDLE MULTIPLE LAYERS
                    var layer = layoutTemplate.TerrainLayers.First(terrain => terrain.Name == terrainNames.First());

                    visibleLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layer.TerrainLayer.SymbolDetails, 1.0, visibleLight);
                    exploredLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layer.TerrainLayer.SymbolDetails, 1.0, exploredLight);
                    revealedLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layer.TerrainLayer.SymbolDetails, 1.0, revealedLight);
                }

                // Doors
                else if (cell.IsDoor)
                {
                    visibleLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layoutTemplate.DoorSymbol, 1.0, visibleLight);
                    exploredLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layoutTemplate.DoorSymbol, 1.0, exploredLight);
                    revealedLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layoutTemplate.DoorSymbol, 1.0, revealedLight);
                }

                // Wall Lights
                else if (cell.IsWallLight)
                {
                    visibleLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layoutTemplate.DoorSymbol, 1.0, visibleLight);
                    exploredLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layoutTemplate.DoorSymbol, 1.0, exploredLight);
                    revealedLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layoutTemplate.DoorSymbol, 1.0, revealedLight);
                }

                // Walls
                else if (cell.IsWall)
                {
                    visibleLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layoutTemplate.WallSymbol, 1.0, visibleLight);
                    exploredLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layoutTemplate.WallSymbol, 1.0, exploredLight);
                    revealedLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layoutTemplate.WallSymbol, 1.0, revealedLight);
                }

                // Walkable Cells
                else
                {
                    visibleLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layoutTemplate.CellSymbol, 1.0, visibleLight);
                    exploredLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layoutTemplate.CellSymbol, 1.0, exploredLight);
                    revealedLayer[cell.Column, cell.Row] = _scenarioResourceService.GetImageSource(layoutTemplate.CellSymbol, 1.0, revealedLight);
                }
            }
        }

        public Geometry CreateOutlineGeometry(IEnumerable<GridLocation> locations)
        {
            var result = new StreamGeometry();

            using (var stream = result.Open())
            {
                // Create an outline of the region by finding the edges
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

            return result.GetOutlinedPathGeometry();
        }

        public void UpdateContent(LevelCanvasImage content, ScenarioObject scenarioObject, bool isMemorized)
        {
            // Calculate visible-to-player
            //
            // Content object is within player's sight radius 
            var lineOfSightVisible = scenarioObject == _modelService.Player || _modelService.Level.Visibility.IsVisible(scenarioObject);

            // Content object is visible on the map (Explored, Detected, Revealed)
            var visibleToPlayer = lineOfSightVisible ||

                                  // Memorized, Detected, or Revealed
                                  isMemorized ||
                                  scenarioObject.IsDetectedAlignment ||
                                  scenarioObject.IsDetectedCategory ||
                                  scenarioObject.IsRevealed;

            var location = isMemorized ? _modelService.Level.MemorizedContent[scenarioObject] :
                                         _modelService.Level.Content[scenarioObject];

            // Effective Lighting
            var lighting = _modelService.Level.Grid[location].EffectiveLighting;

            // "Invisible" status
            var isCharacterInVisibleToPlayer = false;

            // Calculate effective symbol
            var effectiveSymbol = (scenarioObject is CharacterBase) ? _alterationCalculator.CalculateEffectiveSymbol(scenarioObject as CharacterBase) :
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

            // Normal -> Detected -> Revealed -> Memorized
            if (lineOfSightVisible)
            {
                content.Source = _scenarioResourceService.GetImageSource(effectiveSymbol, 1.0, lighting);
            }
            else if (scenarioObject.IsDetectedAlignment)
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
                content.Source = _scenarioResourceService.GetDesaturatedImageSource(effectiveSymbol, 1.0, CreateRevealedLight(lighting));
            }
            else if (isMemorized)
            {
                content.Source = _scenarioResourceService.GetImageSource(effectiveSymbol, 1.0, CreateExploredLight(lighting));
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
            content.Location = _scenarioUIGeometryService.Cell2UI(location);
        }

        public void UpdateLightRadius(LevelCanvasShape canvasShape, CharacterBase character, Rect levelUIBounds)
        {
            if (character.SymbolType != SymbolType.Smiley)
                throw new Exception("Trying to create light radius for non-smiley symbol");

            var location = _modelService.Level.Content[character];
            var point = _scenarioUIGeometryService.Cell2UI(location, true);
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

        public void UpdateAura(LevelCanvasShape aura, string auraColor, int auraRange, CharacterBase character, Rect levelUIBounds)
        {
            (aura.RenderedGeometry as RectangleGeometry).Rect = levelUIBounds;

            var location = _modelService.Level.Content[character];
            var auraUI = (double)auraRange * (double)ModelConstants.CellHeight;
            var point = _scenarioUIGeometryService.Cell2UI(location, true);

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

        private Light CreateExploredLight(Light lighting)
        {
            return new Light(lighting, 0.3);
        }

        private Light CreateRevealedLight(Light lighting)
        {
            return new Light(lighting, 1.0);
        }
    }
}
