﻿using Prism.Events;
using Rogue.NET.Intro.ViewModel;
using Rogue.NET.Scenario.Events;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Intro.Views.GameSetup
{
    [Export]
    public partial class ChooseSavedGame : UserControl
    {
        readonly IEventAggregator _eventAggragator;

        [ImportingConstructor]
        public ChooseSavedGame(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            _eventAggragator = eventAggregator;

            this.Loaded += (obj, e) => {
                var viewModel = this.DataContext as GameSetupViewModel;
                if (viewModel != null)
                    viewModel.Reinitialize();
            };
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _eventAggragator.GetEvent<GameSetupDisplayFinished>().Publish(new GameSetupDisplayFinishedEventArgs()
            {
                NextDisplayType = typeof(NewOpenEdit)
            });
        }
    }
}
