﻿using Prism.Commands;
using Prism.Events;
using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Common.ViewModel;
using System.Windows.Input;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Intro.ViewModel
{
    public class SavedGameViewModel : NotifyViewModel
    {
        readonly IEventAggregator _eventAggregator;

        public string Name { get; set; }
        public Color SmileyColor { get; set; }
        public Color SmileyLineColor { get; set; }
        public int CurrentLevel { get; set; }
        public bool ObjectiveAcheived { get; set; }

        public ICommand DeleteScenarioCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    _eventAggregator.GetEvent<DeleteScenarioEvent>()
                                    .Publish(new DeleteScenarioEventArgs()
                    {
                        ScenarioName = this.Name
                    });
                });
            }
        }
        public ICommand StartScenarioCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    _eventAggregator.GetEvent<OpenScenarioEvent>()
                                    .Publish(new OpenScenarioEventArgs()
                    {
                        ScenarioName = this.Name
                    });
                });
            }
        }

        public SavedGameViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }
    }
}
