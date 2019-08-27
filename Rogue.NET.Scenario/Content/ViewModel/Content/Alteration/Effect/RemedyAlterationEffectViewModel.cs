using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Remedy",
               Description = "Removes all Temporary effects with the specified Altered Character State")]
    public class RemedyAlterationEffectViewModel : AlterationEffectViewModel
    {
        ScenarioImageViewModel _remediedState;

        public ScenarioImageViewModel RemediedState
        {
            get { return _remediedState; }
            set { this.RaiseAndSetIfChanged(ref _remediedState, value); }
        }

        public RemedyAlterationEffectViewModel(RemedyAlterationEffect effect) : base(effect)
        {
            this.RemediedState = new ScenarioImageViewModel(effect.RemediedState);
        }

        public RemedyAlterationEffectViewModel(RemedyAlterationEffectTemplate template) : base(template)
        {
            this.RemediedState = new ScenarioImageViewModel(template.Guid, template.Name, template.RemediedState.SymbolDetails);
        }
    }
}
