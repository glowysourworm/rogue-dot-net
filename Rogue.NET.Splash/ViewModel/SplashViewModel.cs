using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common;
using Rogue.NET.Common.Events.Splash;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Splash.ViewModel
{
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

        public SplashViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<SplashUpdateEvent>().Subscribe((e) =>
            {
                this.Message = e.Message;
                this.Progress = e.Progress;
            });
        }
    }
}
