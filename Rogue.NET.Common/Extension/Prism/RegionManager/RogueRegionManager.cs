using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension.Prism.RegionManager.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

namespace Rogue.NET.Common.Extension.Prism.RegionManager
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRogueRegionManager))]
    public class RogueRegionManager : IRogueRegionManager
    {
        #region Attached Properties
        public static readonly DependencyProperty DefaultViewProperty =
            DependencyProperty.RegisterAttached("DefaultView", typeof(Type), typeof(RogueRegion));

        public static Type GetDefaultView(UIElement element)
        {
            return (Type)element.GetValue(DefaultViewProperty);
        }

        public static void SetDefaultView(UIElement element, Type type)
        {
            var region = element as RogueRegion;

            // Validate RogueRegion container type
            if (region == null)
                throw new ArgumentException("Region content control must inherit (or be an instance of) RogueRegion");

            // Create ONE Default Instance
            var regionView = RegionViews.SingleOrDefault(x => x.Region == region && x.ViewType == type);

            // New Region View
            if (regionView == null)
            {
                // Create View Instance
                var view = ServiceLocator.Current.GetInstance(type);

                // Hook Loaded Event
                region.Loaded += (sender, e) => { LoadImpl(region, view); };

                // Store the Region View
                RegionViews.Add(new RogueRegionView(region, type, view));
            }

            // Existing Region View
            else
                throw new Exception("Trying to reset RogueRegionManager Default View Attached Property");

            // Set Dependency Property Value
            element.SetValue(DefaultViewProperty, type);
        }
        #endregion

        #region (protected) Nested Class
        /// <summary>
        /// Nested class for maintaining region view instances
        /// </summary>
        protected class RogueRegionView
        {
            public RogueRegion Region { get; set; }
            public Type ViewType { get; set; }
            public object View { get; set; }

            public RogueRegionView(RogueRegion region, Type viewType, object view)
            {
                this.Region = region;
                this.ViewType = viewType;
                this.View = view;
            }
        }
        #endregion

        /// <summary>
        /// Maintains primary list of view instances (MUST BE STATIC TO FACILITATE ATTACHED PROPERTY DESIGN)
        /// </summary>
        protected static IList<RogueRegionView> RegionViews { get; set; }

        static RogueRegionManager()
        {
            RegionViews = new List<RogueRegionView>();
        }

        public RogueRegionManager() { }

        public void Load(RogueRegion region, Type viewType)
        {
            var regionViews = RegionViews
                                .Where(x => x.Region == region)
                                .Actualize();
            // Existing Region
            if (regionViews.Any())
            {
                var regionView = regionViews.FirstOrDefault(x => x.ViewType == viewType);

                // Load Existing View
                if (regionView != null)
                    LoadImpl(regionView.Region, regionView.View);

                // Create / Load New View
                else
                {
                    // Create view instance
                    var view = ServiceLocator.Current.GetInstance(viewType);

                    // Store region view entry
                    RegionViews.Add(new RogueRegionView(region, viewType, view));

                    // Load view
                    LoadImpl(region, view);
                }
            }

            // New Region
            else
                RegionViews
                    .Add(new RogueRegionView(region, 
                                             viewType, 
                                             ServiceLocator.Current.GetInstance(viewType)));
        }

        /// <summary>
        /// Shared (with static Attached Property Design code) Load implementation for the RogueRegion
        /// </summary>
        /// <param name="region">RogueRegion instance</param>
        /// <param name="view">UI View Content to load</param>
        protected static void LoadImpl(RogueRegion region, object view)
        {
            // TODO: Add Transition Provider
            region.Content = view;
        }
    }
}
