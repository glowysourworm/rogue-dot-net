using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    [Serializable]
    public class AlterationCostTemplate : Template
    {
        private double _experience;
        private double _hunger;
        private double _hp;
        private double _mp;

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
        public double Hp
        {
            get { return _hp; }
            set
            {
                if (_hp != value)
                {
                    _hp = value;
                    OnPropertyChanged("Hp");
                }
            }
        }
        public double Mp
        {
            get { return _mp; }
            set
            {
                if (_mp != value)
                {
                    _mp = value;
                    OnPropertyChanged("Mp");
                }
            }
        }

        public AlterationCostTemplate()
        {

        }
    }
}
