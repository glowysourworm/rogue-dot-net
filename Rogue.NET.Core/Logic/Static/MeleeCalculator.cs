using Rogue.NET.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Logic.Static
{
    public static class MeleeCalculator
    {
        /// <summary>
        /// Calculates an attack value based on calculated equipment attack value and total character strength.
        /// </summary>
        /// <param name="equipmentAttackValue">Attack value calculated from equipment</param>
        /// <param name="strength">Total Character Strength</param>
        /// <returns>Attack Value</returns>
        public static double GetAttackValue(double equipmentAttackValue, double strength)
        {
            return (1 + equipmentAttackValue) * strength * ModelConstants.Melee.AttackBaseMultiplier;
        }

        public static double GetDefenseValue(double equipmentDefenseValue, double strength)
        {
            return (1 + equipmentDefenseValue) * strength * ModelConstants.Melee.DefenseBaseMultiplier;
        }
    }
}
