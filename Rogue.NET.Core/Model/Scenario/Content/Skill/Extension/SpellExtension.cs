using Rogue.NET.Core.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Content.Skill.Extension
{
    public static class SpellExtension
    {
        public static bool IsPassive(this Spell spell)
        {
            return spell.Type == AlterationType.PassiveAura ||
                   spell.Type == AlterationType.PassiveSource ||
                   (spell.Type == AlterationType.AttackAttribute &&
                    spell.AttackAttributeType == AlterationAttackAttributeType.Passive);
        }
    }
}
