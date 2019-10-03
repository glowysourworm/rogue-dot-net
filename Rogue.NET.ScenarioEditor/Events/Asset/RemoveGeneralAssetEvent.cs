using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.Events.Asset
{
    /// <summary>
    /// Event class to specify adding a shared asset to the Scenario Configuration. This 
    /// would be Attack Attributes, Altered Character States, Character Classes, or Brushes
    /// </summary>
    public class RemoveGeneralAssetEvent : RogueEvent<TemplateViewModel>
    {
    }
}
