using Rogue.NET.Scenario.Content.ViewModel.Content;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Backend.EventData;
using Rogue.NET.Core.Processing.Event.Dialog.Enum;
using Rogue.NET.Scenario.Processing.Event;
using Rogue.NET.Scenario.Processing.Event.Content;

namespace Rogue.NET.Scenario.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class LevelView : UserControl
    {
        readonly IRogueEventAggregator _eventAggregator;

        [ImportingConstructor]
        public LevelView(GameViewModel viewModel, IRogueEventAggregator eventAggregator)
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
            _eventAggregator.GetEvent<DialogEvent>().Publish(new DialogEventData()
            {
                Type = DialogEventType.Objective
            });
        }

        private void HelpButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<DialogEvent>().Publish(new DialogEventData()
            {
                Type = DialogEventType.Help
            });
        }
    }
}
