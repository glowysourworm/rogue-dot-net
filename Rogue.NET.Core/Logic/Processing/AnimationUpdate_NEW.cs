using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Processing
{
    public class AnimationUpdate : IAnimationUpdate
    {
        public IEnumerable<AnimationData> Animations { get; set; }
        public CellPoint SourceLocation { get; set; }
        public IEnumerable<CellPoint> TargetLocations { get; set; }
    }
}
