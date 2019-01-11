
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model;
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

        double _defenseRatio;
        double _attackRatio;
        double _magicBlockRatio;

        double _attackScale;
        Range<double> _defenseScale;
        Range<double> _magicBlockScale;

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
        public double DefenseRatio
        {
            get { return _defenseRatio; }
            set { this.RaiseAndSetIfChanged(ref _defenseRatio, value); }
        }
        public double AttackRatio
        {
            get { return _attackRatio; }
            set { this.RaiseAndSetIfChanged(ref _attackRatio, value); }
        }
        public double MagicBlockRatio
        {
            get { return _magicBlockRatio; }
            set { this.RaiseAndSetIfChanged(ref _magicBlockRatio, value); }
        }
        public Range<double> DefenseScale
        {
            get { return _defenseScale; }
            set { this.RaiseAndSetIfChanged(ref _defenseScale, value); }
        }
        public double AttackMax
        {
            get { return _attackScale; }
            set { this.RaiseAndSetIfChanged(ref _attackScale, value); }
        }
        public Range<double> MagicBlockScale
        {
            get { return _magicBlockScale; }
            set { this.RaiseAndSetIfChanged(ref _magicBlockScale, value); }
        }

        public ReligionAttackParametersViewModel()
        {

        }

        // Use to copy symbol details over
        public ReligionAttackParametersViewModel(
            ReligiousAffiliationAttackParameters attackParameters, 
            ReligiousAffiliationAttackParameters maxParameters,
            bool isIdentified,
            double affiliationLevel,
            double totalIntelligence,
            Religion religion)
            : base(religion)
        {
            this.IsIdentified = isIdentified;
            this.AttackValue = attackParameters.AttackMultiplier * affiliationLevel;
            this.DefenseValue = attackParameters.DefenseMultiplier * affiliationLevel;
            this.MagicBlockValue = attackParameters.BlockMultiplier * affiliationLevel;

            this.AttackRatio = affiliationLevel * attackParameters.AttackMultiplier;
            this.DefenseRatio = affiliationLevel * attackParameters.DefenseMultiplier;
            this.MagicBlockRatio = affiliationLevel * attackParameters.BlockMultiplier;

            this.AttackMax = (affiliationLevel * attackParameters.AttackMultiplier).RoundOrderMagnitudeUp();
            this.DefenseScale = new Range<double>(-1 * (affiliationLevel * attackParameters.DefenseMultiplier).RoundOrderMagnitudeUp(),
                                                       (affiliationLevel * attackParameters.DefenseMultiplier).RoundOrderMagnitudeUp());
            this.MagicBlockScale = new Range<double>(-1 , 1);
        }
    }
}
