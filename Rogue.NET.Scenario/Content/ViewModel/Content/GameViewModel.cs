using Prism.Events;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Events.Content;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public class GameViewModel : NotifyViewModel
    {
        int _ticks;
        int _levelNumber;
        int _seed;
        bool _isObjectiveAcheived;
        bool _isSurvivorMode;
        string _scenarioName;
        string _scenarioDescription;
        ScenarioImageViewModel _playerScenarioImage;

        public string ScenarioName
        {
            get { return _scenarioName; }
            set { this.RaiseAndSetIfChanged(ref _scenarioName, value); }
        }
        public string ScenarioDescription
        {
            get { return _scenarioDescription; }
            set { this.RaiseAndSetIfChanged(ref _scenarioDescription, value); }
        }
        public ScenarioImageViewModel PlayerScenarioImage
        {
            get { return _playerScenarioImage; }
            set { this.RaiseAndSetIfChanged(ref _playerScenarioImage, value); }
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

        public ObservableCollection<ObjectiveViewModel> Objectives { get; set; }

        [ImportingConstructor]
        public GameViewModel(IEventAggregator eventAggregator, IModelService modelService, IScenarioResourceService scenarioResourceService)
        {
            this.Objectives = new ObservableCollection<ObjectiveViewModel>();

            // Have to wait to use IModelService until the level is loaded (TODO: Remove IModelService from this implementation)
            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                this.ScenarioName = modelService.ScenarioConfiguration.DungeonTemplate.Name;
                this.ScenarioDescription = modelService.ScenarioConfiguration.DungeonTemplate.ObjectiveDescription;
                this.PlayerScenarioImage = new ScenarioImageViewModel(modelService.Player);
            });

            // TODO: Think of a way to decouple this better to hide the IModelService
            eventAggregator.GetEvent<GameUpdateEvent>().Subscribe(e =>
            {
                this.Seed = e.Seed;
                this.Ticks = e.Statistics.Ticks;
                this.LevelNumber = e.LevelNumber;
                this.IsObjectiveAcheived = e.IsObjectiveAcheived;
                this.IsSurvivorMode = e.IsSurvivorMode;

                // Update / Add objectives
                foreach (var keyValuePair in e.ScenarioObjectiveUpdates)
                {
                    var metaData = modelService.ScenarioEncyclopedia[keyValuePair.Key];

                    // Add
                    if (!this.Objectives.Any(x => x.RogueName == keyValuePair.Key))
                        this.Objectives.Add(new ObjectiveViewModel(metaData, scenarioResourceService)
                        {
                            IsCompleted = keyValuePair.Value
                        });

                    // Update
                    else
                        this.Objectives.First(x => x.RogueName == keyValuePair.Key).IsCompleted = keyValuePair.Value;
                }
            });
        }
    }
}
