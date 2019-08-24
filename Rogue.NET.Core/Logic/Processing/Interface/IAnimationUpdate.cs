using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    /// <summary>
    /// Specification for updating the UI with the animations. ("Update" chosen for 
    /// event bubbling back to the UI. "Action" chosen for method calls on the backend)
    /// </summary>
    public interface IAnimationUpdate : IRogueUpdate
    {
        IEnumerable<AnimationData> Animations { get; set; }
        GridLocation SourceLocation { get; set; }
        IEnumerable<GridLocation> TargetLocations { get; set; }
    }
}
