using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface;

namespace Rogue.NET.ScenarioEditor.ViewModel.Difficulty
{
    public class ProjectedQuantityViewModel : NotifyViewModel, IProjectedQuantityViewModel
    {
        double _low;
        double _high;
        double _average;
        int _level;

        public double Low
        {
            get { return _low; }
            set { this.RaiseAndSetIfChanged(ref _low, value); }
        }
        public double High
        {
            get { return _high; }
            set { this.RaiseAndSetIfChanged(ref _high, value); }
        }
        public double Average
        {
            get { return _average; }
            set { this.RaiseAndSetIfChanged(ref _average, value); }
        }
        public int Level
        {
            get { return _level; }
            set { this.RaiseAndSetIfChanged(ref _level, value); }
        }
    }
}
