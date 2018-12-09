using Rogue.NET.Common.ViewModel;
using System.Collections.ObjectModel;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class AlterationViewModel : NotifyViewModel
    {
        string _displayName;
        string _type;                    // Passive / Temporary / Temporary (Friendly) / Temporary (Malign)

        bool _isAlteredState;
        ScenarioImageViewModel _alteredCharacterState;

        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }
        public string Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public bool IsAlteredState
        {
            get { return _isAlteredState; }
            set { this.RaiseAndSetIfChanged(ref _isAlteredState, value); }
        }
        public ScenarioImageViewModel AlteredCharacterState
        {
            get { return _alteredCharacterState; }
            set { this.RaiseAndSetIfChanged(ref _alteredCharacterState, value); }
        }

        /// <summary>
        /// These are calculated with pre-set display values
        /// </summary>
        public ObservableCollection<AlterationAttributeViewModel> AlterationCostAttributes { get; set; }

        /// <summary>
        /// These are calculated with pre-set display values
        /// </summary>
        public ObservableCollection<AlterationAttributeViewModel> AlterationEffectAttributes { get; set; }

        /// <summary>
        /// Attack Attributes for the alteration effect
        /// </summary>
        public ObservableCollection<AttackAttributeViewModel> AlterationEffectAttackAttributes { get; set; }

        public AlterationViewModel()
        {
            this.AlterationCostAttributes = new ObservableCollection<AlterationAttributeViewModel>();
            this.AlterationEffectAttributes = new ObservableCollection<AlterationAttributeViewModel>();
            this.AlterationEffectAttackAttributes = new ObservableCollection<AttackAttributeViewModel>();

            this.IsAlteredState = false;
        }
    }
}
