using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.IO;
using Rogue.NET.Common;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Media.Animation;
using Rogue.NET.Model;
using Rogue.NET.Common.Events.Splash;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Model.Events;

namespace Rogue.NET.Scenario.Views
{
    public partial class GameView : UserControl
    {
        readonly IRegionManager _regionManager;
        readonly IUnityContainer _unityContainer;
        readonly IEventAggregator _eventAggregator;

        bool _initialized = false;

        readonly IEnumerable<Type> _displayTypes = new Type[]{
            typeof(EquipmentSelectionCtrl),
            typeof(DungeonEncyclopedia),
            typeof(ShopCtrl),
            typeof(LevelView)
        };

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

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            var levelData = this.DataContext as LevelData;
            var displayTypes = levelData.Level.IsPlayerOnShop ? _displayTypes : _displayTypes.Except(new Type[] { typeof(ShopCtrl) });
            var active = _regionManager.Regions["GameRegion"].ActiveViews.FirstOrDefault();
            var view = (object)_unityContainer.Resolve<LevelView>();
            var index = displayTypes.ToList().IndexOf(active.GetType());
            if (index - 1 < 0)
                view = _unityContainer.Resolve(displayTypes.Last());
            else
                view = _unityContainer.Resolve(displayTypes.ElementAt(index - 1));

            // slide animate the view
            var animation = new DoubleAnimation(this.RenderSize.Width, new TimeSpan(0, 0, 0, 0, 300));
            var translateTransform = new TranslateTransform(0, 0);
            animation.Completed += (obj, ev) =>
            {
                ((FrameworkElement)active).RenderTransform = null;

                // can use Add repeatedly due to the border region adapter
                _regionManager.Regions["GameRegion"].Deactivate(active);
                _regionManager.Regions["GameRegion"].Activate(view);
            };

            ((FrameworkElement)active).RenderTransform = translateTransform;
            translateTransform.ApplyAnimationClock(TranslateTransform.XProperty, animation.CreateClock());
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            var levelData = this.DataContext as LevelData;
            var displayTypes = levelData.Level.IsPlayerOnShop ? _displayTypes : _displayTypes.Except(new Type[] { typeof(ShopCtrl) });
            var active = _regionManager.Regions["GameRegion"].ActiveViews.FirstOrDefault();
            var view = (object)_unityContainer.Resolve<LevelView>();
            var index = displayTypes.ToList().IndexOf(active.GetType());
            if (index + 1 >= displayTypes.Count())
                view = _unityContainer.Resolve(displayTypes.First());
            else
                view = _unityContainer.Resolve(displayTypes.ElementAt(index + 1));

            // slide animate the view
            var animation = new DoubleAnimation(-1 *this.RenderSize.Width, new TimeSpan(0, 0, 0, 0, 300));
            var translateTransform = new TranslateTransform(0, 0);
            animation.Completed += (obj, ev) =>
            {
                ((FrameworkElement)active).RenderTransform = null;

                // can use Add repeatedly due to the border region adapter
                _regionManager.Regions["GameRegion"].Deactivate(active);
                _regionManager.Regions["GameRegion"].Activate(view);
            };

            ((FrameworkElement)active).RenderTransform = translateTransform;
            translateTransform.ApplyAnimationClock(TranslateTransform.XProperty, animation.CreateClock());
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

        private void ShopButton_Click(object sender, RoutedEventArgs e)
        {

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
