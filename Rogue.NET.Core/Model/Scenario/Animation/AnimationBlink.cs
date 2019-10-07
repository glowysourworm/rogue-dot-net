using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Animation
{
    [Serializable]
    public class AnimationBlink : AnimationBase
    {
        public int AnimationTime { get; set; }
        public int RepeatCount { get; set; }
        public bool AutoReverse { get; set; }
        public double Opacity1 { get; set; }
        public double Opacity2 { get; set; }

        public AnimationBlink()
        {
            this.AnimationTime = 1000;
            this.RepeatCount = 1;
            this.AutoReverse = false;
            this.Opacity1 = 1;
            this.Opacity2 = 0;
        }
    }
}
