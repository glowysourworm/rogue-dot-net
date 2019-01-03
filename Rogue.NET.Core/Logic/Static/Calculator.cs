using Rogue.NET.Core.Logic.Content.Enum;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Static
{
    /// <summary>
    /// Static methods necessary to de-couple circular dependency between Character processing
    /// and Character extension methods. 
    /// </summary>
    public static class Calculator
    {
        /// <summary>
        /// Calculates the deducted HP for the effective (aggregate) Attack Attribute - taking into account
        /// scaling both Attack and Resistance by strength or intelligence.
        /// </summary>
        /// <param name="attacker">Attacking Character</param>
        /// <param name="defender">Defending Character</param>
        /// <param name="offenseAttribute">Effective (or aggregate) offensive attack attribute</param>
        /// <param name="defenseAttribute">Effective (or aggregate) defensive attack attribute</param>
        /// <returns></returns>
        public static double CalculateAttackAttributeMelee(
                Character attacker, 
                Character defender, 
                AttackAttribute offenseAttribute, 
                AttackAttribute defenseAttribute,
                InteractionType interactionType)
        {
            var attack = 0D;
            var resistance = 0D;

            // Intelligence Based Combat
            if (offenseAttribute.AppliesToIntelligenceBasedCombat && interactionType == InteractionType.Mental)
            {
                attack = offenseAttribute.ScaledByIntelligence ? attacker.GetIntelligence() * offenseAttribute.Attack : offenseAttribute.Attack;
                resistance = defenseAttribute.ScaledByIntelligence ? defender.GetIntelligence() * defenseAttribute.Resistance : defenseAttribute.Resistance;
            }
            // Strength Based Combat
            else if (offenseAttribute.AppliesToStrengthBasedCombat && interactionType == InteractionType.Physical)
            {
                attack = offenseAttribute.ScaledByStrength ? attacker.GetStrength() * offenseAttribute.Attack : offenseAttribute.Attack;
                resistance = defenseAttribute.ScaledByStrength ? defender.GetStrength() * defenseAttribute.Resistance : defenseAttribute.Resistance;
            }

            return CalculateAttackAttributeValue(attack, resistance);
        }

        /// <summary>
        /// This is used for Malign Attack Attribute calulations. Calculates the deducted HP for the 
        /// effective (aggregate) Attack Attribute - taking into account scaling both Attack and Resistance 
        /// by strength or intelligence.
        /// </summary>
        /// <param name="character">Defending Character</param>
        /// <param name="offenseAttribute">Effective (or aggregate) offensive attack attribute</param>
        /// <param name="defenseAttribute">Effective (or aggregate) defensive attack attribute</param>
        /// <returns></returns>
        public static double CalculateAttackAttributeEffect(
                Character character,
                AttackAttribute offenseAttribute,
                AttackAttribute defenseAttribute,
                InteractionType interactionType)
        {
            var attack = 0D;
            var resistance = 0D;

            // Intelligence Based Combat
            if (offenseAttribute.AppliesToIntelligenceBasedCombat && interactionType == InteractionType.Mental)
            {
                attack = offenseAttribute.Attack;
                resistance = defenseAttribute.ScaledByIntelligence ? character.GetIntelligence() * defenseAttribute.Resistance : defenseAttribute.Resistance;
            }
            // Strength Based Combat
            else if (offenseAttribute.AppliesToStrengthBasedCombat && interactionType == InteractionType.Physical)
            {
                attack = offenseAttribute.Attack;
                resistance = defenseAttribute.ScaledByStrength ? character.GetStrength() * defenseAttribute.Resistance : defenseAttribute.Resistance;
            }

            return CalculateAttackAttributeValue(attack, resistance);
        }

        public static double CalculateAttackAttributeValue(double attack, double resistance)
        {
            if (attack <= 0)
                return 0;

            if ((attack + resistance) <= 0)
                return 0;

            // Calculate the value based on [X = A * (1 - (R / A + R))]
            return attack > 0 ? attack * (1 - (resistance / (attack + resistance))) : 0;
        }

        /// <summary>
        /// Calculates relative dodge probability between two characters. This number could be positive or
        /// negative [-1, 1] depending on the relative agility. Then, adds dodgeBase to this number and clips
        /// the result to [0,1].
        /// </summary>
        public static double CalculateDodgeProbability(double dodgeBase, double defenderAgility, double attackerAgility)
        {
            if (defenderAgility <= 0)
                return 0;

            if (attackerAgility <= 0)
                return 0;

            // Calculated based on asymptotic functions using an effective ratio to calculate a dodge adjustment.
            // 
            // The ATan functions define two separate asymptotes from [0.001,1] and [1, 1000]. These are based on
            // limits of the character agility [0.1, 100]. These are strictly enforced by the "Apply Limits" methods
            // on the characters; and by the Scenario Editor. The Range of values is [-0.5, 0] to [0, 0.5] for the
            // output of the piece-wise function.
            var agilityRatio = defenderAgility / attackerAgility;

            var dodgeRelative = 0D;

            // Scaled for pseudo-logarithmic dynamics from [0.001, 1]
            if (agilityRatio <= 1)
                dodgeRelative = 0.637 * Math.Atan(agilityRatio - 1);

            else
                dodgeRelative = 0.318 * Math.Atan(agilityRatio - 1);

            var result = dodgeBase + dodgeRelative;

            // Clip result and return
            return Math.Min(Math.Max(result, 0), 1);
        }

        public static double EuclideanDistance(CellPoint location1, CellPoint location2)
        {
            double x = location2.Column - location1.Column;
            double y = location2.Row - location1.Row;
            return Math.Sqrt((x * x) + (y * y));
        }
        public static double EuclideanSquareDistance(CellPoint location1, CellPoint location2)
        {
            double x = location2.Column - location1.Column;
            double y = location2.Row - location1.Row;

            return (x * x) + (y * y);
        }
        public static double RoguianDistance(CellPoint location1, CellPoint location2)
        {
            double x = Math.Abs(location2.Column - location1.Column);
            double y = Math.Abs(location2.Row - location1.Row);
            return Math.Max(x, y);
        }
    }
}
