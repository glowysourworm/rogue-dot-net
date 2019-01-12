using Rogue.NET.Core.Model.Scenario.Content.Religion;
using Rogue.NET.Core.Service.Interface;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class ReligionAttackParametersViewModel : Image, INotifyPropertyChanged
    {
        double _defenseValue;
        double _attackValue;
        double _magicBlockValue;
        string _rogueName;

        public event PropertyChangedEventHandler PropertyChanged;

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
        public string RogueName
        {
            get { return _rogueName; }
            set { this.RaiseAndSetIfChanged(ref _rogueName, value); }
        }

        // Use to copy symbol details over
        public ReligionAttackParametersViewModel(
            ReligiousAffiliationAttackParameters attackParameters, 
            double affiliationLevel,
            Religion religion,
            IScenarioResourceService scenarioResourceService)
        {
            this.AttackValue = attackParameters.AttackMultiplier * affiliationLevel;
            this.DefenseValue = attackParameters.DefenseMultiplier * affiliationLevel;
            this.MagicBlockValue = attackParameters.BlockMultiplier * affiliationLevel * 10;

            this.RogueName = religion.RogueName;

            this.Source = scenarioResourceService.GetImageSource(religion);
        }

        public void Update(ReligiousAffiliationAttackParameters attackParameters, double affiliationLevel)
        {
            this.AttackValue = attackParameters.AttackMultiplier * affiliationLevel;
            this.DefenseValue = attackParameters.DefenseMultiplier * affiliationLevel;
            this.MagicBlockValue = attackParameters.BlockMultiplier * affiliationLevel * 10;
        }

        protected void RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string memberName = "")
        {
            field = value;

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
