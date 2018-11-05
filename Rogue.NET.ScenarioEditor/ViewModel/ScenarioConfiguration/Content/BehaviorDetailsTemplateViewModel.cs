using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using ReactiveUI;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class BehaviorDetailsTemplateViewModel : TemplateViewModel
    {
        private BehaviorTemplateViewModel _primaryBehavior;
        private BehaviorTemplateViewModel _secondaryBehavior;
        private SecondaryBehaviorInvokeReason _secondaryReason;
        private double _secondaryProbability;

        public BehaviorTemplateViewModel PrimaryBehavior
        {
            get { return _primaryBehavior; }
            set { this.RaiseAndSetIfChanged(ref _primaryBehavior, value); }
        }
        public BehaviorTemplateViewModel SecondaryBehavior
        {
            get { return _secondaryBehavior; }
            set { this.RaiseAndSetIfChanged(ref _secondaryBehavior, value); }
        }
        public SecondaryBehaviorInvokeReason SecondaryReason
        {
            get { return _secondaryReason; }
            set { this.RaiseAndSetIfChanged(ref _secondaryReason, value); }
        }
        public double SecondaryProbability
        {
            get { return _secondaryProbability; }
            set { this.RaiseAndSetIfChanged(ref _secondaryProbability, value); }
        }

        public BehaviorDetailsTemplateViewModel()
        {
            this.PrimaryBehavior = new BehaviorTemplateViewModel();
            this.SecondaryBehavior = new BehaviorTemplateViewModel();
        }
    }
}
