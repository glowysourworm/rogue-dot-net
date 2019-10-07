using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Animation
{
    [Serializable]
    public class AnimationEllipseBase : AnimationBase
    {
        public AnimationEasingType EasingType { get; set; }
        public double EasingAmount { get; set; }
        public double Opacity1 { get; set; }
        public double Opacity2 { get; set; }
        public int Height1 { get; set; }
        public int Height2 { get; set; }
        public int Width1 { get; set; }
        public int Width2 { get; set; }

        public AnimationEllipseBase()
        {
            this.Height1 = 4;
            this.Height2 = 4;
            this.Opacity1 = 1;
            this.Opacity2 = 1;
            this.Width1 = 4;
            this.Width2 = 4;
        }
    }
}
