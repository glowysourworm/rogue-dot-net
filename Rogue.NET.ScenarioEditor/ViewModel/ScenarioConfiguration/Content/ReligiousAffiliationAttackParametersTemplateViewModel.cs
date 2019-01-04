using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class ReligiousAffiliationAttackParametersTemplateViewModel : TemplateViewModel
    {
        string _enemyAffiliationName;
        double _attackMultiplier;
        double _blockMultiplier;
        double _defenseMultiplier;

        public string EnemyAffiliationName
        {
            get { return _enemyAffiliationName; }
            set { this.RaiseAndSetIfChanged(ref _enemyAffiliationName, value); }
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

        public ReligiousAffiliationAttackParametersTemplateViewModel() { }
    }
}
