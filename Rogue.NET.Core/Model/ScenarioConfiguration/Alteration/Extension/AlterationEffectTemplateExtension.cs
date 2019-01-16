using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Extension
{
    public static class AlterationEffectTemplateExtension
    {
        public static IEnumerable<KeyValuePair<string, string>> GetUIAttributes(this AlterationEffectTemplate template)
        {
            var values = new List<KeyValuePair<string, Range<double>>>()
            {
                new KeyValuePair<string, Range<double>>("Hp", template.HpRange),
                new KeyValuePair<string, Range<double>>("Mp", template.MpRange),
                new KeyValuePair<string, Range<double>>("Hp Regen", template.HpPerStepRange),
                new KeyValuePair<string, Range<double>>("Mp Regen", template.MpPerStepRange),
                new KeyValuePair<string, Range<double>>("Strength", template.StrengthRange),
                new KeyValuePair<string, Range<double>>("Agility", template.AgilityRange),
                new KeyValuePair<string, Range<double>>("Intelligence", template.IntelligenceRange),
                new KeyValuePair<string, Range<double>>("Speed", template.SpeedRange),
                new KeyValuePair<string, Range<double>>("Food Usage", template.FoodUsagePerTurnRange),
                new KeyValuePair<string, Range<double>>("Hunger", template.HungerRange),
                new KeyValuePair<string, Range<double>>("Experience", template.ExperienceRange),
                new KeyValuePair<string, Range<double>>("Light Radius", template.AuraRadiusRange),
                new KeyValuePair<string, Range<double>>("Attack", template.AttackRange),
                new KeyValuePair<string, Range<double>>("Defense", template.DefenseRange),
                new KeyValuePair<string, Range<double>>("Critical Hit", template.CriticalHit),
                new KeyValuePair<string, Range<double>>("Dodge", template.DodgeProbabilityRange),
                new KeyValuePair<string, Range<double>>("Magic Block", template.MagicBlockProbabilityRange),
            };

            return values.Where(x => x.Value.IsSet())
                         .Select(x => new KeyValuePair<string, string>(x.Key,

                         x.Value.Low == x.Value.High ? 
                            x.Value.Low.ToString("F2") :
                            x.Value.Low.ToString("F2") + 
                            " to " + 
                            x.Value.High.ToString("F2")));
        }
    }
}
