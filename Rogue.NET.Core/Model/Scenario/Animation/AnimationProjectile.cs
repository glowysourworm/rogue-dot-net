using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.Scenario.Animation
{
    [Serializable]
    public class AnimationProjectile : AnimationEllipseBase
    {
        public int RepeatCount { get; set; }
        public int AnimationTime { get; set; }
        public bool AutoReverse { get; set; }
        public bool Reverse { get; set; }
        public int Erradicity { get; set; }

        public AnimationProjectile()
        {
            this.AnimationTime = 1000;
            this.AutoReverse = false;
            this.Reverse = false;
            this.Erradicity = 1;
            this.RepeatCount = 1;
        }
    }
}
