﻿using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using System;
using System.Drawing;

namespace Rogue.NET.Core.Processing.Model.Static
{
    /// <summary>
    /// Static methods necessary to de-couple circular dependency between Character processing
    /// and Character extension methods. 
    /// </summary>
    public static class Calculator
    {
        public static double CalculateAttackAttributeMelee(double attack, double resistance, int weakness, bool immune)
        {
            if (immune)
                return 0;

            if (attack <= 0)
                return 0;

            if ((attack + resistance) <= 0)
                return 0;

            // Calculate the value based on [X = A * ((1 - (R / A + R)) + W)]
            return attack > 0 ? attack * ((1 - (resistance / (attack + resistance))) + weakness) : 0;
        }

        /// <summary>
        /// Calculates relative probability between two characters. This will take the ratio of the defender's
        /// attribute to the attacker's attribute and return a value that asymptotically approaches 1 at infinity;
        /// and zero at zero. The purpose of this method is to lend some number to a balance between an attacker
        /// and defender. (Examples:  Dodge, Mental Block, Miss, etc...)
        /// </summary>
        public static double CalculateAttributeProbability(double defenderAttribute, double attackerAttribute)
        {
            if (defenderAttribute <= 0)
                return 0;

            if (attackerAttribute <= 0)
                return 0;

            // Calculated based on asymptotic functions using the ratio of the defender to the attacker's attribute values.
            // 
            // The ATan functions define two separate asymptotes from [0.001,1] and [1, 1000]. These are based on
            // limits of the character agility [0.1, 100]. These are strictly enforced by the "Apply Limits" methods
            // on the characters; and by the Scenario Editor. The Range of values is [-0.5, 0] to [0, 0.5] for the
            // output of the piece-wise function. Adding a base of 0.5 offsets it approx. to give a range from [0, 1) (asymptotically)
            var agilityRatio = defenderAttribute / attackerAttribute;

            var relativeProbability = 0D;

            // Scaled for pseudo-logarithmic dynamics from [0.001, 1]
            if (agilityRatio <= 1)
                relativeProbability = (0.637 * Math.Atan(agilityRatio - 1)) + 0.5;

            else
                relativeProbability = 0.318 * Math.Atan(agilityRatio - 1) + 0.5;

            // Clip result and return
            return relativeProbability.Clip(0, 1.0);
        }

        public static double EuclideanDistance(GridLocation location1, GridLocation location2)
        {
            double x = location2.Column - location1.Column;
            double y = location2.Row - location1.Row;
            return Math.Sqrt((x * x) + (y * y));
        }
        public static double EuclideanSquareDistance(GridLocation location1, GridLocation location2)
        {
            double x = location2.Column - location1.Column;
            double y = location2.Row - location1.Row;

            return (x * x) + (y * y);
        }
        public static double EuclideanSquareDistance(PointF location1, PointF location2)
        {
            double x = location2.X - location1.X;
            double y = location2.Y - location1.Y;

            return (x * x) + (y * y);
        }
        public static int RoguianDistance(GridLocation location1, GridLocation location2)
        {
            int x = Math.Abs(location2.Column - location1.Column);
            int y = Math.Abs(location2.Row - location1.Row);
            return Math.Max(x, y);
        }
    }
}
