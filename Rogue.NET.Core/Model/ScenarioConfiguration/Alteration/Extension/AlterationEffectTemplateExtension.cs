using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.Static;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Extension
{
    public static class AlterationEffectTemplateExtension
    {
        #region (public) Query Methods
        public static bool IsIdentify(this IAlterationEffectTemplate effect)
        {
            return effect is IdentifyAlterationEffectTemplate;
        }

        /// <summary>
        /// Returns true if the IAlterationEffect supports blocking when used with the supplied
        /// alteration type.
        /// </summary>
        /// <returns>True if the alteration should support blocking</returns>
        public static bool GetSupportsBlocking(this IAlterationEffectTemplate template, AlterationTemplate alteration)
        {
            return AlterationSpecificationContainer.GetSupportsBlocking(alteration, template);
        }

        /// <summary>
        /// Returns true if the IAlterationEffect supports blocking when used with the supplied
        /// alteration type.
        /// </summary>
        /// <returns>True if the alteration should support blocking</returns>
        public static AlterationCostType GetCostType(this IAlterationEffectTemplate template, AlterationTemplate alteration)
        {
            return AlterationSpecificationContainer.GetCostType(alteration, template);
        }
        #endregion
    }
}
