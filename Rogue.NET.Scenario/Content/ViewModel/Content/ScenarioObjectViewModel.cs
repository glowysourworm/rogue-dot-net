using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Scenario.Content;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class ScenarioObjectViewModel : NotifyViewModel
    {
        string _id;
        string _rogueName;
        string _displayName;

        public string Id
        {
            get { return _id; }
            protected set { this.RaiseAndSetIfChanged(ref _id, value); }
        }
        public string RogueName
        {
            get { return _rogueName; }
            protected set { this.RaiseAndSetIfChanged(ref _rogueName, value); }
        }
        public string DisplayName
        {
            get { return _displayName; }
            protected set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }

        public ScenarioObjectViewModel(ScenarioObject scenarioObject)
        {
            this.Id = scenarioObject.Id;
            this.RogueName = scenarioObject.RogueName;
            this.DisplayName = scenarioObject.RogueName;  // TODO - set if identified
        }
    }
}
