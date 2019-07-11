using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using System.Collections.ObjectModel;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class ReligionViewModel : ScenarioImageViewModel
    {
        string _attributeBonus;
        bool _isAffiliated;
        bool _hasAttributeBonus;
        bool _hasAttackAttributeBonus;

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

        public ObservableCollection<AttackAttributeViewModel> AttackAttributeBonus { get; set; }

        public ReligionViewModel()
        {
            this.AttackAttributeBonus = new ObservableCollection<AttackAttributeViewModel>();
        }
    }
}
