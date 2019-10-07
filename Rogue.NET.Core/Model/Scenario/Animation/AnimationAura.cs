using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Animation
{
    [Serializable]
    public class AnimationAura : AnimationEllipseBase
    {
        public int RepeatCount { get; set; }
        public int AnimationTime { get; set; }
        public bool AutoReverse { get; set; }

        public AnimationAura()
        {
            this.AnimationTime = 1000;
            this.AutoReverse = false;
            this.RepeatCount = 1;
        }
    }
}
