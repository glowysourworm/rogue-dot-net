using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Create Friendly",
               Description = "Creates an Friendly Character")]
    public class CreateFriendlyAlterationEffectViewModel : AlterationEffectViewModel
    {
        AlterationRandomPlacementType _randomPlacementType;
        string _friendlyName;
        string _range;

        public AlterationRandomPlacementType RandomPlacementType
        {
            get { return _randomPlacementType; }
            set { this.RaiseAndSetIfChanged(ref _randomPlacementType, value); }
        }
        public string FriendlyName
        {
            get { return _friendlyName; }
            set { this.RaiseAndSetIfChanged(ref _friendlyName, value); }
        }
        public string Range
        {
            get { return _range; }
            set { this.RaiseAndSetIfChanged(ref _range, value); }
        }

        public CreateFriendlyAlterationEffectViewModel(CreateFriendlyAlterationEffect effect) : base(effect)
        {
            this.RandomPlacementType = effect.RandomPlacementType;
            this.FriendlyName = effect.Friendly.Name;
            this.Range = effect.Range.ToString();
        }

        public CreateFriendlyAlterationEffectViewModel(CreateFriendlyAlterationEffectTemplate template) : base(template)
        {
            this.RandomPlacementType = template.RandomPlacementType;
            this.FriendlyName = template.Friendly.Name;
            this.Range = template.Range.ToString();
        }
    }
}
