using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface
{
    /// <summary>
    /// Base interface for the *AlterationEffectTemplate interfaces. This is for convenience when
    /// processing. THESE ARE FOR MARKING ONLY. TRY NOT TO ADD ANY PROPERTIES OR METHODS.
    /// </summary>
    public interface IAlterationEffectTemplate
    {
        /// <summary>
        /// PROPERTY INHERITED FROM TEMPLATE - FOR CONVENIENCE ONLY
        /// </summary>
        string Name { get; }

        /// <summary>
        /// PROPERTY INHERITED FROM TEMPLATE - FOR CONVENIENCE ONLY
        /// </summary>
        string Guid { get; }
    }
}
