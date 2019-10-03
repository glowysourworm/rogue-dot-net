using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.Service.Interface;

using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;

using LiveCharts;
using LiveCharts.Wpf;

using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.Overview.Interface;
using System.Windows.Media;
using LiveCharts.Defaults;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.Overview
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioOverviewViewModel))]
    public class ScenarioOverviewViewModel : NotifyViewModel, IScenarioOverviewViewModel
    {
        string _chartName;
        bool _cummulative;
        public string ChartName
        {
            get { return _chartName; }
            set { this.RaiseAndSetIfChanged(ref _chartName, value); }
        }
        public bool Cummulative
        {
            get { return _cummulative; }
            set { this.RaiseAndSetIfChanged(ref _cummulative, value); }
        }
        public SeriesCollection Series { get; set; }

        public ScenarioOverviewViewModel()
        {
            this.Series = new SeriesCollection();
        }

        public void SetSeries(string chartName, IProjectionSetViewModel projectionSet)
        {
            this.ChartName = chartName;

            // TODO: THIS THING FUCKING SUCKS! THERE'S A BUG IN LIVE CHARTS FOR CLEARING THE SERIES COLLECTION.....
            //       ADDED TRY / CATCH TO AVOID A CRASH.
            try
            {
                if (this.Series.Any())
                    this.Series.Clear();
            }
            catch (Exception)
            {
                return;
            }

            var hueLimit = Math.PI * 2.0;

            for (int i=0;i<projectionSet.Count;i++)
            {
                var lineSeries = new LineSeries()
                {
                    Title = projectionSet.GetProjection(i).Key,
                    StrokeThickness = 2,
                    PointGeometrySize = 12,
                    Fill = null
                };

                var projection = projectionSet.GetProjection(i).Value;
                var points = projection.Select(x => new ObservablePoint(x.Level, x.Mean));

                lineSeries.Stroke = BrushFilter.ShiftHSL(Brushes.Red, i * (hueLimit / projectionSet.Count), 0, 0, false);
                lineSeries.Values = new ChartValues<ObservablePoint>(points);

                this.Series.Add(lineSeries);
            }
        }
    }
}
