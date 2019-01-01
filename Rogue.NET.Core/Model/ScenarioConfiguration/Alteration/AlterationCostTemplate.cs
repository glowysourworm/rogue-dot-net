using ProtoBuf;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true)]
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

        [ProtoMember(1)]
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
        [ProtoMember(2)]
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
        [ProtoMember(3)]
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
        [ProtoMember(4)]
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
        [ProtoMember(5)]
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
        [ProtoMember(6)]
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
        [ProtoMember(7)]
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
        [ProtoMember(8)]
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
        [ProtoMember(9)]
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
        [ProtoMember(10)]
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
        [ProtoMember(11)]
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
        [ProtoMember(12)]
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
        [ProtoMember(13)]
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
        [ProtoMember(14)]
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
    }
}
