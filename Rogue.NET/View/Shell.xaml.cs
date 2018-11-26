using Prism.Events;
using Prism.Regions;
using Rogue.NET.Common.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Scenario.Service.Interface;
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
        readonly IEventAggregator _eventAggregator;
        readonly IKeyResolver _keyResolver;

        Window _splashWindow;

        bool _blockUserInput = false;

        [ImportingConstructor]
        public Shell(IEventAggregator eventAggregator, IKeyResolver keyResolver)
        {
            _eventAggregator = eventAggregator;
            _keyResolver = keyResolver;

            _splashWindow = CreatePopupWindow();

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

            _eventAggregator.GetEvent<SplashEvent>().Subscribe((e) =>
            {
                if (e.SplashAction == SplashAction.Hide)
                    HideSplash();
                else
                    ShowSplash(e.SplashType);
            });

            _eventAggregator.GetEvent<DialogEvent>().Subscribe(update =>
            {
                var window = CreatePopupWindow();
                window.Content = CreateDialogView(update.Type);

                window.ShowDialog();
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
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            _splashWindow.Close();
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
                case SplashEventType.Save:
                    return new SaveView();
                case SplashEventType.Open:
                    return new OpenScenarioView();
                default:
                    throw new Exception("Unknwon Splash View Type");
            }
        }

        private UserControl CreateDialogView(DialogEventType type)
        {
            // Passing these in to take care of dependency injection. Another way is to create
            // multiple region managers and have a separate Shell window.
            switch (type)
            {
                case DialogEventType.CommandPreferences:
                    return new CommandPreferencesView();
                case DialogEventType.Help:
                    return new HelpView();
                case DialogEventType.Objective:
                    return new ObjectiveView(_eventAggregator);
                case DialogEventType.Identify:
                    return new IdentifyView(_eventAggregator);
                case DialogEventType.Uncurse:
                    return new UncurseView(_eventAggregator);
                case DialogEventType.EnchantArmor:
                    return new EnchantView(_eventAggregator);
                case DialogEventType.EnchantWeapon:
                    return new EnchantView(_eventAggregator);
                case DialogEventType.Imbue:
                    return new ImbueView(_eventAggregator);
                default:
                    throw new Exception("Unknwon Splash View Type");
            }
        }
    }
}
