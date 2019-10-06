using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Media.Animation.EventData
{
    public class AnimationPlayerStartEventData
    {
        public IEnumerable<AnimationPrimitive> Primitives { get; set; }
    }
}
