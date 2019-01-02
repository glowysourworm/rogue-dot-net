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
        /// Calculates the deducted HP for the effective { attack (offense), resistance (defense) }
        /// </summary>
        /// <param name="attack">Effective (offensive) attack sum</param>
        /// <param name="resistance">Effective (defensive) resistance</param>
        /// <returns>Deducted HP from the target character</returns>
        public static double CalculateAttackAttributeMelee(double attack, double resistance)
        {
            return attack > 0 ? attack * (1 - (resistance / (attack + resistance))) : 0;
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
