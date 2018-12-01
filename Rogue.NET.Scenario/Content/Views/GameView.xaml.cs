using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Model.Events;
using Prism.Events;
using Prism.Regions;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Scenario.Events.Content;
using Rogue.NET.Core.Service.Interface;

namespace Rogue.NET.Scenario.Views
{
    [Export]
    public partial class GameView : UserControl
    {
        protected enum GameViewType
        {
            LevelView,
            EquipmentView,
            EncyclopediaView
        }

        readonly IRegionManager _regionManager;
        readonly IEventAggregator _eventAggregator;

        public static readonly DependencyProperty CurrentViewTitleProperty =
            DependencyProperty.Register("CurrentViewTitle", typeof(string), typeof(GameView));

        public string CurrentViewTitle
        {
            get { return (string)GetValue(CurrentViewTitleProperty); }
            set { SetValue(CurrentViewTitleProperty, value); }
        }

        GameViewType _currentView = GameViewType.LevelView;
        int _currentLevel;

        [ImportingConstructor]
        public GameView(IRegionManager regionManager, IEventAggregator eventAggregator, IModelService modelService)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;

            InitializeComponent();

            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                _currentLevel = modelService.Level.Number;

                SetViewText();
            });
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit current scenario?", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
            {
                _eventAggregator.GetEvent<ExitScenarioEvent>().Publish();
            }
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<DialogEvent>().Publish(new DialogUpdate()
            {
                Type = DialogEventType.Help
            });
        }

        private void CommandsButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<DialogEvent>().Publish(new DialogUpdate()
            {
                Type = DialogEventType.Commands
            });
        }

        private void ObjectiveButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<DialogEvent>().Publish(new DialogUpdate()
            {
                Type = DialogEventType.Objective
            });
        }

        private void CycleViewLeftButton_Click(object sender, RoutedEventArgs e)
        {
            switch (_currentView)
            {
                case GameViewType.LevelView:
                    _eventAggregator.GetEvent<RequestNavigateToEncyclopediaEvent>().Publish();
                    _currentView = GameViewType.EncyclopediaView;
                    break;
                case GameViewType.EquipmentView:
                    _eventAggregator.GetEvent<RequestNavigateToLevelViewEvent>().Publish();
                    _currentView = GameViewType.LevelView;
                    break;
                case GameViewType.EncyclopediaView:
                    _eventAggregator.GetEvent<RequestNavigateToEquipmentSelectionEvent>().Publish();
                    _currentView = GameViewType.EquipmentView;
                    break;
                default:
                    break;
            }

            SetViewText();
        }

        private void CycleViewRightButton_Click(object sender, RoutedEventArgs e)
        {
            switch (_currentView)
            {
                case GameViewType.LevelView:
                    _eventAggregator.GetEvent<RequestNavigateToEquipmentSelectionEvent>().Publish();
                    _currentView = GameViewType.EquipmentView;
                    break;
                case GameViewType.EquipmentView:
                    _eventAggregator.GetEvent<RequestNavigateToEncyclopediaEvent>().Publish();
                    _currentView = GameViewType.EncyclopediaView;
                    break;
                case GameViewType.EncyclopediaView:
                    _eventAggregator.GetEvent<RequestNavigateToLevelViewEvent>().Publish();
                    _currentView = GameViewType.LevelView;
                    break;
                default:
                    break;
            }

            SetViewText();
        }

        private void SetViewText()
        {
            switch (_currentView)
            {
                case GameViewType.LevelView:
                    this.CurrentViewTitle = "Level " + _currentLevel.ToString();
                    break;
                case GameViewType.EquipmentView:
                    this.CurrentViewTitle = "Equipment Detail";
                    break;
                case GameViewType.EncyclopediaView:
                    this.CurrentViewTitle = "Rogue Encyclopedia";
                    break;
                default:
                    break;
            }
        }
    }
}
