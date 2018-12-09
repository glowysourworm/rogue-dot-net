using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Extension
{
    public static class AlterationEffectExtension
    {
        /// <summary>
        /// (TODO - Move this) Returns character-applied AlterationCost attributes as a key/value pair collection
        /// </summary>
        /// <returns>UI-Formatted list of attributes</returns>
        public static IEnumerable<KeyValuePair<string, double>> GetUIAttributes(this AlterationEffect effect)
        {
            return new List<KeyValuePair<string, double>>()
            {
                // Understood to be per-step
                new KeyValuePair<string, double>("Hp", effect.HpPerStep),
                new KeyValuePair<string, double>("Mp", effect.MpPerStep),
                new KeyValuePair<string, double>("Strength", effect.Strength),
                new KeyValuePair<string, double>("Agility", effect.Agility),
                new KeyValuePair<string, double>("Intelligence", effect.Intelligence),
                new KeyValuePair<string, double>("Speed", effect.Speed),

                new KeyValuePair<string, double>("Attack", effect.Attack),
                new KeyValuePair<string, double>("Defense", effect.Defense),
                new KeyValuePair<string, double>("Critical Hit", effect.CriticalHit),
                new KeyValuePair<string, double>("Dodge", effect.DodgeProbability),
                new KeyValuePair<string, double>("Magic Block", effect.MagicBlockProbability),

                new KeyValuePair<string, double>("Food Usage", effect.FoodUsagePerTurn),
                new KeyValuePair<string, double>("Light Radius", effect.AuraRadius)
            }.Where(x => x.Value != 0);
        }
    }
}
