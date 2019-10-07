using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.Scenario.Animation
{
    [Serializable]
    public class AnimationSpiral : AnimationEllipseBase
    {
        public bool Clockwise { get; set; }
        public int AnimationTime { get; set; }
        public int ChildCount { get; set; }
        public int Erradicity { get; set; }
        public int Radius { get; set; }
        public int RadiusChangeRate { get; set; }
        public double RotationRate { get; set; }

        public AnimationSpiral()
        {
            this.Clockwise = false;
            this.AnimationTime = 1000;
            this.ChildCount = 5;
            this.Erradicity = 1;
            this.Radius = 20;
            this.RadiusChangeRate = 0;
            this.RotationRate = 1.0;
        }
    }
}
