using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Change Level",
               Description = "Changes the scenario level by a specified amount")]
    public class ChangeLevelAlterationEffectViewModel : AlterationEffectViewModel
    {
        string _levelChange;

        public string LevelChange
        {
            get { return _levelChange; }
            set { this.RaiseAndSetIfChanged(ref _levelChange, value); }
        }

        public ChangeLevelAlterationEffectViewModel(ChangeLevelAlterationEffect effect) : base(effect)
        {
            this.LevelChange = effect.LevelChange.ToString();
        }

        public ChangeLevelAlterationEffectViewModel(ChangeLevelAlterationEffectTemplate template) : base(template)
        {
            this.LevelChange = template.LevelChange.ToString();
        }
    }
}
