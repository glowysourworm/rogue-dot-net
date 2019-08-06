using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Animation
{
    [Serializable]
    public class AnimationContainer : RogueBase
    {
        public List<AnimationData> Animations { get; set; }

        public AnimationContainer()
        {
            this.Animations = new List<AnimationData>();
        }
    }
}
