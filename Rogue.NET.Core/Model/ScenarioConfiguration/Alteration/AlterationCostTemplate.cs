﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    [Serializable]
    public class AlterationCostTemplate : Template
    {
        private double _experience;
        private double _hunger;
        private double _heatlh;
        private double _stamina;

        public double Experience
        {
            get { return _experience; }
            set
            {
                if (_experience != value)
                {
                    _experience = value;
                    OnPropertyChanged("Experience");
                }
            }
        }
        public double Hunger
        {
            get { return _hunger; }
            set
            {
                if (_hunger != value)
                {
                    _hunger = value;
                    OnPropertyChanged("Hunger");
                }
            }
        }
        public double Health
        {
            get { return _heatlh; }
            set
            {
                if (_heatlh != value)
                {
                    _heatlh = value;
                    OnPropertyChanged("Health");
                }
            }
        }
        public double Stamina
        {
            get { return _stamina; }
            set
            {
                if (_stamina != value)
                {
                    _stamina = value;
                    OnPropertyChanged("Stamina");
                }
            }
        }

        public AlterationCostTemplate()
        {

        }
    }
}
