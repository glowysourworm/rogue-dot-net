using Rogue.NET.Core.Model.Enums;
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

        public BehaviorDetailsTemplate()
        {
            this.Behaviors = new List<BehaviorTemplate>();
            this.UseRandomizer = false;
            this.RandomizerTurnCount = 10;
        }
    }
}
