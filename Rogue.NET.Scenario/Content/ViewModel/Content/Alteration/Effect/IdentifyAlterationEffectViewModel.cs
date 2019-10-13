using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Identify",
               Description = "Effect that identifies an item in your inventory")]
    public class IdentifyAlterationEffectViewModel : AlterationEffectViewModel
    {
        bool _identifyAll;

        public bool IdentifyAll
        {
            get { return _identifyAll; }
            set { this.RaiseAndSetIfChanged(ref _identifyAll, value); }
        }

        public IdentifyAlterationEffectViewModel(IdentifyAlterationEffect effect) : base(effect)
        {
            this.IdentifyAll = effect.IdentifyAll;
        }

        public IdentifyAlterationEffectViewModel(IdentifyAlterationEffectTemplate template) : base(template)
        {
            this.IdentifyAll = template.IdentifyAll;
        }
    }
}
