using System.Windows.Input;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.ScenarioEditor.ViewModel.Difficulty.Interface;

namespace Rogue.NET.ScenarioEditor.ViewModel.Difficulty
{
    public class OverviewChartViewModel : NotifyViewModel, IOverviewChartViewModel
    {
        string _title;
        bool _show;

        public string Title
        {
            get { return _title; }
            set { this.RaiseAndSetIfChanged(ref _title, value); }
        }
        public bool Show
        {
            get { return _show; }
            set { this.RaiseAndSetIfChanged(ref _show, value); }
        }
    }
}
