﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    public class AlterationCostTemplateViewModel : TemplateViewModel
    {
        private double _experience;
        private double _hunger;
        private double _health;
        private double _stamina;

        public double Experience
        {
            get { return _experience; }
            set { this.RaiseAndSetIfChanged(ref _experience, value); }
        }
        public double Hunger
        {
            get { return _hunger; }
            set { this.RaiseAndSetIfChanged(ref _hunger, value); }
        }
        public double Health
        {
            get { return _health; }
            set { this.RaiseAndSetIfChanged(ref _health, value); }
        }
        public double Stamina
        {
            get { return _stamina; }
            set { this.RaiseAndSetIfChanged(ref _stamina, value); }
        }

        public AlterationCostTemplateViewModel()
        {

        }
    }
}
