using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using Rogue.NET.Common.Events.Splash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Rogue.NET.Splash
{
    public class SplashThread<T> : IDisposable where T : Window
    {
        readonly IEventAggregator _eventAggregator;
        readonly IUnityContainer _unityContainer;

        Thread _thread;

        public SplashThread(
            IEventAggregator eventAggregator,
            IUnityContainer unityContainer)
        {
            _eventAggregator = eventAggregator;
            _unityContainer = unityContainer;
        }
        public void Start()
        {
            Dispose();

            var waitForCreation = new AutoResetEvent(false);

            ThreadStart showSplash = () =>
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(
                (Action)(() =>
                {
                    // use default constructor to avoid threading issues with unity container
                    var splashWindow = _unityContainer.Resolve<T>();

                    _eventAggregator.GetEvent<SplashEvent>().Subscribe((e) => 
                    {
                        if (e.SplashAction == SplashAction.Hide)
                            (splashWindow).Dispatcher.BeginInvoke((Action)(splashWindow).Close);
                    }, true);

                    splashWindow.Show();

                    waitForCreation.Set();
                }));

                Dispatcher.Run();
            };

            _thread = new Thread(showSplash) { Name = "SplashThread", IsBackground = true };
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();

            waitForCreation.WaitOne();  
        }

        public void Dispose()
        {
            if (_thread != null)
            {
                _thread.Abort();
                _thread = null;
            }
        }
    }
}
