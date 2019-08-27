using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Run Away",
               Description = "Effect that causes an Enemy character to be removed from the scenario")]
    public class RunAwayAlterationEffectViewModel : AlterationEffectViewModel
    {
        public RunAwayAlterationEffectViewModel(RunAwayAlterationEffect effect) : base(effect)
        {

        }

        public RunAwayAlterationEffectViewModel(RunAwayAlterationEffectTemplate template) : base(template)
        {

        }
    }
}
