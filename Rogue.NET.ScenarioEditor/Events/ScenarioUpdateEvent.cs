using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;

namespace Rogue.NET.ScenarioEditor.Events
{
    /// <summary>
    /// Occurs when asset collections are changed to provide updated source lists for views
    /// </summary>
    public class ScenarioUpdateEvent : RogueEvent<ScenarioConfigurationContainerViewModel>
    {
    }
}
