using Rogue.NET.Core.Model.Scenario.Content;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class ItemViewModel : ScenarioObjectViewModel
    {
        double _weight;

        public double Weight
        {
            get { return _weight; }
            set { this.RaiseAndSetIfChanged(ref _weight, value); }
        }

        public ItemViewModel(ScenarioObject scenarioObject) : base(scenarioObject)
        {
        }
    }
}
