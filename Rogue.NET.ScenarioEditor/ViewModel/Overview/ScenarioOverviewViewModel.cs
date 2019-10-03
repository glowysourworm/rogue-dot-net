using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.Service.Interface;

using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using LiveCharts;
using LiveCharts.Wpf;

using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.Overview.Interface;

namespace Rogue.NET.ScenarioEditor.ViewModel.Overview
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioOverviewViewModel))]
    public class ScenarioOverviewViewModel : NotifyViewModel, IScenarioOverviewViewModel
    {
        readonly IScenarioOverviewCalculationService _scenarioOverviewCalculationService;
        readonly IRogueEventAggregator _eventAggregator;

        public SeriesCollection Series { get; set; }

        public ScenarioOverviewViewModel(
            IRogueEventAggregator eventAggregator,
            IScenarioOverviewCalculationService scenarioOverviewCalculationService)
        {
            _scenarioOverviewCalculationService = scenarioOverviewCalculationService;
            _eventAggregator = eventAggregator;

            this.Series = new SeriesCollection();

            CalculateDifficulty();
        }

        private void CalculateDifficulty()
        {
            //// Must pass validation before plotting charts
            //if (!this.ValidationPassed)
            //    return;

            //this.DifficultySeries.Clear();
            //this.DifficultySeries.AddRange(this.Charts.Where(x => x.Show).Select(x => CreateLineSeries(x.Title, this.IncludeAttackAttributes)));
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

            //lineSeries.Stroke = Brushes.GreenYellow;
            //lineSeries.Values = new ChartValues<ObservablePoint>(_scenarioDifficultyCalculationService
            //                        .CalculateHungerCurve(_scenarioConfiguration, this.AssetBrowserViewModel.Assets)
            //                        .Select(x => new ObservablePoint(x.Level, x.Average)));

            return lineSeries;
        }
    }
}
