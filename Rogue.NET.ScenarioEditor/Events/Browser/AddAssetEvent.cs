using Prism.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.ScenarioEditor.Events.Browser
{
    public class AddAssetEventArgs : System.EventArgs
    {
        public Type AssetType { get; set; }
        public string AssetUniqueName { get; set; }
    }

    // Add Asset <Asset Type>
    public class AddAssetEvent : RogueEvent<AddAssetEventArgs>
    {
    }
}
