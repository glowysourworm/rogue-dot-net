using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Regions;
using Rogue.NET.Model;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Model.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Rogue.NET.Common.Views;

namespace Rogue.NET.Scenario.Views
{
    public partial class GameView : UserControl
    {
        readonly IRegionManager _regionManager;
        readonly IUnityContainer _unityContainer;
        readonly IEventAggregator _eventAggregator;

        bool _initialized = false;

        public GameView(IRegionManager regionManager, IUnityContainer unityContainer, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _unityContainer = unityContainer;
            _eventAggregator = eventAggregator;

            InitializeComponent();

            this.Loaded += GameView_Loaded;
        }

        private void GameView_Loaded(object sender, RoutedEventArgs e)
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;

            _eventAggregator.GetEvent<ScenarioMessageEvent>().Subscribe((e) =>
            {
                this.DialogTB.Text = e.Message;
            });
        }

        private void GameViewButton_Click(object sender, RoutedEventArgs e)
        {
            var levelData = this.DataContext as LevelData;
            var type = (sender as ImageButton).Tag as Type;

            var active = _regionManager.Regions["GameRegion"].ActiveViews.FirstOrDefault();
            var view = (object)_unityContainer.Resolve(type);

            // can use Add repeatedly due to the border region adapter
            _regionManager.Regions["GameRegion"].Deactivate(active);
            _regionManager.Regions["GameRegion"].Activate(view);
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit current scenario?", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
            {
                _eventAggregator.GetEvent<ExitScenarioEvent>().Publish(new ExitScenarioEvent());
            }
        }

        private void ObjectiveButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEvent()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Objective
            });
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEvent()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Help
            });
        }

        private void PreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEvent()
            {
                 SplashAction = SplashAction.Show,
                 SplashType = SplashEventType.CommandPreferences
            });
        }

        private void DialogTB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _eventAggregator.GetEvent<SplashEvent>().Publish(new SplashEvent()
            {
                SplashAction = SplashAction.Show,
                SplashType = SplashEventType.Dialog
            });
        }
    }
}
