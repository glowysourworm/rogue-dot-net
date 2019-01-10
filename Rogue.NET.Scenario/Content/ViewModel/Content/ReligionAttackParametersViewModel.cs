
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Religion;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class ReligionAttackParametersViewModel : ScenarioImageViewModel
    {
        bool _isIdentified;
        double _defenseValue;
        double _attackValue;
        double _magicBlockValue;

        public bool IsIdentified
        {
            get { return _isIdentified; }
            set { this.RaiseAndSetIfChanged(ref _isIdentified, value); }
        }
        public double DefenseValue
        {
            get { return _defenseValue; }
            set { this.RaiseAndSetIfChanged(ref _defenseValue, value); }
        }
        public double AttackValue
        {
            get { return _attackValue; }
            set { this.RaiseAndSetIfChanged(ref _attackValue, value); }
        }
        public double MagicBlockValue
        {
            get { return _magicBlockValue; }
            set { this.RaiseAndSetIfChanged(ref _magicBlockValue, value); }
        }

        // Use to copy symbol details over
        public ReligionAttackParametersViewModel(
            ReligiousAffiliationAttackParameters religiousAffiliationAttackParameters, 
            bool isIdentified,
            double affiliationLevel,
            double totalIntelligence,
            Religion religion)
            : base(religion)
        {
            this.IsIdentified = isIdentified;
            this.AttackValue = totalIntelligence * affiliationLevel * religiousAffiliationAttackParameters.AttackMultiplier;
            this.DefenseValue = totalIntelligence * affiliationLevel * religiousAffiliationAttackParameters.DefenseMultiplier;
            this.MagicBlockValue = totalIntelligence * affiliationLevel * religiousAffiliationAttackParameters.BlockMultiplier;
        }
    }
}
