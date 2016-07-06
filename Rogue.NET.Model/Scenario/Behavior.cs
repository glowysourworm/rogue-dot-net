using Rogue.NET.Common;
using Rogue.NET.Scenario.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rogue.NET.Model.Scenario
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
