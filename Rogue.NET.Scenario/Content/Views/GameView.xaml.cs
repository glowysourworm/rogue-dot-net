using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Model.Events;
using Prism.Events;
using Prism.Regions;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing;

namespace Rogue.NET.Scenario.Views
{
    [Export]
    public partial class GameView : UserControl
    {
        readonly IRegionManager _regionManager;
        readonly IEventAggregator _eventAggregator;

        public static readonly DependencyProperty DialogMessageProperty =
            DependencyProperty.Register("DialogMessage", typeof(string), typeof(GameView));

        public string DialogMessage
        {
            get { return (string)GetValue(DialogMessageProperty); }
            set { SetValue(DialogMessageProperty, value); }
        }

        [ImportingConstructor]
        public GameView(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            InitializeComponent();

            eventAggregator.GetEvent<ScenarioMessageEvent>().Subscribe((message) =>
            {
                this.DialogMessage = message;

            }, ThreadOption.UIThread, true);
        }

        private void GameViewButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            //var levelData = this.DataContext as LevelData;
            //var type = (sender as DisappearingButton).Tag as Type;

            //var active = _regionManager.Regions["GameRegion"].ActiveViews.FirstOrDefault();
            //var view = (object)_unityContainer.Resolve(type);

            //// can use Add repeatedly due to the border region adapter
            //_regionManager.Regions["GameRegion"].Deactivate(active);
            //_regionManager.Regions["GameRegion"].Activate(view);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit current scenario?", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
            {
                _eventAggregator.GetEvent<ExitScenarioEvent>().Publish();
            }
        }

        private void ObjectiveButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Objective
            });
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Help
            });
        }

        private void PreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                 SplashAction = SplashAction.Show,
                 SplashType = SplashEventType.CommandPreferences
            });
        }

        private void DialogTB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashUpdate()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Dialog
            });
        }
    }
}
