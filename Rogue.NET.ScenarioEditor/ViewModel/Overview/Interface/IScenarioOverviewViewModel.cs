using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.ViewModel.Overview.Interface
{
    public interface IScenarioOverviewViewModel
    {
        SeriesCollection Series { get; set;  }

        string ChartName { get; set; }

        bool Cummulative { get; set; }

        void SetSeries(string chartName, IProjectionSetViewModel projectionSet);
    }
}
