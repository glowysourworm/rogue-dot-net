using Prism.Events;
using Prism.Regions;
using Rogue.NET.Common.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Service.Interface;
using Rogue.NET.Splash.ViewModel;
using Rogue.NET.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Rogue.NET.View
{
    [Export]
    public partial class Shell : Window
    {
        readonly IEventAggregator _eventAggregator;
        readonly IKeyResolver _keyResolver;

        // Splash Thread
        Thread _splashThread;
        Window _splashWindow;

        bool _blockUserInput = false;

        [ImportingConstructor]
        public Shell(IEventAggregator eventAggregator, IKeyResolver keyResolver)
        {
            _eventAggregator = eventAggregator;
            _keyResolver = keyResolver;

            InitializeComponent();
            InitializeEvents();
        }

        public void SetFullScreenMode()
        {
            this.ShowInTaskbar = false;
            this.WindowState = WindowState.Normal;
            this.ToolbarGrid.Visibility = Visibility.Collapsed;
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
            Taskbar.Hide();
        }
        public void SetMaximizedMode()
        {
            this.ShowInTaskbar = true;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Maximized;
            this.ToolbarGrid.Visibility = Visibility.Visible;
            Taskbar.Show();
        }
        private void InitializeEvents()
        {
            _eventAggregator.GetEvent<ExitEvent>().Subscribe(() =>
            {
                Application.Current.Shutdown();
            });

            _eventAggregator.GetEvent<SplashEvent>().Subscribe(async (e) =>
            {
                await this.Dispatcher.InvokeAsync(() =>
                {
                    if (e.SplashAction == SplashAction.Hide)
                        HideSplash();
                    else
                        ShowSplash(e.SplashType);
                });
            });
        }
        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            SetFullScreenMode();
        }

        protected override async void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (_blockUserInput)
                return;

            // Have to block user input here becasue OnPreviewKeyDown is not awaited by the calling
            // thread.
            _blockUserInput = true;

            if (e.Key == Key.Escape)
            {
                SetMaximizedMode();

                _blockUserInput = false;
                return;
            }

            var levelCommand = _keyResolver.ResolveKeys(
                e.Key,
                Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
                Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl),
                Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));

            if (levelCommand != null)
            {
                await _eventAggregator.GetEvent<UserCommandEvent>().Publish(levelCommand);
            }

            _blockUserInput = false;
        }

        private void HideSplash()
        {
            if (_splashThread != null)
            {
                _splashWindow.Dispatcher.InvokeShutdown();
                _splashWindow = null;

                _splashThread.Join();
                _splashThread = null;
            }
        }

        private void ShowSplash(SplashEventType type)
        {

            _splashThread = new Thread(() =>
            {
                // Create the window here; but have handle from the other thread.. seems tricky but appears to
                // work to allow us to call InvokeDispatcherShutdown()
                _splashWindow = CreateSplashWindow(type);

                // Must set for objects that rely on synchronization context
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherSynchronizationContext(
                        Dispatcher.CurrentDispatcher));

                _splashWindow.Show();

                // DON'T UNDERSTAND THIS CALL
                System.Windows.Threading.Dispatcher.Run();
            });
            _splashThread.SetApartmentState(ApartmentState.STA);
            _splashThread.Start();
        }

        private Window CreateSplashWindow(SplashEventType type)
        {
            return new Window()
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
                Topmost = true,
                Content = CreateSplashView(type)
            };
        }

        private UserControl CreateSplashView(SplashEventType type)
        {
            // Passing these in to take care of dependency injection. Another way is to create
            // multiple region managers and have a separate Shell window.
            switch (type)
            {
                case SplashEventType.Splash:
                    return new SplashView(new SplashViewModel(_eventAggregator));
                case SplashEventType.NewScenario:
                    return new CreatingScenarioView(new CreatingScenarioViewModel(_eventAggregator));
                case SplashEventType.CommandPreferences:
                    return new CommandPreferencesView();
                case SplashEventType.Help:
                    return new HelpView();
                case SplashEventType.Objective:
                    return new ObjectiveView(_eventAggregator);
                case SplashEventType.Save:
                    return new SaveView();
                case SplashEventType.Open:
                    return new OpenScenarioView();
                case SplashEventType.Identify:
                    return new IdentifyView(_eventAggregator);
                case SplashEventType.Uncurse:
                    return new UncurseView(_eventAggregator);
                case SplashEventType.EnchantArmor:
                    return new EnchantView(_eventAggregator);
                case SplashEventType.EnchantWeapon:
                    return new EnchantView(_eventAggregator);
                case SplashEventType.Imbue:
                    return new ImbueView(_eventAggregator);
                case SplashEventType.Dialog:
                    return new DialogView();
                default:
                    throw new Exception("Unknwon Splash View Type");
            }
        }
    }
}
