using Prism.Events;
using Rogue.NET.Common;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Common.ViewModel;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace Rogue.NET.ViewModel
{
    [Export]
    public class ShellViewModel : NotifyViewModel
    {
        bool _isPopupOpen;

        public bool IsPopupOpen
        {
            get { return _isPopupOpen; }
            set
            {
                _isPopupOpen = value;
                OnPropertyChanged("IsPopupOpen");
            }
        }

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator eventAggregator)
        {
            // TODO - listen for ItemGrid event to close window

            eventAggregator.GetEvent<SplashEvent>().Subscribe((e) =>
            {
                this.IsPopupOpen = e.SplashAction == SplashAction.Show;
            });
        }
    }
}
