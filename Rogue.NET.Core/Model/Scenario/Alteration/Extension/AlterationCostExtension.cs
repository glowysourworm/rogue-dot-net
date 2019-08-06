using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Extension
{
    public static class AlterationCostExtension
    {
        /// <summary>
        /// (TODO - Move this) Returns character-applied AlterationCost attributes as a key/value pair collection
        /// </summary>
        /// <returns>UI-Formatted list of attributes</returns>
        public static IEnumerable<KeyValuePair<string, double>> GetUIAttributes(this AlterationCost cost)
        {
            return new List<KeyValuePair<string, double>>()
            {
                new KeyValuePair<string, double>("Hp", cost.Hp),
                new KeyValuePair<string, double>("Mp", cost.Mp),
                new KeyValuePair<string, double>("Strength", cost.Strength),
                new KeyValuePair<string, double>("Agility", cost.Agility),
                new KeyValuePair<string, double>("Intelligence", cost.Intelligence),
                new KeyValuePair<string, double>("Speed", cost.Speed),
                new KeyValuePair<string, double>("Experience", cost.Experience),
                new KeyValuePair<string, double>("Hunger", cost.Hunger),
                new KeyValuePair<string, double>("Food Usage", cost.FoodUsagePerTurn),
                new KeyValuePair<string, double>("Light Radius", cost.AuraRadius)
            }.Where(x => x.Value != 0);
        }
    }
}
