using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Other",
               Description = "Choose an effect from a list of one-time effects [Identify, Uncurse]")]
    public class OtherAlterationEffectViewModel : AlterationEffectViewModel
    {
        AlterationOtherEffectType _type;

        public AlterationOtherEffectType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }

        public OtherAlterationEffectViewModel(OtherAlterationEffect effect) : base(effect)
        {
            this.Type = effect.Type;
        }

        public OtherAlterationEffectViewModel(OtherAlterationEffectTemplate template) : base(template)
        {
            this.Type = template.Type;
        }
    }
}
