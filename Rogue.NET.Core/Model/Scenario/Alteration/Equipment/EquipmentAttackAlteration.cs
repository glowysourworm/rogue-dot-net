using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Animation;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Equipment
{
    [Serializable]
    public class EquipmentAttackAlteration : RogueBase
    {
        public AnimationContainer Animation { get; set; }
        public AlterationCost Cost { get; set; }
        public IEquipmentAttackAlterationEffect Effect { get; set; }
        public AlterationBlockType BlockType { get; set; }

        public EquipmentAttackAlteration()
        {
            this.Animation = new AnimationContainer();
        }
    }
}
