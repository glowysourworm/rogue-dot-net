using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;
using System;

namespace Rogue.NET.ScenarioEditor.Events
{
    //       This event was made to facilitate loading the alteration effect regions that
    //       exist as sub-control containers for asset controls.
    //
    //       Example:  Consumable -> ConsumableAlterationControl -> "ConsumableAlterationEffectRegion"
    //
    //       The supported types are specified by the marker interfaces on the view model; and
    //       there are several per type. 
    //
    //       Loading of regions I've left purposefully to the ScenarioEditorModule to provide control 
    //       over the data model (because of things like the Undo function, and generally just good 
    //       IoC design)

    public class LoadNewAlterationEffectEventArgs : System.EventArgs
    {
        /// <summary>
        /// Alteration instance that contains the alteration effect
        /// </summary>
        public AlterationTemplateViewModel Alteration { get; set; }

        /// <summary>
        /// The view type associated with the region
        /// </summary>
        public Type AlterationEffectViewType { get; set; }

        /// <summary>
        /// The type of alteration effect to be constructed for the view
        /// </summary>
        public Type AlterationEffectType { get; set; }
    }

    /// <summary>
    /// An event to trigger the loading of the specified view in the specified region - accompanied
    /// by a confirmation message box - with a return event to carry the newly constructed type to
    /// the listener(s)
    /// </summary>
    public class LoadNewAlterationEffectRequestEvent : RogueRegionEvent<LoadNewAlterationEffectEventArgs>
    {
    }
}
