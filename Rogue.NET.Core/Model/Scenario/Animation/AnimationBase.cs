using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Animation
{
    [Serializable]
    public class AnimationBase : RogueBase
    {
        public AnimationPointTargetType PointTargetType { get; set; }
        public BrushTemplate FillTemplate { get; set; }

        public AnimationBase()
        {

        }
    }
}
