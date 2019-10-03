using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.Overview.Interface;

namespace Rogue.NET.ScenarioEditor.ViewModel.Overview
{
    public class ProjectedQuantityViewModel : NotifyViewModel, IProjectedQuantityViewModel
    {
        string _seriesName;
        double _variance;
        double _mean;
        int _level;

        public double Variance
        {
            get { return _variance; }
            set { this.RaiseAndSetIfChanged(ref _variance, value); }
        }
        public double Mean
        {
            get { return _mean; }
            set { this.RaiseAndSetIfChanged(ref _mean, value); }
        }
        public int Level
        {
            get { return _level; }
            set { this.RaiseAndSetIfChanged(ref _level, value); }
        }
        public string SeriesName
        {
            get { return _seriesName; }
            set { this.RaiseAndSetIfChanged(ref _seriesName, value); }
        }
        public ProjectedQuantityViewModel()
        {

        }
    }
}
