using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;

namespace Rogue.NET.Core.Logic.Content.Interface
{
    public interface IInteractionProcessor
    {
        /// <summary>
        /// Calulcates an AttackAttribute interaction (melee event) applied to a defending character. This will
        /// include contributions from the character's state { Alterations, Equipment }
        /// </summary>
        /// <param name="character">Defending Character</param>
        /// <param name="offenseAttribute">Attribute being applied (offensively) to the character</param>
        /// <returns>Total hit to be applied to the character</returns>
        double CalculateAttackAttributeMelee(Character character, AttackAttribute offenseAttribute);

        /// <summary>
        /// Calculates the deducted HP for the effective { attack (offense), resistance (defense), weakness (defense) }
        /// </summary>
        /// <param name="attack">Effective (offensive) attack sum</param>
        /// <param name="resistance">Effective (defensive) resistance</param>
        /// <param name="weakness">Effective (defensive) weakness</param>
        /// <returns>Deducted HP from the target character</returns>
        double CalculateAttackAttributeMelee(double attack, double resistance, double weakness);

        double CalculateEnemyTurn(Player player, Enemy enemy);
        double CalculatePlayerHit(Player player, Enemy enemy);
        double CalculateEnemyHit(Player player, Enemy enemy);
        bool CalculateSpellBlock(Character character, bool physicalBlock);
    }
}
