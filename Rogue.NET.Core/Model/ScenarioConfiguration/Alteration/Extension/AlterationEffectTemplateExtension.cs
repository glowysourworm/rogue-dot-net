using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
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
            return effect is OtherAlterationEffectTemplate &&
                  (effect as OtherAlterationEffectTemplate).Type == AlterationOtherEffectType.Identify;
        }

        /// <summary>
        /// Returns true if the IAlterationEffect supports blocking when used with the supplied
        /// alteration type.
        /// </summary>
        /// <returns>True if the alteration should support blocking</returns>
        public static bool GetSupportsBlocking(this IAlterationEffectTemplate template, Type alterationType)
        {
            // Must look up the interface type using the first or default interface for the alteration type
            var interfaceType = alterationType.GetInterfaces().FirstOrDefault();

            return interfaceType != null ? template.GetAttribute<AlterationBlockableAttribute>()
                                                   .GetSupportsBlocking(interfaceType)
                                         : false;
        }

        /// <summary>
        /// Returns true if the IAlterationEffect supports blocking when used with the supplied
        /// alteration type.
        /// </summary>
        /// <returns>True if the alteration should support blocking</returns>
        public static AlterationCostType GetCostType(this IAlterationEffectTemplate template, Type alterationType)
        {
            // Must look up the interface type using the first or default interface for the alteration type
            var interfaceType = alterationType.GetInterfaces().FirstOrDefault();

            return interfaceType != null ? template.GetAttribute<AlterationCostSpecifierAttribute>()
                                                   .GetCostType(interfaceType)
                                         : AlterationCostType.None;
        }
        #endregion
    }
}
