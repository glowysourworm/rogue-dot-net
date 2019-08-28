using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Attribute
{
    /// <summary>
    /// (FALSE BY DEFAULT) Specifies blockable alteration interface types for the specified alteration effect
    /// </summary>
    public class AlterationBlockableAttribute : System.Attribute
    {
        readonly Type[] _blockableInterfaceTypes;

        public bool GetSupportsBlocking(Type alterationEffectInterface)
        {
            return _blockableInterfaceTypes.Contains(alterationEffectInterface);
        }

        public AlterationBlockableAttribute(params Type[] blockableAlterationEffectInterfaceTypes)
        {
            _blockableInterfaceTypes = blockableAlterationEffectInterfaceTypes ?? new Type[] { };
        }
    }
}
