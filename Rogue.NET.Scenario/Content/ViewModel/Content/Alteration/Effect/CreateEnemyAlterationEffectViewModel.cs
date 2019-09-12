using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Attribute;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect
{
    [UIDisplay(Name = "Create Enemy",
               Description = "Creates an Enemy Character")]
    public class CreateEnemyAlterationEffectViewModel : AlterationEffectViewModel
    {
        AlterationRandomPlacementType _randomPlacementType;
        string _enemyName;
        string _range;

        public AlterationRandomPlacementType RandomPlacementType
        {
            get { return _randomPlacementType; }
            set { this.RaiseAndSetIfChanged(ref _randomPlacementType, value); }
        }
        public string EnemyName
        {
            get { return _enemyName; }
            set { this.RaiseAndSetIfChanged(ref _enemyName, value); }
        }
        public string Range
        {
            get { return _range; }
            set { this.RaiseAndSetIfChanged(ref _range, value); }
        }

        public CreateEnemyAlterationEffectViewModel(CreateEnemyAlterationEffect effect) : base(effect)
        {
            this.RandomPlacementType = effect.RandomPlacementType;
            this.EnemyName = effect.Enemy.Name;
            this.Range = effect.Range.ToString();
        }

        public CreateEnemyAlterationEffectViewModel(CreateEnemyAlterationEffectTemplate template) : base(template)
        {
            this.RandomPlacementType = template.RandomPlacementType;
            this.EnemyName = template.Enemy.Name;
            this.Range = template.Range.ToString();
        }
    }
}
