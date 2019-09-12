using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Teleport",
               Description = "Transports a source character to a random location")]
    public class TeleportManualAlterationEffectViewModel : AlterationEffectViewModel
    {
        public TeleportManualAlterationEffectViewModel(TeleportManualAlterationEffect effect) : base(effect)
        {
        }

        public TeleportManualAlterationEffectViewModel(TeleportManualAlterationEffectTemplate template) : base(template)
        {
        }
    }
}
