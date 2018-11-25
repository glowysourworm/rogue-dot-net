using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class AddAssetEventArgs : System.EventArgs
    {
        public string AssetType { get; set; }
        public string AssetUniqueName { get; set; }
    }

    // Add Asset <Asset Type>
    public class AddAssetEvent : RogueEvent<AddAssetEventArgs>
    {
    }
}
