using Prism.Events;
using Rogue.NET.Core.Event.Scenario.Level.Event;
using Rogue.NET.Core.Event.Splash;
using Rogue.NET.Core.Logic.Processing;
using Rogue.NET.Core.Logic.Processing.Enum;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using Rogue.NET.Scenario.Events;
using Rogue.NET.Scenario.Events.Content;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows;
using Rogue.NET.Common.Events.Scenario;

namespace Rogue.NET.Scenario.Views
{
    [Export]
    public partial class LevelView : UserControl
    {
        readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public LevelView(GameViewModel viewModel, IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            this.DataContext = viewModel;

            InitializeComponent();

            eventAggregator.GetEvent<ToggleLevelViewControlsEvent>().Subscribe(action =>
            {
                var leftCollapsed = this.LeftHandSideControls.Visibility == Visibility.Collapsed;
                var rightCollapsed = this.RightHandSideControls.Visibility == Visibility.Collapsed;

                switch (action.Type)
                {
                    case ToggleLevelViewControlsEventArgs.ToggleLevelViewControlsType.LeftHandSide:
                        this.LeftHandSideControls.Visibility = leftCollapsed ? Visibility.Visible : Visibility.Collapsed;
                        this.LeftHandSideCollapsedControls.Visibility = leftCollapsed ? Visibility.Collapsed : Visibility.Visible;
                        break;
                    case ToggleLevelViewControlsEventArgs.ToggleLevelViewControlsType.RightHandSide:
                        this.RightHandSideControls.Visibility = rightCollapsed ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    case ToggleLevelViewControlsEventArgs.ToggleLevelViewControlsType.All:
                        this.LeftHandSideControls.Visibility = leftCollapsed || rightCollapsed ? Visibility.Visible : Visibility.Collapsed;
                        this.LeftHandSideCollapsedControls.Visibility = leftCollapsed || rightCollapsed ? Visibility.Collapsed : Visibility.Visible;
                        this.RightHandSideControls.Visibility = leftCollapsed || rightCollapsed ? Visibility.Visible : Visibility.Collapsed;
                        break;
                    default:
                        break;
                }
            });
        }

        private void CenterButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.CenterOnPlayer);
        }

        private void UpButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.Up);
        }

        private void DownButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.Down);
        }

        private void LeftButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.Left);
        }

        private void RightButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<ShiftDisplayEvent>().Publish(ShiftDisplayType.Right);
        }

        private void ObjectivesButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<DialogEvent>().Publish(new DialogUpdate()
            {
                Type = DialogEventType.Objective
            });
        }

        private void HelpButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<DialogEvent>().Publish(new DialogUpdate()
            {
                Type = DialogEventType.Help
            });
        }
    }
}
