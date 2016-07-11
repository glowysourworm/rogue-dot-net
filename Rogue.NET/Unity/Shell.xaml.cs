using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
using Rogue.NET.Common.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Model.Events;
using Rogue.NET.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Rogue.NET.Unity
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        KeyResolver _keyResolver = null;
        IEventAggregator _eventAggregator;

        public IEventAggregator EventAggregator 
        {
            get { return _eventAggregator; }
            set
            {
                _keyResolver = new KeyResolver(value);
                _eventAggregator = value;
                InitializeEvents();
            }
        }
        public IUnityContainer Container { get; set; }
        public bool BlockUserInputs { get; private set; }


        public Shell()
        {
            InitializeComponent();

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
            // Block user inputs while animation playing or no level data loaded
            this.EventAggregator.GetEvent<AnimationStartEvent>().Subscribe((e) =>
            {
                this.BlockUserInputs = true;
            });
            this.EventAggregator.GetEvent<ExitScenarioEvent>().Subscribe((e) =>
            {
                this.BlockUserInputs = true;
            });
            this.EventAggregator.GetEvent<AnimationCompletedEvent>().Subscribe((e) =>
            {
                this.BlockUserInputs = false;
            });
            this.EventAggregator.GetEvent<LevelLoadedEvent>().Subscribe((e) =>
            {
                this.BlockUserInputs = false;
            });
            this.EventAggregator.GetEvent<ExitEvent>().Subscribe((e) =>
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

            if (_keyResolver == null || this.BlockUserInputs)
                return;

            var levelCommand = _keyResolver.ResolveKeys(
                e.Key,
                Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift),
                Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl),
                Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));

            if (levelCommand != null)
            {
                this.EventAggregator.GetEvent<UserCommandEvent>().Publish(new UserCommandEvent()
                {
                    LevelCommand = levelCommand
                });
            }
        }
    }
}
