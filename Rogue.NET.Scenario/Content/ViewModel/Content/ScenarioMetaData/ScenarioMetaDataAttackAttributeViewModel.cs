using Rogue.NET.Core.Model.ScenarioConfiguration.Content;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData
{
    public class ScenarioMetaDataAttackAttributeViewModel : ScenarioImageViewModel
    {
        double _attackLow;
        double _attackHigh;
        double _resistanceLow;
        double _resistanceHigh;
        double _weaknessLow;
        double _weaknessHigh;

        bool _isAttackSet;
        bool _isResistanceSet;
        bool _isWeaknessSet;

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
        public double WeaknessLow
        {
            get { return _weaknessLow; }
            set { this.RaiseAndSetIfChanged(ref _weaknessLow, value); }
        }
        public double WeaknessHigh
        {
            get { return _weaknessHigh; }
            set { this.RaiseAndSetIfChanged(ref _weaknessHigh, value); }
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
        public bool IsWeaknessSet
        {
            get { return _isWeaknessSet; }
            set { this.RaiseAndSetIfChanged(ref _isWeaknessSet, value); }
        }
        public ScenarioMetaDataAttackAttributeViewModel(AttackAttributeTemplate template)
        {
            this.AttackLow = template.Attack.Low;
            this.AttackHigh = template.Attack.High;
            this.ResistanceLow = template.Resistance.Low;
            this.ResistanceHigh = template.Resistance.High;
            this.WeaknessHigh = template.Weakness.High;
            this.WeaknessLow = template.Weakness.Low;

            this.IsAttackSet = template.Attack.IsSet();
            this.IsResistanceSet = template.Resistance.IsSet();
            this.IsWeaknessSet = template.Weakness.IsSet();

            this.RogueName = template.Name;

            this.BackgroundColor = template.SymbolDetails.BackgroundColor;
            this.SmileyBodyColor = template.SymbolDetails.SmileyBodyColor;
            this.SmileyLineColor = template.SymbolDetails.SmileyLineColor;
            this.SmileyExpression = template.SymbolDetails.SmileyExpression;
            this.SymbolClampColor = template.SymbolDetails.SymbolClampColor;
            this.SymbolEffectType = template.SymbolDetails.SymbolEffectType;
            this.SymbolHue = template.SymbolDetails.SymbolHue;
            this.SymbolLightness = template.SymbolDetails.SymbolLightness;
            this.SymbolPath = template.SymbolDetails.SymbolPath;
            this.SymbolSaturation = template.SymbolDetails.SymbolSaturation;
            this.SymbolSize = template.SymbolDetails.SymbolSize;
            this.SymbolType = template.SymbolDetails.SymbolType;
        }
    }
}
