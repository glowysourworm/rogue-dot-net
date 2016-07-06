﻿using Microsoft.Practices.Prism.Events;
using Rogue.NET.Model.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Model.Events
{
    public enum AnimationReturnAction
    {
        None,
        ProcessPlayerSpell,
        ProcessEnemySpell
    }
    public class AnimationStartEvent : CompositePresentationEvent<AnimationStartEvent>
    {
        public AnimationReturnAction ReturnAction { get; set; }
        public List<AnimationTemplate> Animations { get; set; }
        public Alteration Alteration { get; set; }
        public Character Source { get; set; }
        public List<Character> Targets { get; set; }
    }
}
