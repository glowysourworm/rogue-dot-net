using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Extension.Prism.RegionManager.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Common.Extension.Prism.RegionManager
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRogueRegionManager))]
    public class RogueRegionManager : IRogueRegionManager
    {
        #region Attached Properties
        public static readonly DependencyProperty DefaultViewProperty =
            DependencyProperty.RegisterAttached("DefaultView", typeof(Type), typeof(RogueRegion));

        public static readonly DependencyProperty RegionNameProperty =
            DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(RogueRegion));

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

            // Validate View Type as FrameworkElement
            if (!typeof(FrameworkElement).IsAssignableFrom(type))
                throw new ArgumentException("View type must inherit from FrameworkElement");

            // Validate Single Default View
            if (RogueRegionManager.RegionViews.Any(x => x.Key == region &&
                                                        x.Value.Any(z => z.IsDefaultView)))
                throw new ArgumentException("Region DefaultView already set");

            // Create View Instance
            var view = (FrameworkElement)ServiceLocator.Current.GetInstance(type);

            // Hook Loaded Event
            region.Loaded += (sender, e) => { LoadImpl(region, view); };

            // Existing Entry
            if (RogueRegionManager.RegionViews.ContainsKey(region))
            {
                // Existing Entry -> New View Type
                if (!RogueRegionManager.RegionViews[region].Any(x => x.ViewType == type))
                     RogueRegionManager.RegionViews[region].Add(new RogueRegionView(type, view, true));

                else
                    throw new Exception("View Type already added to the RogueRegion. View instance lookup not currently supported");
            }
            // New Entry
            else
                RogueRegionManager.RegionViews.Add(region, new List<RogueRegionView>()
                {
                    new RogueRegionView(type, view, true)
                });

            // Set Dependency Property Value
            element.SetValue(DefaultViewProperty, type);
        }

        public static string GetRegionName(UIElement element)
        {
            return (string)element.GetValue(RegionNameProperty);
        }

        public static void SetRegionName(UIElement element, string regionName)
        {
            var region = element as RogueRegion;

            // Validate RogueRegion container type
            if (region == null)
                throw new ArgumentException("Region content control must inherit (or be an instance of) RogueRegion");

            // New Entry
            if (!RogueRegionManager.RegionViews.ContainsKey(region))
                 RogueRegionManager.RegionViews.Add(region, new List<RogueRegionView>());

            // Set Dependency Property Value
            element.SetValue(RegionNameProperty, regionName);
        }
        #endregion

        #region (protected) Nested Class
        /// <summary>
        /// Nested class for maintaining region view instances
        /// </summary>
        protected class RogueRegionView
        {
            public FrameworkElement View { get; set; }
            public Type ViewType { get; set; }
            public bool IsDefaultView { get; set; }

            public RogueRegionView(Type viewType, FrameworkElement view, bool isDefaultView = false)
            {                
                this.ViewType = viewType;
                this.View = view;
                this.IsDefaultView = isDefaultView;
            }
        }
        #endregion

        /// <summary>
        /// Maintains primary list of view instances (MUST BE STATIC TO FACILITATE ATTACHED PROPERTY DESIGN)
        /// </summary>
        protected static Dictionary<RogueRegion, List<RogueRegionView>> RegionViews { get; set; }

        static RogueRegionManager()
        {
            RogueRegionManager.RegionViews = new Dictionary<RogueRegion, List<RogueRegionView>>();
        }

        public RogueRegionManager() { }

        public IEnumerable<RogueRegion> GetRegions()
        {
            return RegionViews.Select(x => x.Key)
                              .Actualize();
        }

        public void Load(RogueRegion region, Type viewType, object dataContext)
        {
            // Validate View Type as FrameworkElement
            if (!typeof(FrameworkElement).IsAssignableFrom(viewType))
                throw new ArgumentException("View type must inherit from FrameworkElement");

            // Existing Region
            if (RogueRegionManager.RegionViews.ContainsKey(region))
            {
                var regionView = RogueRegionManager.RegionViews[region]
                                                   .FirstOrDefault(x => x.ViewType == viewType);

                // Load Existing View
                if (regionView != null)
                    LoadImpl(region, regionView.View, dataContext);

                // Create / Load New View
                else
                {
                    // Create view instance
                    var view = (FrameworkElement)ServiceLocator.Current.GetInstance(viewType);

                    // Store region view entry
                    RegionViews[region].Add(new RogueRegionView(viewType, view));

                    // Load view
                    LoadImpl(region, view, dataContext);
                }
            }

            // New Region
            else
                RegionViews
                    .Add(region, new List<RogueRegionView>()
                    {
                        new RogueRegionView(viewType, (FrameworkElement)ServiceLocator.Current.GetInstance(viewType))
                    });
        }

        public void LoadSingleInstance(string regionName, Type viewType, object dataContext)
        {
            // Validate View Type as FrameworkElement
            if (!typeof(FrameworkElement).IsAssignableFrom(viewType))
                throw new ArgumentException("View type must inherit from FrameworkElement");

            var regionView = RegionViews.SingleOrDefault(x => RogueRegionManager.GetRegionName(x.Key) == regionName);

            // Unknown Region OR Multiple Instance Region
            if (regionView.Key == null)
                throw new ArgumentException("Specified RogueRegion was either never created; or has more than one instance");

            // Load the RegionView - (Allows for new view type)
            Load(regionView.Key, viewType, dataContext);
        }

        /// <summary>
        /// Shared (with static Attached Property Design code) Load implementation for the RogueRegion
        /// </summary>
        /// <param name="region">RogueRegion instance</param>
        /// <param name="view">UI View Content to load</param>
        protected static void LoadImpl(RogueRegion region, FrameworkElement view, object dataContext = null)
        {
            // TODO: Add Transition Provider
            region.Content = view;

            // Set Data Context
            //
            // TODO: Deal with existing data context for the view rather than checking null
            if (dataContext != null)
                view.DataContext = dataContext;
        }
    }
}
