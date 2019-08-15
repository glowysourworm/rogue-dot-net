using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System.Windows;

namespace Rogue.NET.ScenarioEditor.Events
{
    public class LoadAlterationEffectResponseEventArgs : System.EventArgs
    {
        /// <summary>
        /// The type of alteration effect to be constructed for the view
        /// </summary>
        public IAlterationEffectTemplateViewModel AlterationEffect { get; set; }
    }

    /// <summary>
    /// Event that is fired when new data context (alteation effect) needs to be returned to listeners.
    /// 
    /// NOTE*** This design was chosen just for alteration effects because of view switching. The container
    ///         can be aware of the switch using routed events instead of the main controller publishing
    ///         the alteration effect update - which would require some more complicated lookup using reflection.
    /// </summary>
    public class LoadAlterationEffectResponseEvent : RogueRegionEvent<LoadAlterationEffectResponseEventArgs>
    {
    }
}
