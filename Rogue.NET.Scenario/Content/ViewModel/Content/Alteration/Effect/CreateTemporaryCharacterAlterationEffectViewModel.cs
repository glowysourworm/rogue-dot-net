using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Create Temporary Character",
               Description = "Creates an Temporary Character")]
    public class CreateTemporaryCharacterAlterationEffectViewModel : AlterationEffectViewModel
    {
        AlterationRandomPlacementType _randomPlacementType;
        string _temporaryCharacterName;
        string _range;

        public AlterationRandomPlacementType RandomPlacementType
        {
            get { return _randomPlacementType; }
            set { this.RaiseAndSetIfChanged(ref _randomPlacementType, value); }
        }
        public string TemporaryCharacterName
        {
            get { return _temporaryCharacterName; }
            set { this.RaiseAndSetIfChanged(ref _temporaryCharacterName, value); }
        }
        public string Range
        {
            get { return _range; }
            set { this.RaiseAndSetIfChanged(ref _range, value); }
        }

        public CreateTemporaryCharacterAlterationEffectViewModel(CreateTemporaryCharacterAlterationEffect effect) : base(effect)
        {
            this.RandomPlacementType = effect.RandomPlacementType;
            this.TemporaryCharacterName = effect.TemporaryCharacter.Name;
            this.Range = effect.Range.ToString();
        }

        public CreateTemporaryCharacterAlterationEffectViewModel(CreateTemporaryCharacterAlterationEffectTemplate template) : base(template)
        {
            this.RandomPlacementType = template.RandomPlacementType;
            this.TemporaryCharacterName = template.TemporaryCharacter.Name;
            this.Range = template.Range.ToString();
        }
    }
}
