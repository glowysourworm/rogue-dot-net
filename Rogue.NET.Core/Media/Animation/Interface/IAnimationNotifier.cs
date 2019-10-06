using Rogue.NET.Common.Extension.Event;
using Rogue.NET.Core.Media.Animation.EventData;

namespace Rogue.NET.Core.Media.Animation.Interface
{
    public interface IAnimationNotifier
    {
        /// <summary>
        /// Event that is fired each millisecond of the animation
        /// </summary>
        event SimpleEventHandler<AnimationTimeChangedEventData> AnimationTimeChanged;
    }
}
