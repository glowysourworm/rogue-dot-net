using Rogue.NET.Common.Extension.Prism.EventAggregator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.ScenarioEditor.Events
{
    // TODO: This event was made to facilitate loading the alteration effect regions that
    //       exist as sub-control containers for asset controls.
    //
    //       Example:  Consumable -> ConsumableAlterationControl -> "ConsumableAlterationEffectRegion"
    //
    //       The supported types are specified by the marker interfaces on the view model; and
    //       there are several per type. 
    //  
    //       To improve this design the region names could be specified by the UITypeAttribute
    //       or by some other attribute / enum specifier so that they're not being passed around.
    //
    //       Loading of regions I've left purposefully to the ScenarioEditorModule for the time
    //       being to provide control over the data model (because of things like the Undo function,
    //       and generally just good IoC design)

    public class AlterationEffectLoadRequestEventArgs : System.EventArgs
    {
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
    public class AlterationEffectLoadRequestEvent : RogueRegionEvent<AlterationEffectLoadRequestEventArgs>
    {
    }
}
