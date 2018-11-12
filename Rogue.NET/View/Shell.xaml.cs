using Prism.Events;
using Rogue.NET.Common.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.Utility;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario.Service.Interface;
using Rogue.NET.ViewModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Input;

namespace Rogue.NET.View
{
    [Export]
    public partial class Shell : Window
    {
        readonly IEventAggregator _eventAggregator;
        readonly IKeyResolver _keyResolver;

        // NOTE*** This is required because the Publish / Subscribe mechanism for this IEventAggregator isn't
        //         working!!!  I need to be able to block inputs during animation. I tried the following:
        //
        //         1) async / await Subscribe to Animation Event - works! (i was amazed..)
        //         2) Set PublisherThread option on Subscribe to force wait on the calling thread (didn't work)
        //         3) Tried Un-subscribe in the IScenarioController to force blocking of events from user input (DIDN'T WORK!!)
        //         4) Also tried keepSubscriberReferenceAlive = false for UserCommand events (DIDN'T WORK!)
        //
        //         So... Am resorting to forcefully blocking them here.
        bool _blockInput = false;

        [ImportingConstructor]
        public Shell(ShellViewModel viewModel, IEventAggregator eventAggregator, IKeyResolver keyResolver)
        {
            _eventAggregator = eventAggregator;
            _keyResolver = keyResolver;

            this.DataContext = viewModel;

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
            _eventAggregator.GetEvent<AnimationStartEvent>().Subscribe(_ =>
            {
                _blockInput = true;
            });
            _eventAggregator.GetEvent<AnimationCompletedEvent>().Subscribe(() =>
            {
                _blockInput = false;
            });
            _eventAggregator.GetEvent<ExitEvent>().Subscribe(() =>
            {
                Application.Current.Shutdown();
            });
        }
        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            SetFullScreenMode();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Escape)
            {
                SetMaximizedMode();
                return;
            }

            if (_keyResolver == null || _blockInput)
                return;

            var levelCommand = _keyResolver.ResolveKeys(
                e.Key,
                Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
                Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl),
                Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));

            if (levelCommand != null)
            {
                _eventAggregator.GetEvent<UserCommandEvent>().Publish(levelCommand);
            }
        }
    }
}
