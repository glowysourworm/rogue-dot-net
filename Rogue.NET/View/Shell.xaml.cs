using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Event;
using Rogue.NET.Core.Event.Level;
using Rogue.NET.Core.Event.Scenario;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Dialog;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.Router.Interface;
using Rogue.NET.Scenario.Controller.Enum;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Rogue.NET.View
{
    [Export]
    public partial class Shell : Window
    {
        readonly IGameRouter _gameRouter;

        Window _splashWindow;

        bool _blockUserInput = false;
        bool _gameMode = false;

        GameCommandMode _gameCommandMode;

        [ImportingConstructor]
        public Shell(IRogueEventAggregator eventAggregator, IGameRouter gameRouter)
        {
            _gameRouter = gameRouter;

            _splashWindow = CreatePopupWindow();

            // Hook window event request
            _gameRouter.RequestMaximizedWindowEvent += () =>
            {
                SetMaximizedMode();
            };

            InitializeComponent();
            InitializeEvents(eventAggregator);
        }

        private void InitializeEvents(IRogueEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ExitEvent>().Subscribe(() =>
            {
                Application.Current.Shutdown();
            });

            // User enters the game (level loaded)
            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                _gameMode = true;
                _gameCommandMode = GameCommandMode.BackendCommand;
            });

            // User exits the game
            eventAggregator.GetEvent<ExitScenarioEvent>().Subscribe(() =>
            {
                _gameMode = false;
            });

            eventAggregator.GetEvent<SplashEvent>().Subscribe((e) =>
            {
#if DEBUG
                // Don't want to show during debugging unless modifying the UI
                // for the loading event. (It's just really annoying)
#else
                if (e.SplashAction == SplashAction.Hide)
                    HideSplash();
                else
                    ShowSplash(e.SplashType);
#endif

            });

            // *** DIALOG MODE
            //
            //     1) Handle Mouse Events using Routed Event Preview
            //
            //     2) Block user inputs for keyboard level commands
            //
            //     3) Listen for dialog finished event to unblock
            //

            // If routed event bubbles to main region during dialog mode
            this.MainRegion.PreviewMouseDown += (sender, e) =>
            {
                if (_blockUserInput)
                    e.Handled = true;
            };

            // Block user inputs during dialog event
            eventAggregator.GetEvent<DialogEvent>().Subscribe(update =>
            {
                _blockUserInput = true;
            });

            // Resume user inputs after dialog event finished
            eventAggregator.GetEvent<DialogEventFinished>().Subscribe(() =>
            {
                _blockUserInput = false;
            });
        }

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            SetFullScreenMode();
        }

        protected override async void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (_gameMode)
            {
                await _gameRouter.IssueCommand(e.Key,
                            Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
                            Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl),
                            Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _splashWindow.Close();
        }

        protected void SetFullScreenMode()
        {
            this.ShowInTaskbar = false;
            this.WindowState = WindowState.Normal;
            this.ToolbarGrid.Visibility = Visibility.Collapsed;
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
            Taskbar.Hide();
        }
        protected void SetMaximizedMode()
        {
            this.ShowInTaskbar = true;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Maximized;
            this.ToolbarGrid.Visibility = Visibility.Visible;
            Taskbar.Show();
        }

        private void HideSplash()
        {
            _splashWindow.Hide();
        }

        private void ShowSplash(SplashEventType type)
        {
            _splashWindow.Content = CreateSplashView(type);
            _splashWindow.Show();
        }

        private Window CreatePopupWindow()
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
                Topmost = true
            };
        }

        private UserControl CreateSplashView(SplashEventType type)
        {
            // Passing these in to take care of dependency injection. Another way is to create
            // multiple region managers and have a separate Shell window.
            switch (type)
            {
                case SplashEventType.Loading:
                    return new LoadingView();
                default:
                    throw new Exception("Unknwon Splash View Type");
            }
        }
    }
}
