using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Extension
{
    public static class AlterationCostExtension
    {
        /// <summary>
        /// Returns character-applied AlterationCost attributes as a key/value pair collection
        /// </summary>
        /// <returns>UI-Formatted list of attributes</returns>
        public static IDictionary<string, double> GetUIAttributes(this AlterationCost cost)
        {
            var result = new Dictionary<string, double>()
            {
                {"Hp", cost.Hp },
                { "Mp", cost.Mp },
                { "Strength", cost.Strength },
                { "Agility", cost.Agility },
                { "Intelligence", cost.Intelligence },
                { "Speed", cost.Speed },
                { "Experience", cost.Experience },
                { "Hunger", cost.Hunger },
                { "Food Usage", cost.FoodUsagePerTurn },
                { "Light Radius", cost.LightRadius }
            };

            result.Filter(x => x.Value != 0D);

            return result;
        }
    }
}
