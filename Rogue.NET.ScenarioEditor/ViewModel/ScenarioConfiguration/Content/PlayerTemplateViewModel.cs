﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration;

using ReactiveUI;

using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content
{
    public class PlayerTemplateViewModel : CharacterTemplateViewModel
    {
        private string _class;
        private double _auraRadius;
        private RangeViewModel<double> _foodUsage;
        private ObservableCollection<SkillSetTemplateViewModel> _skills;

        public string Class
        {
            get { return _class; }
            set { this.RaiseAndSetIfChanged(ref _class, value); }
        }
        public double AuraRadius
        {
            get { return _auraRadius; }
            set { this.RaiseAndSetIfChanged(ref _auraRadius, value); }
        }
        public RangeViewModel<double> FoodUsage
        {
            get { return _foodUsage; }
            set { this.RaiseAndSetIfChanged(ref _foodUsage, value); }
        }
        public ObservableCollection<SkillSetTemplateViewModel> Skills
        {
            get { return _skills; }
            set { this.RaiseAndSetIfChanged(ref _skills, value); }
        }

        public PlayerTemplateViewModel()
        {
            this.Skills = new ObservableCollection<SkillSetTemplateViewModel>();
            this.FoodUsage = new RangeViewModel<double>(0.0001, 0.005, 0.01, 1);
            this.Class = "Fighter";
            this.AuraRadius = 5;
            this.SymbolDetails.Type = SymbolTypes.Smiley;
        }
    }
}