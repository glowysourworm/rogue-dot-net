using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Content.Interface
{
    public interface IInteractionProcessor
    {
        void CalculateAttackAttributeMelee(string alterationDisplayName, Enemy enemy, IEnumerable<AttackAttribute> offenseAttributes);
        void CalculateAttackAttributeMelee(string alterationDisplayName, Player player, IEnumerable<AttackAttribute> offenseAttributes);

        double CalculateEnemyTurn(Player player, Enemy enemy);

        void CalculatePlayerMeleeHit(Player player, Enemy enemy);
        bool CalculatePlayerRangeHit(Player player, Enemy targetedEnemy);
        void CalculateEnemyHit(Player player, Enemy enemy);
        bool CalculateEnemyRangeHit(Player player, Enemy enemy);
        bool CalculateSpellBlock(Enemy enemy);
        bool CalculateSpellBlock(Player player);
    }
}
