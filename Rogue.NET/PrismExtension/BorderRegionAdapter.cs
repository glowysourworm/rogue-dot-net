using Prism.Regions;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.PrismExtension
{
    [Export]
    public class BorderRegionAdapter : RegionAdapterBase<Border>
    {
        [ImportingConstructor]
        public BorderRegionAdapter(IRegionBehaviorFactory factory) : base(factory)
        {
        }

        protected override void Adapt(IRegion region, Border regionTarget)
        {
            region.ActiveViews.CollectionChanged += (obj, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        regionTarget.Child = (UIElement)e.NewItems[0];
                        break;
                    // ***** RECEIVED "CATASTROPHIC FAILURE" MESSAGE HERE. HAVE NO IDEA WHY
                    //case NotifyCollectionChangedAction.Remove:
                    //    regionTarget.Child = null;
                    //    break;
                }
            };
        }

        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}
