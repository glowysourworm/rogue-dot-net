using Rogue.NET.Common.Extension.Prism.EventAggregator;
using System;

namespace Rogue.NET.ScenarioEditor.Events.Asset.Alteration
{
    public class LoadAlterationEffectEventArgs : System.EventArgs
    {
        /// <summary>
        /// The view type associated with the region
        /// </summary>
        public Type AlterationEffectViewType { get; set; }
    }

    /// <summary>
    /// An event to trigger the loading of the specified view in the specified region
    /// </summary>
    public class LoadAlterationEffectRequestEvent : RogueRegionEvent<LoadAlterationEffectEventArgs>
    {
    }
}
