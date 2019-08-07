using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class CreateMonsterAlterationEffect
        : RogueBase, IConsumableAlterationEffect,
                     IDoodadAlterationEffect,
                     IEnemyAlterationEffect,
                     ISkillAlterationEffect
    {
        public AlterationRandomPlacementType RandomPlacementType { get; set; }

        public string CreateMonsterEnemy { get; set; }

        // TODO:ALTERATION
        /// <summary>
        /// Range from acting character (based on the placement type)
        /// </summary>
        public int Range { get; set; }

        public CreateMonsterAlterationEffect() { }
    }
}
