using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Extension
{
    public static class AlterationCostTemplateExtension
    {
        /// <summary>
        /// Creates UI-prepared strings to show values for all character-applied attributes
        /// </summary>
        public static IEnumerable<KeyValuePair<string, double>> GetUIAttributes(this AlterationCostTemplate template)
        {
            var values = new List<KeyValuePair<string, double>>()
            {
                new KeyValuePair<string, double>("Hp", template.Hp),
                new KeyValuePair<string, double>("Mp", template.Mp),
                new KeyValuePair<string, double>("Hp Regen", template.HpPerStep),
                new KeyValuePair<string, double>("Mp Regen", template.MpPerStep),
                new KeyValuePair<string, double>("Strength", template.Strength),
                new KeyValuePair<string, double>("Agility", template.Agility),
                new KeyValuePair<string, double>("Intelligence", template.Intelligence),
                new KeyValuePair<string, double>("Speed", template.Speed),
                new KeyValuePair<string, double>("Food Usage", template.FoodUsagePerTurn),
                new KeyValuePair<string, double>("Hunger", template.Hunger),
                new KeyValuePair<string, double>("Experience", template.Experience),
                new KeyValuePair<string, double>("Light Radius", template.AuraRadius)
            };

            return values.Where(x => x.Value != 0);
        }
    }
}
