using Rogue.NET.Common.Constant;
using Rogue.NET.Core.Math.Geometry;
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
using Rogue.NET.Core.Processing.Service.Cache.Interface;
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
        readonly IScenarioBitmapSourceFactory _scenarioBitmapSourceFactory;
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
                IScenarioBitmapSourceFactory scenarioBitmapSourceFactory,
                IScenarioResourceService scenarioResourceService,
                IAlterationCalculator alterationCalculator,
                IAnimationSequenceCreator animationSequenceCreator,
                IModelService modelService)
        {
            _scenarioUIGeometryService = scenarioUIGeometryService;
            _scenarioBitmapSourceFactory = scenarioBitmapSourceFactory;
            _scenarioResourceService = scenarioResourceService;
            _alterationCalculator = alterationCalculator;
            _animationSequenceCreator = animationSequenceCreator;
            _modelService = modelService;
        }

        public void CreateRenderingMask(GeometryDrawing[,] renderingMask)
        {
            var pen = new Pen(Brushes.Transparent, 0.0);
            pen.Freeze();

            var visibleBrush = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00));
            visibleBrush.Freeze();

            var exploredBrush = new SolidColorBrush(Color.FromArgb(0x3F, 0x00, 0x00, 0x00));
            exploredBrush.Freeze();

            var revealedBrush = new SolidColorBrush(Color.FromArgb(0x3F, 0x0F, 0x03, 0x03));
            revealedBrush.Freeze();

            var opaqueBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0x00));
            opaqueBrush.Freeze();

            for (int i = 0; i < this.LevelWidth; i++)
            {
                for (int j = 0; j < this.LevelHeight; j++)
                {
                    if (_modelService.Level.Grid[i, j] == null)
                        continue;

                    var cell = _modelService.Level.Grid[i, j];

                    var rectangle = new RectangleGeometry(_scenarioUIGeometryService.Cell2UIRect(cell.Location, false));
                    rectangle.Freeze();

                    // Visible
                    if (_modelService.Level.Movement.IsVisible(cell.Location))
                    {
                        var drawing = new GeometryDrawing(visibleBrush, pen, rectangle);
                        drawing.Freeze();

                        renderingMask[i, j] = drawing;
                    }

                    // Explored
                    else if (cell.IsExplored)
                    {
                        var drawing = new GeometryDrawing(exploredBrush, pen, rectangle);
                        drawing.Freeze();

                        renderingMask[i, j] = drawing;
                    }

                    // Revealed
                    else if (cell.IsRevealed)
                    {
                        var drawing = new GeometryDrawing(revealedBrush, pen, rectangle);
                        drawing.Freeze();

                        renderingMask[i, j] = drawing;
                    }

                    // Unknown
                    else
                    {
                        var drawing = new GeometryDrawing(opaqueBrush, pen, rectangle);
                        drawing.Freeze();

                        renderingMask[i, j] = drawing;
                    }
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
            var lineOfSightVisible = scenarioObject == _modelService.Player || _modelService.Level.Movement.IsVisible(scenarioObject);

            // Content object is visible on the map (Explored, Detected, Revealed)
            var visibleToPlayer = lineOfSightVisible ||

                                  // Memorized, Detected, or Revealed
                                  isMemorized ||
                                  scenarioObject.IsDetectedAlignment ||
                                  scenarioObject.IsDetectedCategory ||
                                  scenarioObject.IsRevealed;

            var location = isMemorized ? _modelService.Level.MemorizedContent[scenarioObject] :
                                         _modelService.Level.Content[scenarioObject];

            // Effective Lighting - TODO: SET UP EFFECTS PIPELINE FOR LIGHTING
            //var lighting = _modelService.Level.Grid[location].EffectiveLighting;

            // "Invisible" status
            var isCharacterInVisibleToPlayer = false;

            // Calculate effective symbol
            var effectiveSymbol = (scenarioObject is CharacterBase) ? _alterationCalculator.CalculateEffectiveSymbol(scenarioObject as CharacterBase) :
                                                                      scenarioObject;

            // Calculate effective vision
            var effectiveVision = _modelService.Level.Movement.GetEffectiveVision(location.Column, location.Row);

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
                content.Source = GetSymbol(effectiveSymbol, lineOfSightVisible, false, false, effectiveVision, _modelService.Level.Grid[location].Lights);
            }
            else if (scenarioObject.IsDetectedAlignment)
            {
                // TODO: The "RogueName" should probably be for the alteration category; but don't have that information for
                //       Alignment detection
                switch (scenarioObject.DetectedAlignmentType)
                {
                    case AlterationAlignmentType.Neutral:
                        content.Source = GetSymbol(ScenarioImage.CreateGameSymbol(scenarioObject.RogueName, GameSymbol.DetectMagicNeutral), lineOfSightVisible, false, false, 1.0, Light.White);
                        break;
                    case AlterationAlignmentType.Good:
                        content.Source = GetSymbol(ScenarioImage.CreateGameSymbol(scenarioObject.RogueName, GameSymbol.DetectMagicGood), lineOfSightVisible, false, false, 1.0, Light.White);
                        break;
                    case AlterationAlignmentType.Bad:
                        content.Source = GetSymbol(ScenarioImage.CreateGameSymbol(scenarioObject.RogueName, GameSymbol.DetectMagicBad), lineOfSightVisible, false, false, 1.0, Light.White);
                        break;
                    default:
                        throw new Exception("Unhandled AlterationAlignmentType ScenarioUIService.UpdateContent");
                }
            }
            else if (scenarioObject.IsDetectedCategory)
            {
                content.Source = GetSymbol(scenarioObject.DetectedAlignmentCategory, lineOfSightVisible, false, false, 1.0, Light.White);
            }
            else if (scenarioObject.IsRevealed)
            {
                content.Source = GetSymbol(effectiveSymbol, lineOfSightVisible, false, true, 1.0, Light.White);
            }
            else if (isMemorized)
            {
                content.Source = GetSymbol(effectiveSymbol, lineOfSightVisible, true, false, 1.0, Light.WhiteExplored);
            }
            else
            {
                // Default to "Visible" - but will be hidden on the map
                content.Source = GetSymbol(effectiveSymbol, true, false, false, effectiveVision, _modelService.Level.Grid[location].Lights);
            }

            // TODO: Design Content Tooltip
            content.ToolTip = scenarioObject.RogueName + "   Id: " + scenarioObject.Id;

            // Set visibility
            content.Visibility = visibleToPlayer && !isCharacterInVisibleToPlayer ? Visibility.Visible : Visibility.Hidden;

            // Set Location (Canvas Location)
            content.Location = _scenarioUIGeometryService.Cell2UI(location);
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
                new GradientStop(ColorOperations.Convert(auraColor), .8),
                new GradientStop(ColorOperations.Convert(auraColor), .9),
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

        private DrawingImage GetSymbol(ScenarioImage scenarioImage, bool isVisible, bool isExplored, bool isRevealed, double effectiveVision, params Light[] lighting)
        {
            // Visible
            if (isVisible)
                return _scenarioResourceService.GetImageSource(scenarioImage, 1.0, effectiveVision, lighting);

            // Revealed
            else if (isRevealed)
                return _scenarioResourceService.GetDesaturatedImageSource(scenarioImage, 1.0, 1.0, Light.WhiteRevealed);

            // Explored
            else if (isExplored)
                return _scenarioResourceService.GetImageSource(scenarioImage, 1.0, 1.0, Light.WhiteExplored);

            else
                throw new Exception("Unhandled Exception LevelLayoutImage.GetSymbol");
        }
    }
}
