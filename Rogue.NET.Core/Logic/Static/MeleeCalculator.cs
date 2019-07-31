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
        /// Calculates an attack value based on calculated equipment attack value and total character attribute.
        /// </summary>
        /// <param name="equipmentAttackValue">Attack value calculated from equipment</param>
        /// <param name="attribute">Total Character Attribute (Strength, Agility, or Intelligence)</param>
        /// <returns>Attack Value</returns>
        public static double GetAttackValue(double equipmentAttackValue, double attribute)
        {
            return (1 + equipmentAttackValue) * attribute * ModelConstants.Melee.AttackBaseMultiplier;
        }

        public static double GetDefenseValue(double equipmentDefenseValue, double attribute)
        {
            return (1 + equipmentDefenseValue) * attribute * ModelConstants.Melee.DefenseBaseMultiplier;
        }
    }
}
