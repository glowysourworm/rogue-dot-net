using Prism.Events;
using Prism.Regions;
using Rogue.NET.Common.ViewModel;
using System.ComponentModel.Composition;

namespace Rogue.NET.ViewModel
{
    [Export]
    public class ShellViewModel : NotifyViewModel
    {
        readonly IRegionManager _regionManager;

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            // TODO - listen for ItemGrid event to close window

            _regionManager = regionManager;
        }
    }
}
