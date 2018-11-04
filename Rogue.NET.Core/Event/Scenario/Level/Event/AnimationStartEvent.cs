﻿using Prism.Events;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System.Collections.Generic;

namespace Rogue.NET.Model.Events
{
    public enum AnimationReturnAction
    {
        None,
        ProcessPlayerSpell,
        ProcessEnemySpell
    }
    public class AnimationStartEventArgs : System.EventArgs
    {
        public AnimationReturnAction ReturnAction { get; set; }
        public List<AnimationTemplate> Animations { get; set; }
        public AlterationContainer Alteration { get; set; }
        public Character Source { get; set; }
        public List<Character> Targets { get; set; }
    }
    public class AnimationStartEvent : PubSubEvent<AnimationStartEventArgs>
    {

    }
}