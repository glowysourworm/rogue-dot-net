﻿using Rogue.NET.Common.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Event.Scenario.Level.Command;
using Rogue.NET.Core.Event.Scenario.Level.EventArgs;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Events.Content.PlayerSubpanel;
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
        readonly IRogueEventAggregator _eventAggregator;
        readonly IKeyResolver _keyResolver;

        Window _splashWindow;

        bool _blockUserInput = false;
        bool _gameMode = false;

        [ImportingConstructor]
        public Shell(IRogueEventAggregator eventAggregator, IKeyResolver keyResolver)
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

            // User enters the game (level loaded)
            _eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                _gameMode = true;
            });

            // User exits the game
            _eventAggregator.GetEvent<ExitScenarioEvent>().Subscribe(() =>
            {
                _gameMode = false;
            });

            _eventAggregator.GetEvent<SplashEvent>().Subscribe((e) =>
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
            _eventAggregator.GetEvent<DialogEvent>().Subscribe(update =>
            {
                _blockUserInput = true;
            });

            // Resume user inputs after dialog event finished
            _eventAggregator.GetEvent<DialogEventFinished>().Subscribe(() =>
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

            if (_blockUserInput)
                return;

            // Have to block user input here becasue OnPreviewKeyDown is not awaited by the calling
            // thread. (???) (NOT TRULY ASYNC / AWAIT !!!)
            _blockUserInput = true;

            if (e.Key == Key.Escape)
            {
                SetMaximizedMode();

                _blockUserInput = false;
                return;
            }

            // During Game Mode - accept user inputs as level commands
            if (_gameMode)
            {
                var levelCommand = _keyResolver.ResolveKeys(
                    e.Key,
                    Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
                    Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl),
                    Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));

                if (levelCommand != null)
                {
                    if (levelCommand is LevelCommandEventArgs ||
                        levelCommand is PlayerCommandEventArgs)
                        await _eventAggregator.GetEvent<UserCommandEvent>().Publish(levelCommand);

                    else if (levelCommand is ViewCommandEventArgs)
                    {
                        switch ((levelCommand as ViewCommandEventArgs).ViewAction)
                        {
                            case ViewActionType.ShowPlayerSubpanelEquipment:
                                _eventAggregator.GetEvent<ShowPlayerSubpanelEquipmentEvent>().Publish();
                                break;
                            case ViewActionType.ShowPlayerSubpanelConsumables:
                                _eventAggregator.GetEvent<ShowPlayerSubpanelConsumablesEvent>().Publish();
                                break;
                            case ViewActionType.ShowPlayerSubpanelSkills:
                                _eventAggregator.GetEvent<ShowPlayerSubpanelSkillsEvent>().Publish();
                                break;
                            case ViewActionType.ShowPlayerSubpanelStats:
                                _eventAggregator.GetEvent<ShowPlayerSubpanelStatsEvent>().Publish();
                                break;
                            case ViewActionType.ShowPlayerSubpanelAlterations:
                                _eventAggregator.GetEvent<ShowPlayerSubpanelAlterationsEvent>().Publish();
                                break;
                            default:
                                break;
                        }
                    }
                    else
                        throw new Exception("Unknown User Command Type");
                }
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
                default:
                    throw new Exception("Unknwon Splash View Type");
            }
        }
    }
}
