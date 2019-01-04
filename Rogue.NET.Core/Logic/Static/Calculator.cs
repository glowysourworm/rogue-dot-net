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
        public static double CalculateAttackAttributeMelee(double attack, double resistance)
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
