using Microsoft.Practices.ServiceLocation;

using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension.Prism.RegionManager.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Rogue.NET.Common.Extension.Prism.RegionManager
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRogueRegionManager))]
    public class RogueRegionManager : IRogueRegionManager
    {
        #region Attached Properties
        public static readonly DependencyProperty RegionNameProperty =
            DependencyProperty.RegisterAttached("RegionName", typeof(string), typeof(RogueRegion));

        public static readonly DependencyProperty DefaultViewTypeProperty =
            DependencyProperty.RegisterAttached("DefaultViewType", typeof(Type), typeof(RogueRegion));

        public static readonly DependencyProperty DefaultViewProperty =
            DependencyProperty.RegisterAttached("DefaultView", typeof(FrameworkElement), typeof(RogueRegion));

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

        public static Type GetDefaultViewType(UIElement element)
        {
            return (Type)element.GetValue(DefaultViewTypeProperty);
        }

        public static void SetDefaultViewType(UIElement element, Type type)
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

            // Load View - use LoadDefaultView method to re-load default view
            LoadImpl(region, view, true);

            // Set Dependency Property Value
            element.SetValue(DefaultViewTypeProperty, type);
        }

        public static FrameworkElement GetDefaultView(UIElement element)
        {
            return (FrameworkElement)element.GetValue(DefaultViewProperty);
        }

        public static void SetDefaultView(UIElement element, FrameworkElement view)
        {
            var region = element as RogueRegion;

            // Validate RogueRegion container type
            if (region == null)
                throw new ArgumentException("Region content control must inherit (or be an instance of) RogueRegion");

            // Validate Single Default View
            if (RogueRegionManager.RegionViews.Any(x => x.Key == region &&
                                                        x.Value.Any(z => z.IsDefaultView)))
                throw new ArgumentException("Region DefaultView already set");

            // Existing Entry
            if (RogueRegionManager.RegionViews.ContainsKey(region))
            {
                // Existing Entry -> New View
                if (!RogueRegionManager.RegionViews[region].Any(x => x.View == view))
                    RogueRegionManager.RegionViews[region].Add(new RogueRegionView(view.GetType(), view, true));

                else
                    throw new Exception("View already added to the RogueRegion.");
            }
            // New Entry
            else
                RogueRegionManager.RegionViews.Add(region, new List<RogueRegionView>()
                {
                    new RogueRegionView(view.GetType(), view, true)
                });

            // Load View - use LoadDefaultView method to re-load default view
            LoadImpl(region, view, true);

            // Set Dependency Property Value
            element.SetValue(DefaultViewProperty, view);
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

            public override string ToString()
            {
                return this.ViewType.Name + "  (" + this.View?.GetHashCode().ToString() +")";
            }

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
        protected static SimpleDictionary<RogueRegion, List<RogueRegionView>> RegionViews { get; set; }

        static RogueRegionManager()
        {
            RogueRegionManager.RegionViews = new SimpleDictionary<RogueRegion, List<RogueRegionView>>();
        }

        public RogueRegionManager() { }

        public RogueRegion GetRegion(string regionName)
        {
            // Allow exception if the region is not found
            return RegionViews.First(x => GetRegionName(x.Key) == regionName).Key;                              
        }

        public void PreRegisterView(string regionName, Type viewType)
        {
            // Validate View Type as FrameworkElement
            if (!typeof(FrameworkElement).IsAssignableFrom(viewType))
                throw new ArgumentException("View type must inherit from FrameworkElement");

            var regionView = RegionViews.SingleOrDefault(x => RogueRegionManager.GetRegionName(x.Key) == regionName);

            // Unknown Region OR Multiple Instance Region
            if (regionView.Key == null)
                throw new ArgumentException("Specified RogueRegion was either never created; or has more than one instance");

            if (regionView.Value.Any(x => x.ViewType == viewType))
                throw new ArgumentException("Specified view type already registered with the region");

            // Create instance of view
            var view = (FrameworkElement)ServiceLocator.Current.GetInstance(viewType);

            // Set default if there are no other default views
            var isDefault = !regionView.Value.Any(x => x.IsDefaultView);

            // Add entry to region views
            regionView.Value.Add(new RogueRegionView(viewType, view, isDefault));
        }

        public FrameworkElement Load(RogueRegion region, Type viewType, bool ignoreTransition = false)
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
                {
                    LoadImpl(region, regionView.View, ignoreTransition);

                    return regionView.View;
                }

                // Create / Load New View
                else
                {
                    // Create view instance
                    var view = (FrameworkElement)ServiceLocator.Current.GetInstance(viewType);

                    // Store region view entry
                    RegionViews[region].Add(new RogueRegionView(viewType, view));

                    // Load view
                    return LoadImpl(region, view, ignoreTransition);
                }
            }

            // New Region
            else
            {
                // Create View
                var view = (FrameworkElement)ServiceLocator.Current.GetInstance(viewType);

                // Add to RegionViews
                RegionViews
                    .Add(region, new List<RogueRegionView>()
                    {
                        new RogueRegionView(viewType, view)
                    });

                // Load View into Region
                return LoadImpl(region, view, ignoreTransition);
            }
        }

        public FrameworkElement LoadDefaultView(RogueRegion region, bool ignoreTransition = false)
        {
            if (!RogueRegionManager.RegionViews.ContainsKey(region))
                throw new Exception("RogueRegion not registered with IRogueRegionManager");

            var regionView = RogueRegionManager.RegionViews[region]
                                               .SingleOrDefault(x => x.IsDefaultView);

            if (regionView == null)
                throw new Exception("A single default view is allowed for a RogueRegion instance. See attached properties");

            // If default view is instantiated - then just load that one
            if (regionView.View != null)
                return LoadImpl(region, regionView.View, ignoreTransition);

            else
                return Load(region, regionView.ViewType, ignoreTransition);
        }

        public FrameworkElement LoadSingleInstance(string regionName, Type viewType, bool ignoreTransition = false)
        {
            // Validate View Type as FrameworkElement
            if (!typeof(FrameworkElement).IsAssignableFrom(viewType))
                throw new ArgumentException("View type must inherit from FrameworkElement");

            var regionView = RegionViews.SingleOrDefault(x => RogueRegionManager.GetRegionName(x.Key) == regionName);

            // Unknown Region OR Multiple Instance Region
            if (regionView.Key == null)
                throw new ArgumentException("Specified RogueRegion was either never created; or has more than one instance");

            // Load the RegionView - (Allows for new view type)
            return Load(regionView.Key, viewType, ignoreTransition);
        }

        /// <summary>
        /// Shared (with static Attached Property Design code) Load implementation for the RogueRegion
        /// </summary>
        /// <param name="region">RogueRegion instance</param>
        /// <param name="view">UI View Content to load</param>
        protected static FrameworkElement LoadImpl(RogueRegion region, FrameworkElement view, bool ignoreTransition)
        {
            if (!ignoreTransition)
                region.TransitionTo(view);

            else
                region.Content = view;

            return view;
        }
    }
}
