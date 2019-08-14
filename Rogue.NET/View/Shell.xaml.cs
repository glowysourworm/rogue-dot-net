using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.Utility;
using Rogue.NET.Core.Event.Scenario.Level.Command;
using Rogue.NET.Core.Event.Scenario.Level.EventArgs;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing.Interface;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Scenario.Events.Content.PlayerSubpanel;
using Rogue.NET.Scenario.Service.Interface;
using Rogue.NET.ViewModel;
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

            _eventAggregator.GetEvent<DialogEvent>().Subscribe(update =>
            {
                var window = CreatePopupWindow();
                // NOTE*** THIS IS SET BY ANOTHER PART OF THE APPLICATION. THIS WAS NECESSARY.....
                Application.Current.MainWindow.Tag = window;

                window.Content = CreateDialogView(update);
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

        private UserControl CreateDialogView(IDialogUpdate update)
        {
            // Passing these in to take care of dependency injection. Another way is to create
            // multiple region managers and have a separate Shell window.
            switch (update.Type)
            {
                case DialogEventType.Help:
                    return GetInstance<HelpView>();
                case DialogEventType.Commands:
                    return GetInstance<CommandsView>();
                case DialogEventType.Objective:
                    return GetInstance<ObjectiveView>();
                case DialogEventType.Note:
                    {
                        // TODO: Use Binding Somehow...
                        var view = GetInstance<NoteView>();
                        view.TitleTB.Text = (update as IDialogNoteUpdate).NoteTitle;
                        view.MessageTB.Text = (update as IDialogNoteUpdate).NoteMessage;

                        return view;
                    }
                case DialogEventType.Identify:
                    return GetInstance<IdentifyView>();
                case DialogEventType.Uncurse:
                    return GetInstance<UncurseView>();
                case DialogEventType.EnchantArmor:
                    return GetInstance<EnchantArmorView>();
                case DialogEventType.EnchantWeapon:
                    return GetInstance<EnchantWeaponView>();
                case DialogEventType.ImbueArmor:
                    return GetInstance<ImbueArmorView>();
                case DialogEventType.ImbueWeapon:
                    return GetInstance<ImbueWeaponView>();
                case DialogEventType.PlayerAdvancement:
                    {
                        var view = GetInstance<PlayerAdvancementView>();
                        var playerUpdate = update as IDialogPlayerAdvancementUpdate;

                        view.DataContext = new PlayerAdvancementViewModel()
                        {
                            Agility = playerUpdate.Agility,                            
                            Intelligence = playerUpdate.Intelligence,
                            Strength = playerUpdate.Strength,
                            SkillPoints = playerUpdate.SkillPoints,

                            // Initialize the new variables
                            NewAgility = playerUpdate.Agility,
                            NewIntelligence = playerUpdate.Intelligence,
                            NewStrength = playerUpdate.Strength,
                            NewSkillPoints = playerUpdate.SkillPoints,

                            // Points to spend
                            PlayerPoints = playerUpdate.PlayerPoints
                        };

                        return view;
                    }
                default:
                    throw new Exception("Unknwon Splash View Type");
            }
        }

        private T GetInstance<T>()
        {
            return ServiceLocator.Current.GetInstance<T>();
        }
    }
}
