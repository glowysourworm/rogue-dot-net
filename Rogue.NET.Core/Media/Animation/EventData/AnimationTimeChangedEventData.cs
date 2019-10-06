using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Media.Animation.EventData
{
    public class AnimationTimeChangedEventData
    {
        public int CurrentTimeMilliseconds { get; set; }

        public AnimationTimeChangedEventData(int currentTimeMilliseconds)
        {
            this.CurrentTimeMilliseconds = currentTimeMilliseconds;
        }
    }
}
