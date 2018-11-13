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

            if (_keyResolver == null)
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
