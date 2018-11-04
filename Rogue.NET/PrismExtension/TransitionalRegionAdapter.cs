﻿using System;
using System.Linq;
using System.Collections.Specialized;
using Prism.Regions;
using System.ComponentModel.Composition;
using Rogue.NET.Common.Extension.Prism;
using System.Windows.Controls;

namespace Rogue.NET.PrismExtension
{
    [Export]
    public class TransitionPresenterRegionAdapater : RegionAdapterBase<TransitionPresenter>
    {
        [ImportingConstructor]
        public TransitionPresenterRegionAdapater(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, TransitionPresenter regionTarget)
        {
            if (regionTarget == null)
                throw new ArgumentNullException("TransitionPresenterRegionAdapater regionTarget is null");

            var contentIsSet = regionTarget.Content != null;

            if (contentIsSet)
                throw new InvalidOperationException("ContentControlHasContentException");

            region.ActiveViews.CollectionChanged += delegate
            {
                regionTarget.TransitionTo(region.ActiveViews.FirstOrDefault() as UserControl);
            };

            region.Views.CollectionChanged +=
                (sender, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add && !region.ActiveViews.Any())
                    {
                        region.Activate(e.NewItems[0]);
                    }
                };
        }

        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}