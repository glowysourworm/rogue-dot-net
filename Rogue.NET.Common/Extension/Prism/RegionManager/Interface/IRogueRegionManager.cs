using System;
using System.Collections.Generic;
using System.Windows;

namespace Rogue.NET.Common.Extension.Prism.RegionManager.Interface
{
    /// <summary>
    /// Replacement for IRegionManager for the following reasons:
    /// 
    /// 1) Can't support multiple instances of the same Region / UI branch
    /// 
    /// 2) Race condition in the transition presenter somewhere in the libray
    /// 
    /// 3) Want to simplify the implementation and make it specific to region 
    ///    INSTANCE rather than region NAME
    ///    
    /// Using Scoped IRegionManager may have worked if the NEW IRegionManager
    /// was known to the INSTANCE of the control where it was being used. This
    /// would've been essentially the UserControl that had the ContentControl
    /// with the registered prism:RegionManager property.
    /// 
    /// This solution would've made things a lot more coupled because navigation
    /// and management of views would've been handled by some of the views 
    /// themselves. 
    /// 
    /// The way to solve all this is to make the concept of a region specific to
    /// the control INSTANCE where the region resides.
    /// 
    /// UPDATE:  I ended up making a base content control class to signal a "Region"
    ///          and used a specialized event to forward it to the IModule (where I
    ///          like to keep the IRegionManager)
    /// </summary>
    public interface IRogueRegionManager
    {
        /// <summary>
        /// Returns a collection of RogueRegion instances that have been loaded into the 
        /// IRogueRegionManager
        /// </summary>
        IEnumerable<RogueRegion> GetRegions();

        /// <summary>
        /// Loads the RogueRegion with the specified view type OR the last view
        /// instance with the same type. Loading is accomplished using the ServiceLocator
        /// </summary>
        /// <returns>Instance of view</returns>
        FrameworkElement Load(RogueRegion region, Type viewType);

        /// <summary>
        /// Finds the SINGLE RogueRegion instance with the specified name - with the
        /// specified type. Loading is accomplished using the ServiceLocator
        /// </summary>
        /// <returns>Instance of view</returns>
        FrameworkElement LoadSingleInstance(string regionName, Type viewType);
    }
}
