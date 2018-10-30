using Prism.Events;
using Rogue.NET.Common.Events.Splash;
using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Rogue.NET.Splash
{
    [Export]
    public class SplashThread<T> : IDisposable where T : Window
    {
        readonly IEventAggregator _eventAggregator;

        Thread _thread;

        [ImportingConstructor]
        public SplashThread(
            IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
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
