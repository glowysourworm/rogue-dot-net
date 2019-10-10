using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Content.ViewModel.Dialog
{
    public class PlayerAdvancementViewModel : NotifyViewModel
    {
        string _playerName;
        Color _smileyColor;
        Color _smileyLineColor;
        SmileyExpression _smileyExpression;
        int _playerLevel;

        int _playerPoints;
        double _hp;
        double _stamina;
        double _strength;
        double _agility;
        double _intelligence;
        int _skillPoints;

        double _newHp;
        double _newStamina;
        double _newStrength;
        double _newAgility;
        double _newIntelligence;
        int _newSkillPoints;

        double _hpPerPoint;
        double _staminaPerPoint;
        double _strengthPerPoint;
        double _agilityPerPoint;
        double _intelligencePerPoint;
        int _skillPointsPerPoint;

        public string PlayerName
        {
            get { return _playerName; }
            set { this.RaiseAndSetIfChanged(ref _playerName, value); }
        }
        public int PlayerLevel
        {
            get { return _playerLevel; }
            set { this.RaiseAndSetIfChanged(ref _playerLevel, value); }
        }
        public Color SmileyColor
        {
            get { return _smileyColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyColor, value); }
        }
        public Color SmileyLineColor
        {
            get { return _smileyLineColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyLineColor, value); }
        }
        public SmileyExpression SmileyExpression
        {
            get { return _smileyExpression; }
            set { this.RaiseAndSetIfChanged(ref _smileyExpression, value); }
        }

        public int PlayerPoints
        {
            get { return _playerPoints; }
            set { this.RaiseAndSetIfChanged(ref _playerPoints, value); }
        }
        public double Hp
        {
            get { return _hp; }
            set { this.RaiseAndSetIfChanged(ref _hp, value); }
        }
        public double Stamina
        {
            get { return _stamina; }
            set { this.RaiseAndSetIfChanged(ref _stamina, value); }
        }
        public double Strength
        {
            get { return _strength; }
            set { this.RaiseAndSetIfChanged(ref _strength, value); }
        }
        public double Agility
        {
            get { return _agility; }
            set { this.RaiseAndSetIfChanged(ref _agility, value); }
        }
        public double Intelligence
        {
            get { return _intelligence; }
            set { this.RaiseAndSetIfChanged(ref _intelligence, value); }
        }
        public int SkillPoints
        {
            get { return _skillPoints; }
            set { this.RaiseAndSetIfChanged(ref _skillPoints, value); }
        }

        public double NewHp
        {
            get { return _newHp; }
            set { this.RaiseAndSetIfChanged(ref _newHp, value); }
        }
        public double NewStamina
        {
            get { return _newStamina; }
            set { this.RaiseAndSetIfChanged(ref _newStamina, value); }
        }
        public double NewStrength
        {
            get { return _newStrength; }
            set { this.RaiseAndSetIfChanged(ref _newStrength, value); }
        }
        public double NewAgility
        {
            get { return _newAgility; }
            set { this.RaiseAndSetIfChanged(ref _newAgility, value); }
        }
        public double NewIntelligence
        {
            get { return _newIntelligence; }
            set { this.RaiseAndSetIfChanged(ref _newIntelligence, value); }
        }
        public int NewSkillPoints
        {
            get { return _newSkillPoints; }
            set { this.RaiseAndSetIfChanged(ref _newSkillPoints, value); }
        }

        public double HpPerPoint
        {
            get { return _hpPerPoint; }
            set { this.RaiseAndSetIfChanged(ref _hpPerPoint, value); }
        }
        public double StaminaPerPoint
        {
            get { return _staminaPerPoint; }
            set { this.RaiseAndSetIfChanged(ref _staminaPerPoint, value); }
        }
        public double StrengthPerPoint
        {
            get { return _strengthPerPoint; }
            set { this.RaiseAndSetIfChanged(ref _strengthPerPoint, value); }
        }
        public double AgilityPerPoint
        {
            get { return _agilityPerPoint; }
            set { this.RaiseAndSetIfChanged(ref _agilityPerPoint, value); }
        }
        public double IntelligencePerPoint
        {
            get { return _intelligencePerPoint; }
            set { this.RaiseAndSetIfChanged(ref _intelligencePerPoint, value); }
        }
        public int SkillPointsPerPoint
        {
            get { return _skillPointsPerPoint; }
            set { this.RaiseAndSetIfChanged(ref _skillPointsPerPoint, value); }
        }
    }
}
