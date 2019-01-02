using Rogue.NET.Core.Model.ScenarioConfiguration.Content;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class ScenarioMetaDataAttackAttributeViewModel : ScenarioImageViewModel
    {
        double _attackLow;
        double _attackHigh;
        double _resistanceLow;
        double _resistanceHigh;

        bool _isAttackSet;
        bool _isResistanceSet;

        public double AttackLow
        {
            get { return _attackLow; }
            set { this.RaiseAndSetIfChanged(ref _attackLow, value); }
        }
        public double AttackHigh
        {
            get { return _attackHigh; }
            set { this.RaiseAndSetIfChanged(ref _attackHigh, value); }
        }
        public double ResistanceLow
        {
            get { return _resistanceLow; }
            set { this.RaiseAndSetIfChanged(ref _resistanceLow, value); }
        }
        public double ResistanceHigh
        {
            get { return _resistanceHigh; }
            set { this.RaiseAndSetIfChanged(ref _resistanceHigh, value); }
        }

        public bool IsAttackSet
        {
            get { return _isAttackSet; }
            set { this.RaiseAndSetIfChanged(ref _isAttackSet, value); }
        }
        public bool IsResistanceSet
        {
            get { return _isResistanceSet; }
            set { this.RaiseAndSetIfChanged(ref _isResistanceSet, value); }
        }

        public ScenarioMetaDataAttackAttributeViewModel(AttackAttributeTemplate template)
        {
            this.AttackLow = template.Attack.Low;
            this.AttackHigh = template.Attack.High;
            this.ResistanceLow = template.Resistance.Low;
            this.ResistanceHigh = template.Resistance.High;

            this.IsAttackSet = template.Attack.IsSet();
            this.IsResistanceSet = template.Resistance.IsSet();

            this.RogueName = template.Name;

            this.CharacterColor = template.SymbolDetails.CharacterColor;
            this.CharacterSymbol = template.SymbolDetails.CharacterSymbol;
            this.Icon = template.SymbolDetails.Icon;
            this.SmileyAuraColor = template.SymbolDetails.SmileyAuraColor;
            this.SmileyBodyColor = template.SymbolDetails.SmileyBodyColor;
            this.SmileyLineColor = template.SymbolDetails.SmileyLineColor;
            this.SmileyMood= template.SymbolDetails.SmileyMood;
            this.SymbolType = template.SymbolDetails.Type;
        }
    }
}
