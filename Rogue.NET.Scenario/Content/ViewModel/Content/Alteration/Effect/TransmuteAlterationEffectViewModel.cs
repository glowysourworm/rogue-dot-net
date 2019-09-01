using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Transmute",
               Description = "Creates a product item from one-many user selected ingredients")]
    public class TransmuteAlterationEffectViewModel : AlterationEffectViewModel
    {
        string _probabilityOfSuccess;

        public string ProbabilityOfSuccess
        {
            get { return _probabilityOfSuccess; }
            set { this.RaiseAndSetIfChanged(ref _probabilityOfSuccess, value); }
        }

        public TransmuteAlterationEffectViewModel(TransmuteAlterationEffect effect) : base(effect)
        {
            this.ProbabilityOfSuccess = effect.ProbabilityOfSuccess.ToString("F1");
        }

        public TransmuteAlterationEffectViewModel(TransmuteAlterationEffectTemplate template) : base(template)
        {
            this.ProbabilityOfSuccess = template.ProbabilityOfSuccess.ToString("F1");
        }
    }
}
