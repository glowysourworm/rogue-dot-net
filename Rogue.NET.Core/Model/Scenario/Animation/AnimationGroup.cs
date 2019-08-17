﻿using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Animation
{
    [Serializable]
    public class AnimationGroup : RogueBase
    {
        public List<AnimationData> Animations { get; set; }
        public AlterationTargetType TargetType { get; set; }

        public AnimationGroup()
        {
            this.Animations = new List<AnimationData>();
        }
    }
}
