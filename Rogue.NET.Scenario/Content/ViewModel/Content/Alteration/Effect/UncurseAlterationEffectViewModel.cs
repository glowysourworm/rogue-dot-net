using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Uncurse",
               Description = "Effect that removes the curse from an item in your inventory")]
    public class UncurseAlterationEffectViewModel : AlterationEffectViewModel
    {
        bool _uncurseAll;

        public bool UncurseAll
        {
            get { return _uncurseAll; }
            set { this.RaiseAndSetIfChanged(ref _uncurseAll, value); }
        }

        public UncurseAlterationEffectViewModel(UncurseAlterationEffect effect) : base(effect)
        {
            this.UncurseAll = effect.UncurseAll;
        }

        public UncurseAlterationEffectViewModel(UncurseAlterationEffectTemplate template) : base(template)
        {
            this.UncurseAll = template.UncurseAll;
        }
    }
}
