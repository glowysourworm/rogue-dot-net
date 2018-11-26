using Prism.Events;
using Rogue.NET.Common;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Common.ViewModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace Rogue.NET.Splash.ViewModel
{
    [Export(typeof(SplashViewModel))]
    public class SplashViewModel : NotifyViewModel
    {
        string _message = "";
        double _progress = 0;

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }
        public double Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged("Progress");
            }
        }

        [ImportingConstructor]
        public SplashViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<SplashUpdateEvent>().Subscribe(Update);
        }

        private async Task Update(SplashUpdateEventArgs e)
        {
            this.Message = e.Message;
            this.Progress = e.Progress;
        }
    }
}
