using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;

namespace Rogue.NET.Core.Logic.Content.Interface
{
    public interface IInteractionProcessor
    {
        double CalculateAttackAttributeMelee(Enemy enemy, AttackAttribute offenseAttribute);
        double CalculateAttackAttributeMelee(Player player, AttackAttribute offenseAttribute);
        double CalculateEnemyTurn(Player player, Enemy enemy);
        double CalculatePlayerHit(Player player, Enemy enemy);
        double CalculateEnemyHit(Player player, Enemy enemy);
        bool CalculateSpellBlock(Enemy enemy, bool physicalBlock);
        bool CalculateSpellBlock(Player player, bool physicalBlock);
    }
}
