using Rogue.NET.Common.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Common.Extension
{
    public static class AttackAttributeExtension
    {
        /// <summary>
        /// Returns an attack attribute with the combined properties of both. SET NEW INSTANCE FLAG FOR A NEW CLONED ATTACK ATTRIBUTE
        /// </summary>
        /// <exception cref="ArgumentException">Throws an argument exception if names of attack attributes don't match</exception>
        public static AttackAttribute Add(this AttackAttribute attackAttribute, AttackAttribute sourceAttribute, bool newInstance)
        {
            if (attackAttribute.RogueName != sourceAttribute.RogueName)
                throw new ArgumentException("Attack Attribute names don't match");

            // Clone the first attack attribute
            var result = newInstance ? attackAttribute.DeepClone() : attackAttribute;

            // Add on the other values
            result.Attack += sourceAttribute.Attack;
            result.Resistance += sourceAttribute.Resistance;
            result.Weakness += sourceAttribute.Weakness;
            result.Immune = result.Immune || sourceAttribute.Immune;

            return result;
        }
    }
}
