using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing
{
    public class AnimationUpdate : IAnimationUpdate
    {
        public IEnumerable<AnimationTemplate> Animations { get; set; }
        public CellPoint SourceLocation { get; set; }
        public IEnumerable<CellPoint> TargetLocations { get; set; }
    }
}
