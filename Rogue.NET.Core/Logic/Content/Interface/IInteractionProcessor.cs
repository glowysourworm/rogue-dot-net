using Rogue.NET.Core.Logic.Content.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Content.Interface
{
    public interface IInteractionProcessor
    {
        /// <summary>
        /// Used to calculate an attack attribute offensive strike created by an Alteration -> AttackAttribute -> MeleeTarget
        /// </summary>
        /// <param name="alterationDisplayName">Alteration Display Name (AlterationEffect.DisplayName)</param>
        /// <param name="offenseAttributes">Attack Attributes coming from the Alteration</param>
        void CalculateAttackAttributeHit(string alterationDisplayName, Character attacker, Character defender, IEnumerable<AttackAttribute> offenseAttributes);

        /// <summary>
        /// Calculates increment to enemy turn counter based on relative speed of characters
        /// </summary>
        double CalculateEnemyTurnIncrement(Player player, Enemy enemy);

        /// <summary>
        /// Calculates a combat interaction between an attacker and defender. This could be any type described
        /// by interactionType (Range, Melee, etc...). Returns true if an attack landed on the defender. Attack
        /// HP is deducted from the defending character.
        /// </summary>
        bool CalculateInteraction(Character attacker, Character defender, InteractionType interactionType);

        /// <summary>
        /// Calculates a block to an alteration
        /// </summary>
        bool CalculateAlterationBlock(Character attacker, Character defender, AlterationBlockType blockType);
    }
}
