using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Media.Animation.EventData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Media.Animation.Interface
{
    public interface IAnimationPlayer : IAnimationController, IAnimationNotifier
    {
        /// <summary>
        /// Notifies listeners when new primitives need to be loaded into the viewer
        /// </summary>
        event SimpleEventHandler<IAnimationPlayer, AnimationPlayerChangeEventData> AnimationPlayerChangeEvent;

        /// <summary>
        /// Notifies listeners when first set of animation primitives need to be loaded into the viewer
        /// </summary>
        event SimpleEventHandler<AnimationPlayerStartEventData> AnimationPlayerStartEvent;
    }
}
