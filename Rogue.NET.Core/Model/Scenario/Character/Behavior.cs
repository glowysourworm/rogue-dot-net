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
        public bool CanOpenDoors { get; set; }
        public double EngageRadius { get; set; }
        public double DisengageRadius { get; set; }
        public double CriticalRatio { get; set; }
        public double CounterAttackProbability { get; set; }

        public Behavior()
        {
            this.EnemySkill = new Spell();
        }
    }
}
