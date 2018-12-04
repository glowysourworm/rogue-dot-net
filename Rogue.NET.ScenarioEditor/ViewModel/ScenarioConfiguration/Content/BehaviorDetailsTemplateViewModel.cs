using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class BehaviorDetailsTemplateViewModel : TemplateViewModel
    {
        private BehaviorTemplateViewModel _primaryBehavior;
        private BehaviorTemplateViewModel _secondaryBehavior;
        private SecondaryBehaviorInvokeReason _secondaryReason;
        private double _secondaryProbability;
        private bool _canOpenDoors;
        private double _engageRadius;
        private double _disengageRadius;
        private double _criticalRatio;
        private double _counterAttackProbability;

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
        public bool CanOpenDoors
        {
            get { return _canOpenDoors; }
            set { this.RaiseAndSetIfChanged(ref _canOpenDoors, value); }
        }
        public double EngageRadius
        {
            get { return _engageRadius; }
            set { this.RaiseAndSetIfChanged(ref _engageRadius, value); }
        }
        public double DisengageRadius
        {
            get { return _disengageRadius; }
            set { this.RaiseAndSetIfChanged(ref _disengageRadius, value); }
        }
        public double CriticalRatio
        {
            get { return _criticalRatio; }
            set { this.RaiseAndSetIfChanged(ref _criticalRatio, value); }
        }
        public double CounterAttackProbability
        {
            get { return _counterAttackProbability; }
            set { this.RaiseAndSetIfChanged(ref _counterAttackProbability, value); }
        }

        public BehaviorDetailsTemplateViewModel()
        {
            this.PrimaryBehavior = new BehaviorTemplateViewModel();
            this.SecondaryBehavior = new BehaviorTemplateViewModel();
        }
    }
}
