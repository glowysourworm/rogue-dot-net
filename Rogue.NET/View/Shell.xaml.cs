using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Processing.Event;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Dialog;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.Core.Processing.Event.Level;
using Rogue.NET.Core.Processing.Event.Scenario;
using Rogue.NET.ModalDialog;
using Rogue.NET.Scenario.Processing.Controller.Enum;
using Rogue.NET.Scenario.Processing.Controller.Interface;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;

namespace Rogue.NET.View
{
    [Export]
    public partial class Shell : Window
    {
        readonly ICommandRouter _gameRouter;

        Window _splashWindow;

        [ImportingConstructor]
        public Shell(IRogueEventAggregator eventAggregator, ICommandRouter gameRouter)
        {
            _gameRouter = gameRouter;

            _splashWindow = SplashWindowFactory.CreatePopupWindow(SplashEventType.Loading);

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
                _gameRouter.CommandMode = GameCommandMode.BackendCommand;
            });

            // User exits the game
            eventAggregator.GetEvent<ExitScenarioEvent>().Subscribe(() =>
            {
                _gameRouter.CommandMode = GameCommandMode.Blocked;
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
                    ShowSplash();
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
                if (_gameRouter.CommandMode == GameCommandMode.DialogCommand)
                    e.Handled = true;
            };

            // Block user inputs during dialog event
            eventAggregator.GetEvent<DialogEvent>().Subscribe(update =>
            {
                _gameRouter.CommandMode = GameCommandMode.DialogCommand;
            });

            // Resume user inputs after dialog event finished
            eventAggregator.GetEvent<DialogEventFinished>().Subscribe(() =>
            {
                _gameRouter.CommandMode = GameCommandMode.BackendCommand;
            });
        }

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            SetFullScreenMode();
        }

        protected override async void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            await _gameRouter.IssueCommand(e.Key,
                        Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
                        Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl),
                        Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));
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

        private void ShowSplash()
        {
            _splashWindow.Show();
        }
    }
}
