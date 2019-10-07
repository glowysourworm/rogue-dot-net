using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Animation
{
    [Serializable]
    public class AnimationBubbles : AnimationEllipseBase
    {
        public int AnimationTime { get; set; }
        public int ChildCount { get; set; }
        public int Erradicity { get; set; }
        public int Radius { get; set; }
        public int RoamRadius { get; set; }

        public AnimationBubbles()
        {
            this.AnimationTime = 1000;
            this.ChildCount = 5;
            this.Erradicity = 1;
            this.Radius = 20;
            this.RoamRadius = 20;
        }
    }
}
