using Rogue.NET.Core.Media.Animation.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Media.Animation.EventData
{
    /// <summary>
    /// Represents a switch-over in an animation sequence. Old primitives should be removed from
    /// the viewer - and new ones are put in their place
    /// </summary>
    public class AnimationPlayerChangeEventData
    {
        public bool SequenceFinished { get; set; }

        public IEnumerable<AnimationPrimitive> OldPrimitives { get; set; }

        public IEnumerable<AnimationPrimitive> NewPrimitives { get; set; }
    }
}
