using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class ReligiousAffiliationAttackParametersTemplateViewModel : TemplateViewModel
    {
        string _enemyReligionName;
        double _attackMultiplier;
        double _blockMultiplier;
        double _defenseMultiplier;

        public string EnemyReligionName
        {
            get { return _enemyReligionName; }
            set { this.RaiseAndSetIfChanged(ref _enemyReligionName, value); }
        }
        public double AttackMultiplier
        {
            get { return _attackMultiplier; }
            set { this.RaiseAndSetIfChanged(ref _attackMultiplier, value); }
        }
        public double BlockMultiplier
        {
            get { return _blockMultiplier; }
            set { this.RaiseAndSetIfChanged(ref _blockMultiplier, value); }
        }
        public double DefenseMultiplier
        {
            get { return _defenseMultiplier; }
            set { this.RaiseAndSetIfChanged(ref _defenseMultiplier, value); }
        }

        public ReligiousAffiliationAttackParametersTemplateViewModel()
        {
            this.EnemyReligionName = "";
        }
    }
}
