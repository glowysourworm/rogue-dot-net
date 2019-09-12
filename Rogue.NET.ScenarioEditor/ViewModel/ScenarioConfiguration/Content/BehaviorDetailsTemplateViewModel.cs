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

        public ObservableCollection<BehaviorTemplateViewModel> Behaviors { get; set; }

        public BehaviorDetailsTemplateViewModel()
        {
            this.Behaviors = new ObservableCollection<BehaviorTemplateViewModel>();
            this.UseRandomizer = false;
            this.RandomizerTurnCount = 1;   // Set to prevent % arithmatic issues
        }
    }
}
