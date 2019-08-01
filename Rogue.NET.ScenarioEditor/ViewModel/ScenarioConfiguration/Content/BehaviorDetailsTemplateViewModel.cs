using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class BehaviorDetailsTemplateViewModel : TemplateViewModel
    {
        private bool _canOpenDoors;
        private bool _useRandomizer;
        private int _randomizerTurnCount;
        private double _engageRadius;
        private double _disengageRadius;
        private double _criticalRatio;
        private double _counterAttackProbability;

        public bool CanOpenDoors
        {
            get { return _canOpenDoors; }
            set { this.RaiseAndSetIfChanged(ref _canOpenDoors, value); }
        }
        public bool UseRandomizer
        {
            get { return _useRandomizer; }
            set { this.RaiseAndSetIfChanged(ref _useRandomizer, value); }
        }
        public int RandomizerTurnCount
        {
            get { return _randomizerTurnCount; }
            set { this.RaiseAndSetIfChanged(ref _randomizerTurnCount, value); }
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

        public ObservableCollection<BehaviorTemplateViewModel> Behaviors { get; set; }

        public BehaviorDetailsTemplateViewModel()
        {
            this.Behaviors = new ObservableCollection<BehaviorTemplateViewModel>();
            this.UseRandomizer = false;
        }
    }
}
