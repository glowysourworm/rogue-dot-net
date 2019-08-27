using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Create Monster",
               Description = "Creates an Enemy Character")]
    public class CreateMonsterAlterationEffectViewModel : AlterationEffectViewModel
    {
        AlterationRandomPlacementType _randomPlacementType;
        string _createMonsterEnemy;
        string _range;

        public AlterationRandomPlacementType RandomPlacementType
        {
            get { return _randomPlacementType; }
            set { this.RaiseAndSetIfChanged(ref _randomPlacementType, value); }
        }
        public string CreateMonsterEnemy
        {
            get { return _createMonsterEnemy; }
            set { this.RaiseAndSetIfChanged(ref _createMonsterEnemy, value); }
        }
        public string Range
        {
            get { return _range; }
            set { this.RaiseAndSetIfChanged(ref _range, value); }
        }

        public CreateMonsterAlterationEffectViewModel(CreateMonsterAlterationEffect effect) : base(effect)
        {
            this.RandomPlacementType = effect.RandomPlacementType;
            this.CreateMonsterEnemy = effect.CreateMonsterEnemy;
            this.Range = effect.Range.ToString();
        }

        public CreateMonsterAlterationEffectViewModel(CreateMonsterAlterationEffectTemplate template) : base(template)
        {
            this.RandomPlacementType = template.RandomPlacementType;
            this.CreateMonsterEnemy = template.CreateMonsterEnemy;
            this.Range = template.Range.ToString();
        }
    }
}
