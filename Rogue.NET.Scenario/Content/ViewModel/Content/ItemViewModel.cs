using Rogue.NET.Core.Model.Scenario.Content;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class ItemViewModel : ScenarioImageViewModel
    {
        double _weight;

        public double Weight
        {
            get { return _weight; }
            set { this.RaiseAndSetIfChanged(ref _weight, value); }
        }

        public ItemViewModel() { }
        public ItemViewModel(ScenarioObject scenarioObject) : base(scenarioObject)
        {
        }
    }
}
