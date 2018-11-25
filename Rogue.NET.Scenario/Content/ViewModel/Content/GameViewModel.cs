using Prism.Events;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Scenario.Events.Content;
using System.ComponentModel.Composition;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    [Export]
    public class GameViewModel : NotifyViewModel
    {
        int _ticks;
        int _levelNumber;
        int _seed;
        bool _isObjectiveAcheived;
        bool _isSurvivorMode;
        string _scenarioName;

        public string ScenarioName
        {
            get { return _scenarioName; }
            set { this.RaiseAndSetIfChanged(ref _scenarioName, value); }
        }
        public int Ticks
        {
            get { return _ticks; }
            set { this.RaiseAndSetIfChanged(ref _ticks, value); }
        }
        public int LevelNumber
        {
            get { return _levelNumber; }
            set { this.RaiseAndSetIfChanged(ref _levelNumber, value); }
        }
        public int Seed
        {
            get { return _seed; }
            set { this.RaiseAndSetIfChanged(ref _seed, value); }
        }
        public bool IsObjectiveAcheived
        {
            get { return _isObjectiveAcheived; }
            set { this.RaiseAndSetIfChanged(ref _isObjectiveAcheived, value); }
        }
        public bool IsSurvivorMode
        {
            get { return _isSurvivorMode; }
            set { this.RaiseAndSetIfChanged(ref _isSurvivorMode, value); }
        }

        [ImportingConstructor]
        public GameViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<GameUpdateEvent>().Subscribe(e =>
            {
                this.Ticks = e.Statistics.Ticks;
                this.LevelNumber = e.LevelNumber;
                this.Seed = e.Seed;
                this.IsObjectiveAcheived = e.IsObjectiveAcheived;
                this.IsSurvivorMode = e.IsSurvivorMode;
                this.ScenarioName = e.ScenarioName;

            });
        }
    }
}
