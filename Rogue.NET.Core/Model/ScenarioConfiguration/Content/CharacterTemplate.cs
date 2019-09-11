using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class CharacterTemplate : DungeonObjectTemplate
    {
        public List<ProbabilityEquipmentTemplate> StartingEquipment { get; set; }
        public List<ProbabilityConsumableTemplate> StartingConsumables { get; set; }

        public List<AttackAttributeTemplate> AttackAttributes { get; set; }

        private Range<double> _strength;
        private Range<double> _agility;
        private Range<double> _intelligence;
        private Range<double> _speed;
        private Range<double> _hp;
        private Range<double> _mp;
        private Range<int> _lightRadius;

        public Range<double> Strength
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
        public Range<double> Agility
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
        public Range<double> Intelligence
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
        public Range<double> Speed
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
        public Range<double> Hp
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
        public Range<double> Mp
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
        public Range<int> LightRadius
        {
            get { return _lightRadius; }
            set
            {
                if (_lightRadius != value)
                {
                    _lightRadius = value;
                    OnPropertyChanged("LightRadius");
                }
            }
        }

        public CharacterTemplate()
        {
            this.Strength = new Range<double>(3, 5);
            this.Agility = new Range<double>(4, 5);
            this.Intelligence = new Range<double>(2, 3);
            this.Speed = new Range<double>(0.5, 0.5);       // Exclude 0 because Paralyzed altered state
            this.Hp = new Range<double>(10, 20);
            this.Mp = new Range<double>(2, 5);
            this.LightRadius = new Range<int>(5, 5);

            this.StartingConsumables = new List<ProbabilityConsumableTemplate>();
            this.StartingEquipment = new List<ProbabilityEquipmentTemplate>();

            this.AttackAttributes = new List<AttackAttributeTemplate>();
        }
    }
}
