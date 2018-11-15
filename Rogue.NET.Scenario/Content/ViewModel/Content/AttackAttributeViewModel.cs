using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class AttackAttributeViewModel : RogueBaseViewModel
    {
        string _characterSymbol;
        string _characterColor;
        ImageResources _icon;
        SmileyMoods _smileyMood;
        string _smileyBodyColor;
        string _smileyLineColor;
        string _smileyAuraColor;
        SymbolTypes _symbolType;

        double _attack;
        double _resistance;
        int _weakness;

        public AttackAttributeViewModel(AttackAttribute attackAttribute) : base(attackAttribute)
        {
            this.Attack = attackAttribute.Attack;
            this.Resistance = attackAttribute.Resistance;
            this.Weakness = attackAttribute.Weakness;

            this.CharacterSymbol = attackAttribute.CharacterSymbol;
            this.CharacterColor = attackAttribute.CharacterColor;
            this.Icon = attackAttribute.Icon;
            this.SmileyMood = attackAttribute.SmileyMood;
            this.SmileyBodyColor = attackAttribute.SmileyBodyColor;
            this.SmileyLineColor = attackAttribute.SmileyLineColor;
            this.SmileyAuraColor = attackAttribute.SmileyAuraColor;
            this.SymbolType = attackAttribute.SymbolType;
        }

        public string CharacterSymbol
        {
            get { return _characterSymbol; }
            set { this.RaiseAndSetIfChanged(ref _characterSymbol, value); }
        }
        public string CharacterColor
        {
            get { return _characterColor; }
            set { this.RaiseAndSetIfChanged(ref _characterColor, value); }
        }
        public ImageResources Icon
        {
            get { return _icon; }
            set { this.RaiseAndSetIfChanged(ref _icon, value); }
        }
        public SmileyMoods SmileyMood
        {
            get { return _smileyMood; }
            set { this.RaiseAndSetIfChanged(ref _smileyMood, value); }
        }
        public string SmileyBodyColor
        {
            get { return _smileyBodyColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyBodyColor, value); }
        }
        public string SmileyLineColor
        {
            get { return _smileyLineColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyLineColor, value); }
        }
        public string SmileyAuraColor
        {
            get { return _smileyAuraColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyAuraColor, value); }
        }
        public SymbolTypes SymbolType
        {
            get { return _symbolType; }
            set { this.RaiseAndSetIfChanged(ref _symbolType, value); }
        }

        public double Attack
        {
            get { return _attack; }
            set { this.RaiseAndSetIfChanged(ref _attack, value); }
        }
        public double Resistance
        {
            get { return _resistance; }
            set { this.RaiseAndSetIfChanged(ref _resistance, value); }
        }
        public int Weakness
        {
            get { return _weakness; }
            set { this.RaiseAndSetIfChanged(ref _weakness, value); }
        }
    }
}
