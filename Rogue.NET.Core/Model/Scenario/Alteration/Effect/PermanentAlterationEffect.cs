using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    [AlterationBlockable(typeof(IEnemyAlterationEffect),
                         typeof(IEquipmentAttackAlterationEffect),
                         typeof(ISkillAlterationEffect))]
    [AlterationCostSpecifier(AlterationCostType.OneTime,
                             typeof(IConsumableAlterationEffect),
                             typeof(IEnemyAlterationEffect),
                             typeof(IEquipmentAttackAlterationEffect),
                             typeof(ISkillAlterationEffect))]
    public class PermanentAlterationEffect
        : RogueBase, IConsumableAlterationEffect,
                     IConsumableProjectileAlterationEffect,
                     IDoodadAlterationEffect,
                     IEnemyAlterationEffect,
                     IEquipmentAttackAlterationEffect,
                     ISkillAlterationEffect
    {
        public double Strength { get; set; }
        public double Intelligence { get; set; }
        public double Agility { get; set; }
        public double Speed { get; set; }
        public double LightRadius { get; set; }
        public double Experience { get; set; }
        public double Hunger { get; set; }
        public double Hp { get; set; }
        public double Mp { get; set; }

        public PermanentAlterationEffect()
        {
        }
    }
}
