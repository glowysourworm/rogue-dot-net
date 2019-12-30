using Rogue.NET.Core.Media;
using Rogue.NET.Core.Media.Animation.Interface;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Processing.Service.Interface
{
    /// <summary>
    /// Component responsible for "drawing" the UI for the backend
    /// </summary>
    public interface IScenarioUIService
    {
        int LevelWidth { get; }
        int LevelHeight { get; }

        int LevelUIWidth { get; }
        int LevelUIHeight { get; }

        /// <summary>
        /// Creates primary drawings for the level rendering. These are expensive to calculate; but even more to
        /// render. So, to prevent performance issues, be sure to set a bitmap cache for the Level Canvas elements
        /// that have any of the large (in terms of child elements) renderings. So, this would be (usually) the 
        /// rooms, walls, and terrain.
        /// </summary>
        void CreateLayoutDrawings(DrawingImage[,] visibleLayer,
                                  DrawingImage[,] exploredLayer,
                                  DrawingImage[,] revealedLayer);

        Geometry CreateGeometry(IEnumerable<GridLocation> locations);

        void UpdateContent(LevelCanvasImage content, ScenarioObject scenarioObject);
        void UpdateLightRadius(LevelCanvasShape canvasShape, Character character, Rect levelUIBounds);
        void UpdateAura(LevelCanvasShape aura, string auraColor, int auraRange, Character character, Rect levelUIBounds);

        IAnimationPlayer CreateAnimation(AnimationEventData eventData, Rect levelUIBounds);
        IAnimationPlayer CreateAnimation(ProjectileAnimationEventData eventData, Rect levelUIBounds);
        IAnimationPlayer CreateTargetAnimation(GridLocation location, Color fillColor, Color strokeColor);
    }
}
