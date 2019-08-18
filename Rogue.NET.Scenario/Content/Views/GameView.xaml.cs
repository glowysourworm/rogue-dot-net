using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Model.Events;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Scenario.Events.Content;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Scenario.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.Extension.Prism.RegionManager.Interface;
using Rogue.NET.Scenario.Content.Views;
using Rogue.NET.Scenario.Constant;

namespace Rogue.NET.Scenario.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class GameView : UserControl
    {
        protected enum GameViewType
        {
            LevelView,
            EquipmentView,
            SkillTreeView,
            EncyclopediaView
        }

        readonly IRogueEventAggregator _eventAggregator;

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
        public GameView(IRogueRegionManager regionManager, IRogueEventAggregator eventAggregator, IModelService modelService)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();

            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                _currentLevel = modelService.Level.Number;

                SetViewText();
            });

            // Subscribe here to handle title text
            eventAggregator.GetEvent<RequestNavigateToSkillTreeEvent>().Subscribe(() =>
            {
                regionManager.LoadSingleInstance(RegionName.GameRegion, typeof(SkillTree));

                _currentView = GameViewType.SkillTreeView;

                SetViewText();
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
                case GameViewType.SkillTreeView:
                    _eventAggregator.GetEvent<RequestNavigateToEquipmentSelectionEvent>().Publish();
                    _currentView = GameViewType.EquipmentView;
                    break;
                case GameViewType.EncyclopediaView:
                    _eventAggregator.GetEvent<RequestNavigateToSkillTreeEvent>().Publish();
                    _currentView = GameViewType.SkillTreeView;
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
                    _eventAggregator.GetEvent<RequestNavigateToSkillTreeEvent>().Publish();
                    _currentView = GameViewType.SkillTreeView;
                    break;
                case GameViewType.SkillTreeView:
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
                case GameViewType.SkillTreeView:
                    this.CurrentViewTitle = "Skill Detail";
                    break;
                case GameViewType.EncyclopediaView:
                    this.CurrentViewTitle = "Rogue Encyclopedia";
                    break;
                default:
                    break;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit current scenario?", MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
            {
                _eventAggregator.GetEvent<ExitScenarioEvent>().Publish();
            }
        }

        private void CommandsButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<DialogEvent>().Publish(new DialogUpdate()
            {
                Type = DialogEventType.Commands
            });
        }

        private void CollapseALLButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ToggleLevelViewControlsEvent>().Publish(new ToggleLevelViewControlsEventArgs()
            {
                Type = ToggleLevelViewControlsEventArgs.ToggleLevelViewControlsType.All
            });
        }

        private void CollapseLHSButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ToggleLevelViewControlsEvent>().Publish(new ToggleLevelViewControlsEventArgs()
            {
                Type = ToggleLevelViewControlsEventArgs.ToggleLevelViewControlsType.LeftHandSide
            });
        }

        private void CollapseRHSButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ToggleLevelViewControlsEvent>().Publish(new ToggleLevelViewControlsEventArgs()
            {
                Type = ToggleLevelViewControlsEventArgs.ToggleLevelViewControlsType.RightHandSide
            });
        }
    }
}
