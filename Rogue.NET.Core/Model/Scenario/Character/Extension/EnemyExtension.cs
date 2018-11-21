using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Character.Extension
{
    public static class EnemyExtension
    {
        public static void ApplyLimits(this Enemy enemy)
        {
            if (enemy.Mp < 0)
                enemy.Mp = 0;

            if (enemy.Hp > enemy.HpMax)
                enemy.Hp = enemy.HpMax;

            if (enemy.Mp > enemy.MpMax)
                enemy.Mp = enemy.MpMax;

            if (enemy.SpeedBase < ModelConstants.MIN_SPEED)
                enemy.SpeedBase = ModelConstants.MIN_SPEED;

            if (enemy.StrengthBase < 0)
                enemy.StrengthBase = 0;

            if (enemy.AgilityBase < 0)
                enemy.AgilityBase = 0;

            if (enemy.IntelligenceBase < 0)
                enemy.IntelligenceBase = 0;
        }
    }
}
