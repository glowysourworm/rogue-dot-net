using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    [Serializable]
    public class AlterationCostTemplate : Template
    {
        private AlterationCostType _type;
        private double _strength;
        private double _intelligence;
        private double _agility;
        private double _speed;
        private double _foodUsagePerTurn;
        private double _auraRadius;
        private double _experience;
        private double _hunger;
        private double _hp;
        private double _mp;
        private double _hpPerStep;
        private double _mpPerStep;
        private double _hungerPerStep;

        public AlterationCostType Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
        }
        public double Strength
        {
            get { return _strength; }
            set
            {
                if (_strength != value)
                {
                    _strength = value;
                    OnPropertyChanged("Strength");
                }
            }
        }
        public double Intelligence
        {
            get { return _intelligence; }
            set
            {
                if (_intelligence != value)
                {
                    _intelligence = value;
                    OnPropertyChanged("Intelligence");
                }
            }
        }
        public double Agility
        {
            get { return _agility; }
            set
            {
                if (_agility != value)
                {
                    _agility = value;
                    OnPropertyChanged("Agility");
                }
            }
        }
        public double Speed
        {
            get { return _speed; }
            set
            {
                if (_speed != value)
                {
                    _speed = value;
                    OnPropertyChanged("Speed");
                }
            }
        }
        public double FoodUsagePerTurn
        {
            get { return _foodUsagePerTurn; }
            set
            {
                if (_foodUsagePerTurn != value)
                {
                    _foodUsagePerTurn = value;
                    OnPropertyChanged("FoodUsagePerTurn");
                }
            }
        }
        public double AuraRadius
        {
            get { return _auraRadius; }
            set
            {
                if (_auraRadius != value)
                {
                    _auraRadius = value;
                    OnPropertyChanged("AuraRadius");
                }
            }
        }
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
        public double HpPerStep
        {
            get { return _hpPerStep; }
            set
            {
                if (_hpPerStep != value)
                {
                    _hpPerStep = value;
                    OnPropertyChanged("HpPerStep");
                }
            }
        }
        public double MpPerStep
        {
            get { return _mpPerStep; }
            set
            {
                if (_mpPerStep != value)
                {
                    _mpPerStep = value;
                    OnPropertyChanged("MpPerStep");
                }
            }
        }
        public double HungerPerStep
        {
            get { return _hungerPerStep; }
            set
            {
                if (_hungerPerStep != value)
                {
                    _hungerPerStep = value;
                    OnPropertyChanged("HungerPerStep");
                }
            }
        }

        public AlterationCostTemplate()
        {

        }
    }
}
