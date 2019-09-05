using Rogue.NET.Common.ViewModel;

namespace Rogue.NET.Scenario.Content.ViewModel.Dialog
{
    public class PlayerAdvancementViewModel : NotifyViewModel
    {
        int _playerPoints;
        double _strength;
        double _agility;
        double _intelligence;
        int _skillPoints;

        double _newStrength;
        double _newAgility;
        double _newIntelligence;
        int _newSkillPoints;

        public int PlayerPoints
        {
            get { return _playerPoints; }
            set { this.RaiseAndSetIfChanged(ref _playerPoints, value); }
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
    }
}
