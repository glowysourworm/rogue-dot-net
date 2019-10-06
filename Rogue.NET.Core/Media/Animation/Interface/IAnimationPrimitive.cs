using Rogue.NET.Common.Extension.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Media.Animation.Interface
{
    public interface IAnimationPrimitive
    {
        event SimpleEventHandler<IAnimationPrimitive> AnimationTimeElapsed;
    }
}
