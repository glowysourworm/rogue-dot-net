using Rogue.NET.Core.Model.Scenario.Alteration;
using System;

namespace Rogue.NET.Core.Model.Scenario.Content
{
    [Serializable]
    public class CharacterClass : ScenarioImage
    {
        public bool HasAttributeBonus { get; set; }
        public bool HasBonusAttackAttributes { get; set; }

        public AlterationEffect AttributeAlteration { get; set; }
        public AlterationEffect AttackAttributeAlteration { get; set; }

        public CharacterClass()
        {
            this.AttackAttributeAlteration = new AlterationEffect();
            this.AttributeAlteration = new AlterationEffect();
        }
    }
}
