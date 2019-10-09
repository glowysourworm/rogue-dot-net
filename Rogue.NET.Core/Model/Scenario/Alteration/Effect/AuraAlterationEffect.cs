using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class AuraAlterationEffect : RogueBase, IEquipmentCurseAlterationEffect,
                                                   IEquipmentEquipAlterationEffect,
                                                   ISkillAlterationEffect
    {
        public SymbolEffectTemplate SymbolAlteration { get; set; }
        public double Strength { get; set; }
        public double Intelligence { get; set; }
        public double Agility { get; set; }
        public double Speed { get; set; }
        public double HpPerStep { get; set; }
        public double StaminaPerStep { get; set; }
        public double Attack { get; set; }
        public double Defense { get; set; }

        public AuraAlterationEffect()
        {
            this.SymbolAlteration = new SymbolEffectTemplate();
        }
    }
}
