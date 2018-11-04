using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System.Collections.Generic;
using System.Windows;

namespace Rogue.NET.Core.Logic.Processing.Interface
{
    /// <summary>
    /// Specification for updating the UI with the animations. ("Update" chosen for 
    /// event bubbling back to the UI. "Action" chosen for method calls on the backend)
    /// </summary>
    public interface IAnimationUpdate
    {
        IEnumerable<AnimationTemplate> Animations { get; set; }
        CellPoint SourceLocation { get; set; }
        IEnumerable<CellPoint> TargetLocations { get; set; }
    }
}
