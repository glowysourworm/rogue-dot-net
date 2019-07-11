using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Content.Skill
{
    /// <summary>
    /// A "Spell" is a container for everything that happens during an event involving an Alteration. It contains
    /// the Animation Templates to show an animation, Alteration cost and effect templates to create randomized 
    /// Alteration parameters, and effect range for certain types of AlterationEffects.
    /// </summary>
    [Serializable]
    public class Spell : RogueBase
    {
        public List<AnimationTemplate> Animations { get; set; }

        public AlterationCostTemplate Cost { get; set; }
        public AlterationEffectTemplate Effect { get; set; }
        public AlterationEffectTemplate AuraEffect { get; set; }
        public AlterationType Type { get; set; }
        public AlterationBlockType BlockType { get; set; }
        public AlterationMagicEffectType OtherEffectType { get; set; }
        public AlterationAttackAttributeType AttackAttributeType { get; set; }

        public bool IsStackable { get; set; }
        public double EffectRange { get; set; }        
        public string CreateMonsterEnemyName { get; set; }
        public string DisplayName { get; set; }

        public Religion Religion { get; set; }

        public Spell() { }
    }
}
