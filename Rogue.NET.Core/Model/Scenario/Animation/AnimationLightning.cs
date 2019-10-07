using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.Scenario.Animation
{
    [Serializable]
    public class AnimationLightning : AnimationBase
    {
        public int AnimationTime { get; set; }
        public int IncrementHeightLimit { get; set; }
        public int IncrementWidthLimit { get; set; }
        public int HoldEndTime { get; set; }

        public AnimationLightning()
        {
            this.AnimationTime = 1000;
            this.IncrementHeightLimit = 10;
            this.IncrementWidthLimit = 10;
            this.HoldEndTime = 750;
        }
    }
}
