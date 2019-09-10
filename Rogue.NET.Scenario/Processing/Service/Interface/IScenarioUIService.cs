using Rogue.NET.Core.Media;
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
        Geometry CreateWallLayout(out Geometry revealedGeometry);
        Geometry CreateDoorLayout();
        Geometry CreateGeometry(IEnumerable<GridLocation> locations);

        void UpdateContent(LevelCanvasImage content, ScenarioObject scenarioObject);
        void UpdateLightRadius(LevelCanvasShape canvasShape, Player player, Rect levelUIBounds);
        void UpdateAura(LevelCanvasShape aura, string auraColor, int auraRange, Character character, Rect levelUIBounds);

        IEnumerable<IEnumerable<AnimationQueue>> CreateAnimations(AnimationEventData eventData, Rect levelUIBounds);
        AnimationQueue CreateTargetAnimation(GridLocation location, Color fillColor, Color strokeColor);
    }
}
