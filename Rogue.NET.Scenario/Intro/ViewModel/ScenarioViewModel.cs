﻿using Rogue.NET.Common.ViewModel;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Rogue.NET.Scenario.Intro.ViewModel
{
    public class ScenarioViewModel : NotifyViewModel
    {
        public string Name { get; set; }
        public Color SmileyColor { get; set; }
        public Color SmileyLineColor { get; set; }
        public string Description { get; set; }

        public ObservableCollection<CharacterClassSelectionViewModel> CharacterClasses { get; set; }

        public ScenarioViewModel()
        {
            this.CharacterClasses = new ObservableCollection<CharacterClassSelectionViewModel>();
        }
    }
}
