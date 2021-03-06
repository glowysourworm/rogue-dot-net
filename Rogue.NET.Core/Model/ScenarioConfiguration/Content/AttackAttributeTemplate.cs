﻿using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class AttackAttributeTemplate : DungeonObjectTemplate
    {
        private Range<double> _attack;
        private Range<double> _resistance;
        private Range<int> _weakness;
        bool _immune;

        public Range<double> Attack
        {
            get { return _attack; }
            set
            {
                if (_attack != value)
                {
                    _attack = value;
                    OnPropertyChanged("Attack");
                }
            }
        }
        public Range<double> Resistance
        {
            get { return _resistance; }
            set
            {
                if (_resistance != value)
                {
                    _resistance = value;
                    OnPropertyChanged("Resistance");
                }
            }
        }
        public Range<int> Weakness
        {
            get { return _weakness; }
            set
            {
                if (_weakness != value)
                {
                    _weakness = value;
                    OnPropertyChanged("Weakness");
                }
            }
        }
        public bool Immune
        {
            get { return _immune; }
            set
            {
                if (_immune != value)
                {
                    _immune = value;
                    OnPropertyChanged("Immune");
                }
            }
        }

        public AttackAttributeTemplate()
        {
            this.Attack = new Range<double>(0, 0);
            this.Resistance = new Range<double>(0, 0);
            this.Weakness = new Range<int>(0, 0);
            this.Immune = false;
        }
    }
}
