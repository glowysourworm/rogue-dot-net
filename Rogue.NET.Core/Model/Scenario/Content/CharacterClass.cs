using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using System;

namespace Rogue.NET.Core.Model.Scenario.Content
{
    [Serializable]
    public class CharacterClass : ScenarioImage
    {
        public bool HasAttributeBonus { get; set; }
        public bool HasBonusAttackAttributes { get; set; }

        public PassiveAlterationEffect AttributeAlteration { get; set; }
        public AttackAttributePassiveAlterationEffect AttackAttributeAlteration { get; set; }

        public CharacterClass()
        {
            this.AttackAttributeAlteration = new AttackAttributePassiveAlterationEffect();
            this.AttributeAlteration = new PassiveAlterationEffect();
        }
    }
}
