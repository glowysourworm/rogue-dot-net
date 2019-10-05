using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.Views;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    [UIType(DisplayName = "Attack Attribute",
            Description = "Special combat attribute created for this scenario", 
            ViewType = typeof(EditorInstructions))]
    public class AttackAttributeTemplateViewModel : DungeonObjectTemplateViewModel
    {
        private RangeViewModel<double> _attack;
        private RangeViewModel<double> _resistance;
        private RangeViewModel<int> _weakness;
        private bool _immune;

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
        public bool Immune
        {
            get { return _immune; }
            set { this.RaiseAndSetIfChanged(ref _immune, value); }
        }

        public AttackAttributeTemplateViewModel()
        {
            this.Attack = new RangeViewModel<double>(0, 0);
            this.Resistance = new RangeViewModel<double>(0, 0);
            this.Weakness = new RangeViewModel<int>(0, 0);
            this.Immune = false;
        }
    }
}
