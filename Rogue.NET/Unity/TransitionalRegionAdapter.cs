using System;
using System.Linq;
using Microsoft.Practices.Prism.Regions;
using System.Collections.Specialized;
using PixelLab.Wpf.Transitions;

namespace Rogue.NET.Unity
{
    public class TransitionPresenterRegionAdapater : RegionAdapterBase<TransitionPresenter>
    {
        public TransitionPresenterRegionAdapater(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        {
        }

        protected override void Adapt(IRegion region, TransitionPresenter regionTarget)
        {
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");
            var contentIsSet = regionTarget.Content != null;

            if (contentIsSet)
            {
                throw new InvalidOperationException("ContentControlHasContentException");
            }

            region.ActiveViews.CollectionChanged += delegate
            {
                regionTarget.Content = region.ActiveViews.FirstOrDefault();
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