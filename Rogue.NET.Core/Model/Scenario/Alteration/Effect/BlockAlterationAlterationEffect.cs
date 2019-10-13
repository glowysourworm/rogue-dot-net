using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class BlockAlterationAlterationEffect : RogueBase, IEquipmentEquipAlterationEffect,
                                                              ISkillAlterationEffect
    {
        public AlterationCategory AlterationCategory { get; set; }
        public SymbolEffectTemplate SymbolAlteration { get; set; }

        public BlockAlterationAlterationEffect()
        {
            this.SymbolAlteration = new SymbolEffectTemplate();
        }
    }
}
