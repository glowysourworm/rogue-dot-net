using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.Scenario.Animation
{
    [Serializable]
    public class AnimationChainConstantVelocity : AnimationEllipseBase
    {
        public int RepeatCount { get; set; }
        public int Velocity { get; set; }
        public bool AutoReverse { get; set; }
        public bool Reverse { get; set; }
        public int Erradicity { get; set; }

        public AnimationChainConstantVelocity()
        {
            this.Velocity = 250;
            this.AutoReverse = false;
            this.Reverse = false;
            this.Erradicity = 1;
            this.RepeatCount = 1;
        }
    }
}
