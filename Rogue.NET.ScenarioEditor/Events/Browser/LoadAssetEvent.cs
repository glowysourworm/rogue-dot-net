using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.Browser.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.Events.Browser
{
    public class LoadAssetEvent : RogueEvent<IScenarioAssetReadonlyViewModel>
    {
    }
}
