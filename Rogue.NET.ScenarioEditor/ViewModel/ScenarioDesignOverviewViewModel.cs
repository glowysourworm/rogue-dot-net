using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.Difficulty;
using Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Collections.Generic;
using System.Windows.Input;
using Prism.Commands;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.Core.Processing.Model.Validation.Interface;
using Rogue.NET.Core.Processing.Service.Interface;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    [Export(typeof(IScenarioDifficultyViewModel))]
    public class ScenarioDesignOverviewViewModel : NotifyViewModel, IScenarioDifficultyViewModel
    {
        readonly IScenarioDifficultyCalculationService _scenarioDifficultyCalculationService;
        readonly ScenarioConfigurationContainerViewModel _scenarioConfiguration;
        readonly ScenarioConfigurationMapper _scenarioConfigurationMapper;
        readonly IScenarioValidationService _scenarioValidationService;
        readonly IAlterationNameService _alterationNameService;
        readonly IRogueEventAggregator _eventAggregator;

        const string CHART_HUNGER_AVERAGE = "Hunger (Avg)";
        const string CHART_HUNGER_HIGH = "Hunger (High)";
        const string CHART_HUNGER_LOW = "Hunger (Low)";
        const string CHART_PLAYER_ATTACK_POWER_AVERAGE = "Player Attack Power (Avg)";
        const string CHART_PLAYER_ATTACK_POWER_HIGH = "Player Attack Power (High)";
        const string CHART_PLAYER_ATTACK_POWER_LOW = "Player Attack Power (Low)";
        const string CHART_PLAYER_HP_AVERAGE = "Player Hp (Avg)";
        const string CHART_PLAYER_HP_HIGH = "Player Hp (High)";
        const string CHART_PLAYER_HP_LOW = "Player Hp (Low)";
        const string CHART_PLAYER_STRENGTH_AVERAGE = "Player Strength (Avg)";
        const string CHART_PLAYER_STRENGTH_HIGH = "Player Strength (High)";
        const string CHART_PLAYER_STRENGTH_LOW = "Player Strength (Low)";
        const string CHART_PLAYER_LEVEL_AVERAGE = "Player Level (Avg)";
        const string CHART_PLAYER_LEVEL_HIGH = "Player Level (High)";
        const string CHART_PLAYER_LEVEL_LOW = "Player Level (Low)";
        const string CHART_ENEMY_ATTACK_POWER_AVERAGE = "Enemy Attack Power (Avg)";
        const string CHART_ENEMY_ATTACK_POWER_HIGH = "Enemy Attack Power (High)";
        const string CHART_ENEMY_ATTACK_POWER_LOW = "Enemy Attack Power (Low)";
        const string CHART_ENEMY_STRENGTH_AVERAGE = "Enemy Strength (Avg)";
        const string CHART_ENEMY_STRENGTH_HIGH = "Enemy Strength (High)";
        const string CHART_ENEMY_STRENGTH_LOW = "Enemy Strength (Low)";
        const string CHART_ENEMY_HP_AVERAGE = "Enemy Hp (Avg)";
        const string CHART_ENEMY_HP_HIGH = "Enemy Hp (High)";
        const string CHART_ENEMY_HP_LOW = "Enemy Hp (Low)";

        bool _validationPassed = false;
        bool _includeAttackAttributes = false;
        IDifficultyAssetBrowserViewModel _assetBrowserViewModel;

        public bool ValidationPassed
        {
            get { return _validationPassed; }
            set { this.RaiseAndSetIfChanged(ref _validationPassed, value); }
        }
        public bool IncludeAttackAttributes
        {
            get { return _includeAttackAttributes; }
            set { this.RaiseAndSetIfChanged(ref _includeAttackAttributes, value); }
        }
        public IDifficultyAssetBrowserViewModel AssetBrowserViewModel
        {
            get { return _assetBrowserViewModel; }
            set { this.RaiseAndSetIfChanged(ref _assetBrowserViewModel, value); }
        }

        public ICommand CalculateCommand { get; set; }
        public ICommand ValidateCommand { get; set; }

        public SeriesCollection DifficultySeries { get; set; }

        public ObservableCollection<IDifficultyChartViewModel> Charts { get; set; }
        public ObservableCollection<IScenarioValidationMessage> ValidationMessages { get; set; }

        public ScenarioDesignOverviewViewModel(
            ScenarioConfigurationContainerViewModel scenarioConfiguration,
            IScenarioValidationService scenarioValidationService,
            IAlterationNameService alterationNameService,
            IRogueEventAggregator eventAggregator)
        {
            _scenarioDifficultyCalculationService = ServiceLocator.Current.GetInstance<IScenarioDifficultyCalculationService>();
            _scenarioConfiguration = scenarioConfiguration;
            _scenarioConfigurationMapper = new ScenarioConfigurationMapper();
            _scenarioValidationService = scenarioValidationService;
            _alterationNameService = alterationNameService;
            _eventAggregator = eventAggregator;

            this.IncludeAttackAttributes = true;
            this.DifficultySeries = new SeriesCollection();
            this.ValidationMessages = new ObservableCollection<IScenarioValidationMessage>();
            this.AssetBrowserViewModel = new DifficultyAssetBrowserViewModel(scenarioConfiguration);
            this.Charts = new ObservableCollection<IDifficultyChartViewModel>(new List<IDifficultyChartViewModel>()
            {
                new DifficultyChartViewModel() { Title = CHART_HUNGER_AVERAGE, Show = false },
                new DifficultyChartViewModel() { Title = CHART_HUNGER_HIGH, Show = false },
                new DifficultyChartViewModel() { Title = CHART_HUNGER_LOW, Show = false },
                new DifficultyChartViewModel() { Title = CHART_PLAYER_LEVEL_AVERAGE, Show = false },
                new DifficultyChartViewModel() { Title = CHART_PLAYER_LEVEL_HIGH, Show = false },
                new DifficultyChartViewModel() { Title = CHART_PLAYER_LEVEL_LOW, Show = false },
                new DifficultyChartViewModel() { Title = CHART_PLAYER_ATTACK_POWER_AVERAGE, Show = false },
                new DifficultyChartViewModel() { Title = CHART_PLAYER_ATTACK_POWER_HIGH, Show = false },
                new DifficultyChartViewModel() { Title = CHART_PLAYER_ATTACK_POWER_LOW, Show = false },
                new DifficultyChartViewModel() { Title = CHART_PLAYER_HP_AVERAGE, Show = true },
                new DifficultyChartViewModel() { Title = CHART_PLAYER_HP_HIGH, Show = false },
                new DifficultyChartViewModel() { Title = CHART_PLAYER_HP_LOW, Show = false },
                new DifficultyChartViewModel() { Title = CHART_PLAYER_STRENGTH_AVERAGE, Show = true },
                new DifficultyChartViewModel() { Title = CHART_PLAYER_STRENGTH_HIGH, Show = false },
                new DifficultyChartViewModel() { Title = CHART_PLAYER_STRENGTH_LOW, Show = false },
                new DifficultyChartViewModel() { Title = CHART_ENEMY_HP_AVERAGE, Show = false },
                new DifficultyChartViewModel() { Title = CHART_ENEMY_HP_HIGH, Show = false },
                new DifficultyChartViewModel() { Title = CHART_ENEMY_HP_LOW, Show = false },
                new DifficultyChartViewModel() { Title = CHART_ENEMY_STRENGTH_AVERAGE, Show = false },
                new DifficultyChartViewModel() { Title = CHART_ENEMY_STRENGTH_HIGH, Show = false },
                new DifficultyChartViewModel() { Title = CHART_ENEMY_STRENGTH_LOW, Show = false },
                new DifficultyChartViewModel() { Title = CHART_ENEMY_ATTACK_POWER_AVERAGE, Show = false },
                new DifficultyChartViewModel() { Title = CHART_ENEMY_ATTACK_POWER_HIGH, Show = true },
                new DifficultyChartViewModel() { Title = CHART_ENEMY_ATTACK_POWER_LOW, Show = false }
            });

            this.ValidateCommand = new DelegateCommand(Validate);
            this.CalculateCommand = new DelegateCommand(() =>
            {
                CalculateDifficulty();

            }, () => this.ValidationPassed);

            Validate();
            CalculateDifficulty();
        }

        private void CalculateDifficulty()
        {
            // Must pass validation before plotting charts
            if (!this.ValidationPassed)
                return;

            this.DifficultySeries.Clear();
            this.DifficultySeries.AddRange(this.Charts.Where(x => x.Show).Select(x => CreateLineSeries(x.Title, this.IncludeAttackAttributes)));
        }

        private void Validate()
        {
            // Show loading screen because of validation
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Loading
            });

            // SET ALTERATION EFFECT NAMES BEFORE MAPPING (THIS COULD BE REDESIGNED)
            _alterationNameService.Execute(_scenarioConfiguration);

            // MAP BACK CONFIGURATION TO VALIDATE
            var configuration = _scenarioConfigurationMapper.MapBack(_scenarioConfiguration);

            // VALIDATE CONFIGURATION
            this.ValidationMessages.Clear();
            this.ValidationMessages.AddRange(_scenarioValidationService.Validate(configuration));
            this.ValidationPassed = this.ValidationMessages
                                        .Count(x => x.Severity == ValidationMessageSeverity.Error) == 0;

            (this.CalculateCommand as DelegateCommand).RaiseCanExecuteChanged();

            // Hide loading screen
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEventData()
            {
                SplashAction = SplashAction.Hide,
                SplashType = SplashEventType.Loading
            });
        }

        private LineSeries CreateLineSeries(string chartTitle, bool includeAttackAttributes)
        {
            var lineSeries = new LineSeries()
            {
                Title = chartTitle,
                StrokeThickness = 2,
                PointGeometrySize = 12,
                Fill = null
            };

            switch (chartTitle)
            {
                case CHART_HUNGER_AVERAGE:
                    {
                        lineSeries.Stroke = Brushes.GreenYellow;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculateHungerCurve(_scenarioConfiguration, this.AssetBrowserViewModel.Assets)
                                                .Select(x => new ObservablePoint(x.Level, x.Average)));
                    }
                    break;
                case CHART_HUNGER_HIGH:
                    {
                        lineSeries.Stroke = Brushes.GreenYellow;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculateHungerCurve(_scenarioConfiguration, this.AssetBrowserViewModel.Assets)
                                                .Select(x => new ObservablePoint(x.Level, x.High)));
                    }
                    break;
                case CHART_HUNGER_LOW:
                    {
                        lineSeries.Stroke = Brushes.GreenYellow;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculateHungerCurve(_scenarioConfiguration, this.AssetBrowserViewModel.Assets)
                                                .Select(x => new ObservablePoint(x.Level, x.Low)));
                    }
                    break;
                case CHART_ENEMY_ATTACK_POWER_AVERAGE:
                    {
                        lineSeries.Stroke = Brushes.Purple;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculateEnemyAttackPower(_scenarioConfiguration, this.AssetBrowserViewModel.Assets, true, includeAttackAttributes)
                                                .Select(x => new ObservablePoint(x.Level, x.Average)));
                    }
                    break;
                case CHART_ENEMY_ATTACK_POWER_HIGH:
                    {
                        lineSeries.Stroke = Brushes.Purple;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculateEnemyAttackPower(_scenarioConfiguration, this.AssetBrowserViewModel.Assets, true, includeAttackAttributes)
                                                .Select(x => new ObservablePoint(x.Level, x.High)));
                    }
                    break;
                case CHART_ENEMY_ATTACK_POWER_LOW:
                    {
                        lineSeries.Stroke = Brushes.Purple;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculateEnemyAttackPower(_scenarioConfiguration, this.AssetBrowserViewModel.Assets, true, includeAttackAttributes)
                                                .Select(x => new ObservablePoint(x.Level, x.Low)));
                    }
                    break;
                case CHART_ENEMY_HP_AVERAGE:
                    {
                        lineSeries.Stroke = Brushes.MediumVioletRed;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculateEnemyHp(_scenarioConfiguration, this.AssetBrowserViewModel.Assets)
                                                .Select(x => new ObservablePoint(x.Level, x.Average)));
                    }
                    break;
                case CHART_ENEMY_HP_HIGH:
                    {
                        lineSeries.Stroke = Brushes.MediumVioletRed;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculateEnemyHp(_scenarioConfiguration, this.AssetBrowserViewModel.Assets)
                                                .Select(x => new ObservablePoint(x.Level, x.High)));
                    }
                    break;
                case CHART_ENEMY_HP_LOW:
                    {
                        lineSeries.Stroke = Brushes.MediumVioletRed;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculateEnemyHp(_scenarioConfiguration, this.AssetBrowserViewModel.Assets)
                                                .Select(x => new ObservablePoint(x.Level, x.Low)));
                    }
                    break;
                case CHART_ENEMY_STRENGTH_AVERAGE:
                    {
                        lineSeries.Stroke = Brushes.MediumVioletRed;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculateEnemyStrength(_scenarioConfiguration, this.AssetBrowserViewModel.Assets)
                                                .Select(x => new ObservablePoint(x.Level, x.Average)));
                    }
                    break;
                case CHART_ENEMY_STRENGTH_HIGH:
                    {
                        lineSeries.Stroke = Brushes.MediumVioletRed;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculateEnemyStrength(_scenarioConfiguration, this.AssetBrowserViewModel.Assets)
                                                .Select(x => new ObservablePoint(x.Level, x.High)));
                    }
                    break;
                case CHART_ENEMY_STRENGTH_LOW:
                    {
                        lineSeries.Stroke = Brushes.MediumVioletRed;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculateEnemyStrength(_scenarioConfiguration, this.AssetBrowserViewModel.Assets)
                                                .Select(x => new ObservablePoint(x.Level, x.Low)));
                    }
                    break;
                case CHART_PLAYER_ATTACK_POWER_AVERAGE:
                    {
                        lineSeries.Stroke = Brushes.White;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculatePlayerAttackPower(_scenarioConfiguration, this.AssetBrowserViewModel.Assets, true, includeAttackAttributes)
                                                .Select(x => new ObservablePoint(x.Level, x.Average)));
                    }
                    break;
                case CHART_PLAYER_ATTACK_POWER_HIGH:
                    {
                        lineSeries.Stroke = Brushes.White;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculatePlayerAttackPower(_scenarioConfiguration, this.AssetBrowserViewModel.Assets, true, includeAttackAttributes)
                                                .Select(x => new ObservablePoint(x.Level, x.High)));
                    }
                    break;
                case CHART_PLAYER_ATTACK_POWER_LOW:
                    {
                        lineSeries.Stroke = Brushes.White;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculatePlayerAttackPower(_scenarioConfiguration, this.AssetBrowserViewModel.Assets, true, includeAttackAttributes)
                                                .Select(x => new ObservablePoint(x.Level, x.Low)));
                    }
                    break;
                case CHART_PLAYER_HP_AVERAGE:
                    {
                        lineSeries.Stroke = Brushes.Red;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculatePlayerHp(_scenarioConfiguration, this.AssetBrowserViewModel.Assets, true)
                                                .Select(x => new ObservablePoint(x.Level, x.Average)));
                    }
                    break;
                case CHART_PLAYER_HP_HIGH:
                    {
                        lineSeries.Stroke = Brushes.Red;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculatePlayerHp(_scenarioConfiguration, this.AssetBrowserViewModel.Assets, true)
                                                .Select(x => new ObservablePoint(x.Level, x.High)));
                    }
                    break;
                case CHART_PLAYER_HP_LOW:
                    {
                        lineSeries.Stroke = Brushes.Red;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculatePlayerHp(_scenarioConfiguration, this.AssetBrowserViewModel.Assets, true)
                                                .Select(x => new ObservablePoint(x.Level, x.Low)));
                    }
                    break;
                case CHART_PLAYER_STRENGTH_AVERAGE:
                    {
                        lineSeries.Stroke = Brushes.Red;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculatePlayerStrength(_scenarioConfiguration, this.AssetBrowserViewModel.Assets, true)
                                                .Select(x => new ObservablePoint(x.Level, x.Average)));
                    }
                    break;
                case CHART_PLAYER_STRENGTH_HIGH:
                    {
                        lineSeries.Stroke = Brushes.Red;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculatePlayerStrength(_scenarioConfiguration, this.AssetBrowserViewModel.Assets, true)
                                                .Select(x => new ObservablePoint(x.Level, x.High)));
                    }
                    break;
                case CHART_PLAYER_STRENGTH_LOW:
                    {
                        lineSeries.Stroke = Brushes.Red;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculatePlayerStrength(_scenarioConfiguration, this.AssetBrowserViewModel.Assets, true)
                                                .Select(x => new ObservablePoint(x.Level, x.Low)));
                    }
                    break;
                case CHART_PLAYER_LEVEL_AVERAGE:
                    {
                        lineSeries.Stroke = Brushes.Tan;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculatePlayerLevel(_scenarioConfiguration, this.AssetBrowserViewModel.Assets)
                                                .Select(x => new ObservablePoint(x.Level, x.Average)));
                    }
                    break;
                case CHART_PLAYER_LEVEL_HIGH:
                    {
                        lineSeries.Stroke = Brushes.Tan;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculatePlayerLevel(_scenarioConfiguration, this.AssetBrowserViewModel.Assets)
                                                .Select(x => new ObservablePoint(x.Level, x.High)));
                    }
                    break;
                case CHART_PLAYER_LEVEL_LOW:
                    {
                        lineSeries.Stroke = Brushes.Tan;
                        lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
                                                .CalculatePlayerLevel(_scenarioConfiguration, this.AssetBrowserViewModel.Assets)
                                                .Select(x => new ObservablePoint(x.Level, x.Low)));
                    }
                    break;
                default:
                    break;
            }

            return lineSeries;
        }
    }
}
