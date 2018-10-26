using Rogue.NET.Core.Logic.Event;
using System;

namespace Rogue.NET.Core.Logic.Interface
{
    /// <summary>
    /// Component responsible for processing events involved with using a player or enemy spell. This includes
    /// alterations, animations, and events back to UI listeners.
    /// </summary>
    public interface ISpellEngine
    {
        event EventHandler<LevelChangeEventArgs> LevelChangeEvent;
        event EventHandler<SplashEventType> SplashEvent;
        event EventHandler<AnimationEventArgs> AnimationEvent;
    }
}
