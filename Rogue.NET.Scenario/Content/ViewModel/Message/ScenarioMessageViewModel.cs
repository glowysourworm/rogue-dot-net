using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;

namespace Rogue.NET.Scenario.Content.ViewModel.Message
{
    public class ScenarioMessageViewModel : NotifyViewModel
    {
        public ScenarioMessagePriority Priority { get; set; }
    }
}
