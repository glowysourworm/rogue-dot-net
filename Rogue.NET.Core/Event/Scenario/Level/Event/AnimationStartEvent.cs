using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System.Collections.Generic;

namespace Rogue.NET.Model.Events
{
    public class AnimationStartEvent : RogueAsyncEvent<IAnimationUpdate>
    {

    }
}
