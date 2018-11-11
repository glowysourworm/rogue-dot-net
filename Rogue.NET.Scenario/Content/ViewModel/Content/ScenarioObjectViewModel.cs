using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Scenario.Content;

namespace Rogue.NET.Scenario.Content.ViewModel.Content
{
    public class ScenarioObjectViewModel : NotifyViewModel
    {
        public string Id { get; protected set; }
        public string RogueName { get; set; }

        public ScenarioObjectViewModel(ScenarioObject scenarioObject)
        {
            this.Id = scenarioObject.Id;
            this.RogueName = scenarioObject.RogueName;
        }
    }
}
