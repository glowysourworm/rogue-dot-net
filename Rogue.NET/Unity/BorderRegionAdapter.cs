using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Unity
{
    public class BorderRegionAdapter : RegionAdapterBase<Border>
    {
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
