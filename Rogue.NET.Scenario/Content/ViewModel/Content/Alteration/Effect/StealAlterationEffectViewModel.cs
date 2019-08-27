using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Steal",
               Description = "Allows a source character to steal a random item from a target character")]
    public class StealAlterationEffectViewModel : AlterationEffectViewModel
    {
        public StealAlterationEffectViewModel(StealAlterationEffect effect) : base(effect)
        {

        }
        public StealAlterationEffectViewModel(StealAlterationEffectTemplate template) : base(template)
        {

        }
    }
}
