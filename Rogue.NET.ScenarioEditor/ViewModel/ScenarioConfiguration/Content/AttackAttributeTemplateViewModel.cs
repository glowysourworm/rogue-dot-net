using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class AttackAttributeTemplateViewModel : DungeonObjectTemplateViewModel
    {
        private RangeViewModel<double> _attack;
        private RangeViewModel<double> _resistance;
        private RangeViewModel<int> _weakness;

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
        public RangeViewModel<int> Weakness
        {
            get { return _weakness; }
            set { this.RaiseAndSetIfChanged(ref _weakness, value); }
        }

        public AttackAttributeTemplateViewModel()
        {
            this.Attack = new RangeViewModel<double>(0, 0, 0, 5000);
            this.Resistance = new RangeViewModel<double>(0, 0, 0, 5000);
            this.Weakness = new RangeViewModel<int>(0, 0, 0, 10);
        }
    }
}
