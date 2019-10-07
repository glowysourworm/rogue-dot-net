using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Animation
{
    [Serializable]
    public class AnimationSequence : RogueBase
    {
        public List<AnimationBase> Animations { get; set; }
        public AlterationTargetType TargetType { get; set; }
        public bool DarkenBackground { get; set; }

        public AnimationSequence()
        {
            this.Animations = new List<AnimationBase>();
            this.TargetType = AlterationTargetType.Source;
            this.DarkenBackground = false;
        }
    }
}
