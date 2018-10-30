using Prism.Mef.Modularity;
using Prism.Modularity;
using Prism.Regions;
using System.ComponentModel.Composition;

namespace Rogue.NET
{
    [ModuleExport("Rogue", typeof(RogueModule))]
    public class RogueModule : IModule
    {
        readonly IRegionManager _regionManager;

        [ImportingConstructor]
        public RogueModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
        }

        //public void OnInitialized(IContainerProvider containerProvider)
        //{
        //    _regionManager.RegisterViewWithRegion("UncurseItemGridRegion", () =>
        //    {
        //        var itemGrid = containerProvider.Resolve<ItemGrid>();
        //        itemGrid.Mode = ItemGridModes.Uncurse;
        //        return itemGrid;
        //    });
        //    _regionManager.RegisterViewWithRegion("ImbueItemGridRegion", () =>
        //    {
        //        var itemGrid = containerProvider.Resolve<ItemGrid>();
        //        itemGrid.Mode = ItemGridModes.Imbue;
        //        return itemGrid;
        //    });
        //    _regionManager.RegisterViewWithRegion("IdentifyItemGridRegion", () =>
        //    {
        //        var itemGrid = containerProvider.Resolve<ItemGrid>();
        //        itemGrid.Mode = ItemGridModes.Identify;
        //        return itemGrid;
        //    });
        //    _regionManager.RegisterViewWithRegion("EnchantItemGridRegion", () =>
        //    {
        //        var itemGrid = containerProvider.Resolve<ItemGrid>();
        //        itemGrid.Mode = ItemGridModes.Identify;
        //        return itemGrid;
        //    });
        //}
    }
}
