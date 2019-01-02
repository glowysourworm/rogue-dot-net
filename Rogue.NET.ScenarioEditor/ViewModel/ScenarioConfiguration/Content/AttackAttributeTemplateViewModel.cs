using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class AttackAttributeTemplateViewModel : DungeonObjectTemplateViewModel
    {
        private RangeViewModel<double> _attack;
        private RangeViewModel<double> _resistance;

        private bool _appliesToStrengthBasedCombat;
        private bool _appliesToIntelligenceBasedCombat;
        private bool _scaledByStrength;
        private bool _scaledByIntelligence;

        public RangeViewModel<double> Attack
        {
            get { return _attack; }
            set { this.RaiseAndSetIfChanged(ref _attack, value); }
        }
        public RangeViewModel<double> Resistance
        {
            get { return _resistance; }
            set { this.RaiseAndSetIfChanged(ref _resistance, value); }
        }

        public bool AppliesToStrengthBasedCombat
        {
            get { return _appliesToStrengthBasedCombat; }
            set { this.RaiseAndSetIfChanged(ref _appliesToStrengthBasedCombat, value); }
        }
        public bool AppliesToIntelligenceBasedCombat
        {
            get { return _appliesToIntelligenceBasedCombat; }
            set { this.RaiseAndSetIfChanged(ref _appliesToIntelligenceBasedCombat, value); }
        }

        public bool ScaledByStrength
        {
            get { return _scaledByStrength; }
            set { this.RaiseAndSetIfChanged(ref _scaledByStrength, value); }
        }
        public bool ScaledByIntelligence
        {
            get { return _scaledByIntelligence; }
            set { this.RaiseAndSetIfChanged(ref _scaledByIntelligence, value); }
        }
        public AttackAttributeTemplateViewModel()
        {
            this.Attack = new RangeViewModel<double>(0, 0, 0, 5000);
            this.Resistance = new RangeViewModel<double>(0, 0, 0, 5000);
        }
    }
}
