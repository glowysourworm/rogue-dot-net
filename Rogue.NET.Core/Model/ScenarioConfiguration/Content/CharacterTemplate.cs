﻿using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class CharacterTemplate : DungeonObjectTemplate
    {
        public List<ProbabilityEquipmentTemplate> StartingEquipment { get; set; }
        public List<ProbabilityConsumableTemplate> StartingConsumables { get; set; }

        private Range<double> _strength;
        private Range<double> _agility;
        private Range<double> _intelligence;
        private Range<double> _hp;
        private Range<double> _mp;

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

        public CharacterTemplate()
        {
            this.Strength = new Range<double>(1, 3, 5, 100);
            this.Agility = new Range<double>(1, 4, 5, 100);
            this.Intelligence = new Range<double>(1, 2, 3, 100);
            this.Hp = new Range<double>(1, 10, 20, 100);
            this.Mp = new Range<double>(1, 2, 5, 100);

            this.StartingConsumables = new List<ProbabilityConsumableTemplate>();
            this.StartingEquipment = new List<ProbabilityEquipmentTemplate>();
        }
        public CharacterTemplate(DungeonObjectTemplate tmp) : base(tmp)
        {
            this.Strength = new Range<double>(1, 3, 5, 100);
            this.Agility = new Range<double>(1, 4, 5, 100);
            this.Intelligence = new Range<double>(1, 2, 3, 100);
            this.Hp = new Range<double>(1, 10, 20, 100);
            this.Mp = new Range<double>(1, 2, 5, 100);

            this.StartingConsumables = new List<ProbabilityConsumableTemplate>();
            this.StartingEquipment = new List<ProbabilityEquipmentTemplate>();
        }
    }
}
