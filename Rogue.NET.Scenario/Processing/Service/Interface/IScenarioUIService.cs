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

        void CreateRenderingMask(GeometryDrawing[,] renderingMask);

        Geometry CreateOutlineGeometry(IEnumerable<GridLocation> locations);
        

        void UpdateContent(LevelCanvasImage content, ScenarioObject scenarioObject, bool isMemorized);
        void UpdateAura(LevelCanvasShape aura, string auraColor, int auraRange, CharacterBase character, Rect levelUIBounds);

        IAnimationPlayer CreateAnimation(AnimationEventData eventData, Rect levelUIBounds);
        IAnimationPlayer CreateAnimation(ProjectileAnimationEventData eventData, Rect levelUIBounds);
        IAnimationPlayer CreateTargetAnimation(GridLocation location, Color fillColor, Color strokeColor);
    }
}
