using Prism.Events;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Splash.ViewModel;
using Rogue.NET.View;
using Rogue.NET.ViewModel;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Rogue.NET.Splash
{
    public class SplashThread
    {
        Thread _thread;
        Window _window;

        protected class SplashParameters
        {
            public IEventAggregator EventAggregator { get; set; }
            public SplashEventType Type { get; set; }
        }

        public SplashThread(IEventAggregator eventAggregator, SplashEventType splashEventType)
        {
            _thread = new Thread(new ParameterizedThreadStart(e =>
            {
                var parameters = e as SplashParameters;
                _window = CreateSplashWindow(parameters.EventAggregator, parameters.Type);                
                _window.Show();

                // DON'T UNDERSTAND THIS CALL - BUT IT FORCES A WAIT ON THIS THREAD
                Dispatcher.Run();
            }));

            _thread.SetApartmentState(ApartmentState.STA);

            _thread.Start(new SplashParameters()
            {
                EventAggregator = eventAggregator,
                Type = splashEventType
            });
        }

        public void Stop()
        {
            if (_thread != null)
            {
                // Have to terminate dispatcher for new window when thread is aborted
                _window.Dispatcher.InvokeShutdown();
                _thread.Abort();
                _thread = null;
            }
        }

        private Window CreateSplashWindow(IEventAggregator eventAggregator, SplashEventType splashEventType)
        {
            var splashWindow = new Window()
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SizeToContent = SizeToContent.WidthAndHeight,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Margin = new Thickness(0),
                FontFamily = new FontFamily(new Uri(@"pack://application:,,,/Rogue.NET.Common;Component/Resource/Fonts/CENTAUR.TTF#Centaur"), "Centaur"),
                Topmost = true
            };

            // Passing these in to take care of dependency injection. Another way is to create
            // multiple region managers and have a separate Shell window.
            switch (splashEventType)
            {
                case SplashEventType.Splash:
                    splashWindow.Content = new SplashView(new SplashViewModel(eventAggregator));
                    break;
                case SplashEventType.NewScenario:
                    splashWindow.Content = new CreatingScenarioView(new CreatingScenarioViewModel(eventAggregator));
                    break;
                case SplashEventType.CommandPreferences:
                    splashWindow.Content = new CommandPreferencesView();
                    break;
                case SplashEventType.Help:
                    splashWindow.Content = new HelpView();
                    break;
                case SplashEventType.Objective:
                    splashWindow.Content = new ObjectiveView(eventAggregator);
                    break;
                case SplashEventType.Save:
                    splashWindow.Content = new SaveView();
                    break;
                case SplashEventType.Open:
                    splashWindow.Content = new OpenScenarioView();
                    break;
                case SplashEventType.Identify:
                    splashWindow.Content = new IdentifyView(eventAggregator);
                    break;
                case SplashEventType.Uncurse:
                    splashWindow.Content = new UncurseView(eventAggregator);
                    break;
                case SplashEventType.EnchantArmor:
                    splashWindow.Content = new EnchantView(eventAggregator);
                    break;
                case SplashEventType.EnchantWeapon:
                    splashWindow.Content = new EnchantView(eventAggregator);
                    break;
                case SplashEventType.Imbue:
                    splashWindow.Content = new ImbueView(eventAggregator);
                    break;
                case SplashEventType.Dialog:
                    splashWindow.Content = new DialogView();
                    break;
                default:
                    throw new Exception("Unknwon Splash View Type");
            }

            return splashWindow;
        }
    }
}
