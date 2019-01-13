using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using System.Collections.ObjectModel;

namespace Rogue.NET.Scenario.Content.ViewModel.Content.Religion
{
    public class ReligionViewModel : ScenarioImageViewModel
    {
        double _affiliationLevel;
        string _attributeBonus;
        bool _isAffiliated;
        bool _hasAttributeBonus;
        bool _hasAttackAttributeBonus;

        public double AffiliationLevel
        {
            get { return _affiliationLevel; }
            set { this.RaiseAndSetIfChanged(ref _affiliationLevel, value); }
        }
        public string AttributeBonus
        {
            get { return _attributeBonus; }
            set { this.RaiseAndSetIfChanged(ref _attributeBonus, value); }
        }
        public bool IsAffiliated
        {
            get { return _isAffiliated; }
            set { this.RaiseAndSetIfChanged(ref _isAffiliated, value); }
        }
        public bool HasAttributeBonus
        {
            get { return _hasAttributeBonus; }
            set { this.RaiseAndSetIfChanged(ref _hasAttributeBonus, value); }
        }
        public bool HasAttackAttributeBonus
        {
            get { return _hasAttackAttributeBonus; }
            set { this.RaiseAndSetIfChanged(ref _hasAttackAttributeBonus, value); }
        }

        public ObservableCollection<ReligionAttackParametersViewModel> AttackParameters { get; set; }
        public ObservableCollection<AttackAttributeViewModel> AttackAttributeBonus { get; set; }

        public ReligionViewModel()
        {
            this.AttackParameters = new ObservableCollection<ReligionAttackParametersViewModel>();
            this.AttackAttributeBonus = new ObservableCollection<AttackAttributeViewModel>();
        }
    }
}
