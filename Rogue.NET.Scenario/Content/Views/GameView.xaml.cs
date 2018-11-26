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
using Rogue.NET.Common.View;
using System;
using Rogue.NET.Scenario.Events.Content;

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

            });
        }

        private void GameViewButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<RequestNavigateToLevelViewEvent>().Publish();
        }
        private void EquipmentButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<RequestNavigateToEquipmentSelectionEvent>().Publish();
        }

        private void GameDictionaryButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<RequestNavigateToEncyclopediaEvent>().Publish();
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
            _eventAggregator.GetEvent<DialogEvent>().Publish(new DialogUpdate()
            {
                Type = DialogEventType.Objective
            });
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<DialogEvent>().Publish(new DialogUpdate()
            {
                Type = DialogEventType.Help
            });
        }

        private void PreferencesButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggregator.GetEvent<DialogEvent>().Publish(new DialogUpdate()
            {
                 Type = DialogEventType.CommandPreferences
            });
        }
    }
}
