using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Scenario.Content;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class ScenarioObjectViewModel : RogueBaseViewModel
    {
        string _displayName;

        public string DisplayName
        {
            get { return _displayName; }
            protected set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }

        public ScenarioObjectViewModel(ScenarioObject scenarioObject) : base(scenarioObject)
        {
            this.DisplayName = scenarioObject.RogueName;  // TODO - set if identified
        }
    }
}
