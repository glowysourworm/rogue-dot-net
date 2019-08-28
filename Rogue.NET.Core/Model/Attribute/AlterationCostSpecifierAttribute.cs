using Rogue.NET.Core.Model.Enums;

using System;
using System.Linq;

namespace Rogue.NET.Core.Model.Attribute
{
    /// <summary>
    /// (NONE BY DEFAULT) Specifies alteration cost type for the alteration effect based on the alteration interface.
    /// THERE CAN ONLY BE ONE COST TYPE PER:  Alteration Effect + Alteration Container (Doodad, Skill, Consumable Projectile, etc..).
    /// So, there should only ever be one specifier (attribute) for the cost type PER Alteration Effect.
    /// </summary>
    public class AlterationCostSpecifierAttribute : System.Attribute
    {
        readonly Type[] _alterationInterfaceTypes;
        readonly AlterationCostType _costType;

        public AlterationCostType GetCostType(Type alterationEffectInterface)
        {
            return _alterationInterfaceTypes.Contains(alterationEffectInterface) ? _costType : AlterationCostType.None;
        }

        public AlterationCostSpecifierAttribute(AlterationCostType costType, params Type[] alterationInterfaceTypes)
        {
            _costType = costType;
            _alterationInterfaceTypes = alterationInterfaceTypes ?? new Type[] { };
        }
    }
}
