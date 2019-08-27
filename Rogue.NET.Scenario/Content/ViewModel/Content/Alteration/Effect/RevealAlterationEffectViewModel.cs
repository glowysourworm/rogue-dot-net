using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    public class RevealAlterationEffectViewModel : AlterationEffectViewModel
    {
        AlterationRevealType _type;

        public AlterationRevealType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }

        public RevealAlterationEffectViewModel(RevealAlterationEffect effect) : base(effect)
        {
            this.Type = effect.Type;
        }

        public RevealAlterationEffectViewModel(RevealAlterationEffectTemplate template) : base(template)
        {
            this.Type = template.Type;
        }
    }
}
