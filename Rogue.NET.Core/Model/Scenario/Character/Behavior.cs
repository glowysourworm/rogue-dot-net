using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Skill;
using System;

namespace Rogue.NET.Core.Model.Scenario.Character
{
    [Serializable]
    public class Behavior
    {
        public CharacterMovementType MovementType { get; set; }
        public CharacterAttackType AttackType { get; set; }
        public Spell EnemySkill { get; set; }

        public Behavior()
        {
            this.EnemySkill = new Spell();
        }
    }
}
