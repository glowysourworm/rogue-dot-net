﻿using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design
{
    public class ScenarioDesignTemplateViewModel : TemplateViewModel
    {
        private string _objectiveDescription;
        public string ObjectiveDescription
        {
            get { return _objectiveDescription; }
            set { this.RaiseAndSetIfChanged(ref _objectiveDescription, value); }
        }
        public ObservableCollection<LevelDesignTemplateViewModel> LevelDesigns { get; set; }
        public ScenarioDesignTemplateViewModel()
        {
            this.LevelDesigns = new ObservableCollection<LevelDesignTemplateViewModel>();
            this.ObjectiveDescription = "Objective Description (Goes Here)";
        }
    }
}
