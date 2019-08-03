using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    public class AnimationGroupTemplate : Template
    {
        public List<AnimationTemplate> Animations { get; set; }

        public AnimationGroupTemplate()
        {
            this.Animations = new List<AnimationTemplate>();
        }
    }
}
