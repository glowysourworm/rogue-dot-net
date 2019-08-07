using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Processing
{
    public class AnimationUpdate_NEW : IAnimationUpdate
    {
        public IEnumerable<AnimationTemplate> Animations { get; set; }
        public IEnumerable<AnimationData> Animations_NEW { get; set; }
        public CellPoint SourceLocation { get; set; }
        public IEnumerable<CellPoint> TargetLocations { get; set; }
    }
}
