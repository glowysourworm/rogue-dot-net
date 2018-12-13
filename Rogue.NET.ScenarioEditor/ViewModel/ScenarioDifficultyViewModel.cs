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
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Defaults;
using System.Collections.Generic;
using System.Windows.Input;
using Prism.Commands;
using Rogue.NET.Common.Extension;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    [Export(typeof(IScenarioDifficultyViewModel))]
    public class ScenarioDifficultyViewModel : NotifyViewModel, IScenarioDifficultyViewModel
    {
        readonly IScenarioDifficultyCalculationService _scenarioDifficultyCalculationService;
        readonly ScenarioConfigurationContainerViewModel _scenarioConfiguration;

        const string CHART_HUNGER_AVERAGE = "Hunger (Avg)";
        const string CHART_HUNGER_HIGH = "Hunger (High)";
        const string CHART_HUNGER_LOW = "Hunger (Low)";
        const string CHART_PLAYER_ATTACK_POWER_AVERAGE = "Player Attack Power (Avg)";
        const string CHART_PLAYER_ATTACK_POWER_HIGH = "Player Attack Power (High)";
        const string CHART_PLAYER_ATTACK_POWER_LOW = "Player Attack Power (Low)";
        const string CHART_PLAYER_HP_AVERAGE = "Player Hp (Avg)";
        const string CHART_PLAYER_HP_HIGH = "Player Hp (High)";
        const string CHART_PLAYER_HP_LOW = "Player Hp (Low)";
        const string CHART_PLAYER_LEVEL_AVERAGE = "Player Level (Avg)";
        const string CHART_PLAYER_LEVEL_HIGH = "Player Level (High)";
        const string CHART_PLAYER_LEVEL_LOW = "Player Level (Low)";
        const string CHART_ENEMY_ATTACK_POWER_AVERAGE = "Enemy Attack Power (Avg)";
        const string CHART_ENEMY_ATTACK_POWER_HIGH = "Enemy Attack Power (High)";
        const string CHART_ENEMY_ATTACK_POWER_LOW = "Enemy Attack Power (Low)";
        const string CHART_ENEMY_HP_AVERAGE = "Enemy Hp (Avg)";
        const string CHART_ENEMY_HP_HIGH = "Enemy Hp (High)";
        const string CHART_ENEMY_HP_LOW = "Enemy Hp (Low)";

        bool _includeAttackAttributes = false;
        IDifficultyAssetBrowserViewModel _assetBrowserViewModel;

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

        public SeriesCollection DifficultySeries { get; set; }

        public ObservableCollection<IDifficultyChartViewModel> Charts { get; set; }

        public ScenarioDifficultyViewModel(ScenarioConfigurationContainerViewModel scenarioConfiguration)
        {
            _scenarioDifficultyCalculationService = ServiceLocator.Current.GetInstance<IScenarioDifficultyCalculationService>();
            _scenarioConfiguration = scenarioConfiguration;

            this.DifficultySeries = new SeriesCollection();
            this.AssetBrowserViewModel = new DifficultyAssetBrowserViewModel(scenarioConfiguration);
            this.Charts = new ObservableCollection<IDifficultyChartViewModel>(new List<IDifficultyChartViewModel>()
            {
                new DifficultyChartViewModel() { Title = CHART_HUNGER_AVERAGE, Show = false, CalculateCommand = new DelegateCommand(Initialize) },
                new DifficultyChartViewModel() { Title = CHART_HUNGER_HIGH, Show = false, CalculateCommand = new DelegateCommand(Initialize) },
                new DifficultyChartViewModel() { Title = CHART_HUNGER_LOW, Show = false, CalculateCommand = new DelegateCommand(Initialize) },
                new DifficultyChartViewModel() { Title = CHART_PLAYER_LEVEL_AVERAGE, Show = true, CalculateCommand = new DelegateCommand(Initialize)},
                new DifficultyChartViewModel() { Title = CHART_PLAYER_LEVEL_HIGH, Show = true, CalculateCommand = new DelegateCommand(Initialize)},
                new DifficultyChartViewModel() { Title = CHART_PLAYER_LEVEL_LOW, Show = true, CalculateCommand = new DelegateCommand(Initialize)},
                new DifficultyChartViewModel() { Title = CHART_PLAYER_ATTACK_POWER_AVERAGE, Show = false, CalculateCommand = new DelegateCommand(Initialize)},
                new DifficultyChartViewModel() { Title = CHART_PLAYER_ATTACK_POWER_HIGH, Show = false, CalculateCommand = new DelegateCommand(Initialize)},
                new DifficultyChartViewModel() { Title = CHART_PLAYER_ATTACK_POWER_LOW, Show = false, CalculateCommand = new DelegateCommand(Initialize)},
                new DifficultyChartViewModel() { Title = CHART_PLAYER_HP_AVERAGE, Show = false, CalculateCommand = new DelegateCommand(Initialize)},
                new DifficultyChartViewModel() { Title = CHART_PLAYER_HP_HIGH, Show = false, CalculateCommand = new DelegateCommand(Initialize)},
                new DifficultyChartViewModel() { Title = CHART_PLAYER_HP_LOW, Show = false, CalculateCommand = new DelegateCommand(Initialize)},
                new DifficultyChartViewModel() { Title = CHART_ENEMY_HP_AVERAGE, Show = false, CalculateCommand = new DelegateCommand(Initialize)},
                new DifficultyChartViewModel() { Title = CHART_ENEMY_HP_HIGH, Show = false, CalculateCommand = new DelegateCommand(Initialize)},
                new DifficultyChartViewModel() { Title = CHART_ENEMY_HP_LOW, Show = false, CalculateCommand = new DelegateCommand(Initialize)},
                new DifficultyChartViewModel() { Title = CHART_ENEMY_ATTACK_POWER_AVERAGE, Show = false, CalculateCommand = new DelegateCommand(Initialize)},
                new DifficultyChartViewModel() { Title = CHART_ENEMY_ATTACK_POWER_HIGH, Show = false, CalculateCommand = new DelegateCommand(Initialize)},
                new DifficultyChartViewModel() { Title = CHART_ENEMY_ATTACK_POWER_LOW, Show = false, CalculateCommand = new DelegateCommand(Initialize)}
            });

            // TODO: MOVE THIS TO IDifficultyAssetViewModel
            this.AssetBrowserViewModel.Assets.ForEach(x => x.CalculateCommand = new DelegateCommand(Initialize));

            Initialize();
        }

        private void Initialize()
        {
            // TODO: REPLACE THIS WITH REQUIRED VALIDATION BEFORE USING DIFFICULTY CHART
            if (_scenarioConfiguration.DungeonTemplate.LayoutTemplates.Count <= 0)
                return;

            this.DifficultySeries.Clear();
            this.DifficultySeries.AddRange(this.Charts.Where(x => x.Show).Select(x => CreateLineSeries(x.Title, this.IncludeAttackAttributes)));
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
