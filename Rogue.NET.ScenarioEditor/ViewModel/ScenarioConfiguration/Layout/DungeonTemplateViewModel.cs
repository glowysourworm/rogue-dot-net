using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout
{
    public class DungeonTemplateViewModel : TemplateViewModel
    {
        private int _numberOfLevels;
        private double _monsterGenerationBase;
        private double _partyRoomGenerationRate;
        private string _objectiveDescription;

        public int NumberOfLevels
        {
            get { return _numberOfLevels; }
            set { this.RaiseAndSetIfChanged(ref _numberOfLevels, value); }
        }
        public double MonsterGenerationBase
        {
            get { return _monsterGenerationBase; }
            set { this.RaiseAndSetIfChanged(ref _monsterGenerationBase, value); }
        }
        public double PartyRoomGenerationRate
        {
            get { return _partyRoomGenerationRate; }
            set { this.RaiseAndSetIfChanged(ref _partyRoomGenerationRate, value); }
        }
        public string ObjectiveDescription
        {
            get { return _objectiveDescription; }
            set { this.RaiseAndSetIfChanged(ref _objectiveDescription, value); }
        }


        public ObservableCollection<LayoutTemplateViewModel> LayoutTemplates { get; set; }

        public DungeonTemplateViewModel()
        {
            this.LayoutTemplates = new ObservableCollection<LayoutTemplateViewModel>();

            this.NumberOfLevels = 100;
            this.MonsterGenerationBase = 0.01;
            this.PartyRoomGenerationRate = 0.1;
        }
    }
}
