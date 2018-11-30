using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Item;

namespace Rogue.NET.Core.Logic.Content.Interface
{
    public interface IInteractionProcessor
    {
        double CalculateAttackAttributeMelee(Enemy enemy, AttackAttribute offenseAttribute);
        double CalculateAttackAttributeMelee(Player player, AttackAttribute offenseAttribute);
        double CalculateEnemyTurn(Player player, Enemy enemy);
        void CalculatePlayerMeleeHit(Player player, Enemy enemy);
        bool CalculatePlayerRangeHit(Player player, Enemy targetedEnemy);
        void CalculateEnemyHit(Player player, Enemy enemy);
        bool CalculateSpellBlock(Enemy enemy, bool physicalBlock);
        bool CalculateSpellBlock(Player player, bool physicalBlock);
    }
}
