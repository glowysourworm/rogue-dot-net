﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class BehaviorDetailsTemplate : Template
    {
        private List<BehaviorTemplate> _behaviors;
        private bool _canOpenDoors;
        private bool _useRandomizer;
        private int _randomizerTurnCount;

        private double _searchRadiusRatio;
        private double _searchRestBias;

        public List<BehaviorTemplate> Behaviors
        {
            get { return _behaviors; }
            set
            {
                if (_behaviors != value)
                {
                    _behaviors = value;
                    OnPropertyChanged("Behaviors");
                }
            }
        }
        public bool CanOpenDoors
        {
            get { return _canOpenDoors; }
            set
            {
                if (_canOpenDoors != value)
                {
                    _canOpenDoors = value;
                    OnPropertyChanged("CanOpenDoors");
                }
            }
        }
        public bool UseRandomizer
        {
            get { return _useRandomizer; }
            set
            {
                if (_useRandomizer != value)
                {
                    _useRandomizer = value;
                    OnPropertyChanged("UseRandomizer");
                }
            }
        }
        public int RandomizerTurnCount
        {
            get { return _randomizerTurnCount; }
            set
            {
                if (_randomizerTurnCount != value)
                {
                    _randomizerTurnCount = value;
                    OnPropertyChanged("RandomizerTurnCount");
                }
            }
        }

        public double SearchRadiusRatio
        {
            get { return _searchRadiusRatio; }
            set
            {
                if (_searchRadiusRatio != value)
                {
                    _searchRadiusRatio = value;
                    OnPropertyChanged("SearchRadiusRatio");
                }
            }
        }
        public double SearchRestBias
        {
            get { return _searchRestBias; }
            set
            {
                if (_searchRestBias != value)
                {
                    _searchRestBias = value;
                    OnPropertyChanged("SearchRestBias");
                }
            }
        }

        public BehaviorDetailsTemplate()
        {
            this.Behaviors = new List<BehaviorTemplate>();
            this.UseRandomizer = false;
            this.RandomizerTurnCount = 1;   // Set to prevent % arithmatic issues

            this.SearchRadiusRatio = 1;
            this.SearchRestBias = 0.80;
        }
    }
}
