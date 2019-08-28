using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    [AlterationBlockable]
    [AlterationCostSpecifier(AlterationCostType.OneTime,
                             typeof(IConsumableAlterationEffect),
                             typeof(IEnemyAlterationEffect),
                             typeof(ISkillAlterationEffect))]
    public class CreateMonsterAlterationEffect
        : RogueBase, IConsumableAlterationEffect,
                     IDoodadAlterationEffect,
                     IEnemyAlterationEffect,
                     ISkillAlterationEffect
    {
        public AlterationRandomPlacementType RandomPlacementType { get; set; }

        public string CreateMonsterEnemy { get; set; }

        /// <summary>
        /// Range (in cell pseudo-euclidean distance) from acting character (based on the placement type)
        /// </summary>
        public int Range { get; set; }

        public CreateMonsterAlterationEffect() { }
    }
}
