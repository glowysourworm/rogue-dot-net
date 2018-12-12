using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.Difficulty;
using Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Sparrow.Chart;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Media;
using System.Windows;

namespace Rogue.NET.ScenarioEditor.ViewModel
{
    [Export(typeof(IScenarioDifficultyViewModel))]
    public class ScenarioDifficultyViewModel : NotifyViewModel, IScenarioDifficultyViewModel
    {
        readonly IScenarioDifficultyCalculationService _scenarioDifficultyCalculationService;

        int _numberOfLevels;
        double _maxValue;
        IDifficultyAssetBrowserViewModel _assetBrowserViewModel;
      
        public int NumberOfLevels
        {
            get { return _numberOfLevels; }
            set { this.RaiseAndSetIfChanged(ref _numberOfLevels, value); }
        }
        public double MaxValue
        {
            get { return _maxValue; }
            set { this.RaiseAndSetIfChanged(ref _maxValue, value); }
        }
        public IDifficultyAssetBrowserViewModel AssetBrowserViewModel
        {
            get { return _assetBrowserViewModel; }
            set { this.RaiseAndSetIfChanged(ref _assetBrowserViewModel, value); }
        }

        public ObservableCollection<LineSeries> DifficultySeries { get; set; }

        public ScenarioDifficultyViewModel(ScenarioConfigurationContainerViewModel scenarioConfiguration)
        {
            _scenarioDifficultyCalculationService = ServiceLocator.Current.GetInstance<IScenarioDifficultyCalculationService>();

            this.DifficultySeries = new ObservableCollection<LineSeries>();
            this.AssetBrowserViewModel = new DifficultyAssetBrowserViewModel(scenarioConfiguration);

            // TODO: REPLACE THIS WITH REQUIRED VALIDATION BEFORE USING DIFFICULTY CHART
            if (scenarioConfiguration.DungeonTemplate.LayoutTemplates.Count <= 0)
                return;

            // Calculate a test curve
            var projectedExperience = _scenarioDifficultyCalculationService
                                        .CalculatePlayerExperience(scenarioConfiguration, this.AssetBrowserViewModel.Assets);

            this.NumberOfLevels = 50;
            this.MaxValue = projectedExperience.Max(x => x.Average);

            this.DifficultySeries.Add(new LineSeries()
            {
                Label = "Experience",
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                PointsSource = new ObservableCollection<Point>(projectedExperience.Select(x => new Point(x.Level, x.Average))),
                XPath = "X",
                YPath = "Y",
                UseSinglePart = true
            });
        }
    }
}
