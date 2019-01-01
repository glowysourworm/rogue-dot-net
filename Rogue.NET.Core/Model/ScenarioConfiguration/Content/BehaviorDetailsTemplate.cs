using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class BehaviorDetailsTemplate : Template
    {
        private BehaviorTemplate _primaryBehavior;
        private BehaviorTemplate _secondaryBehavior;
        private SecondaryBehaviorInvokeReason _secondaryReason;
        private double _secondaryProbability;
        private bool _canOpenDoors;
        private double _engageRadius;
        private double _disengageRadius;
        private double _criticalRatio;
        private double _counterAttackProbability;

        public BehaviorTemplate PrimaryBehavior
        {
            get { return _primaryBehavior; }
            set
            {
                if (_primaryBehavior != value)
                {
                    _primaryBehavior = value;
                    OnPropertyChanged("PrimaryBehavior");
                }
            }
        }
        public BehaviorTemplate SecondaryBehavior
        {
            get { return _secondaryBehavior; }
            set
            {
                if (_secondaryBehavior != value)
                {
                    _secondaryBehavior = value;
                    OnPropertyChanged("SecondaryBehavior");
                }
            }
        }
        public SecondaryBehaviorInvokeReason SecondaryReason
        {
            get { return _secondaryReason; }
            set
            {
                if (_secondaryReason != value)
                {
                    _secondaryReason = value;
                    OnPropertyChanged("SecondaryReason");
                }
            }
        }
        public double SecondaryProbability
        {
            get { return _secondaryProbability; }
            set
            {
                if (_secondaryProbability != value)
                {
                    _secondaryProbability = value;
                    OnPropertyChanged("SecondaryProbability");
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
        public double EngageRadius
        {
            get { return _engageRadius; }
            set
            {
                if (_engageRadius != value)
                {
                    _engageRadius = value;
                    OnPropertyChanged("EngageRadius");
                }
            }
        }
        public double DisengageRadius
        {
            get { return _disengageRadius; }
            set
            {
                if (_disengageRadius != value)
                {
                    _disengageRadius = value;
                    OnPropertyChanged("DisengageRadius");
                }
            }
        }
        public double CriticalRatio
        {
            get { return _criticalRatio; }
            set
            {
                if (_criticalRatio != value)
                {
                    _criticalRatio = value;
                    OnPropertyChanged("CriticalRatio");
                }
            }
        }
        public double CounterAttackProbability
        {
            get { return _counterAttackProbability; }
            set
            {
                if (_counterAttackProbability != value)
                {
                    _counterAttackProbability = value;
                    OnPropertyChanged("CounterAttackProbability");
                }
            }
        }

        public BehaviorDetailsTemplate()
        {
            this.PrimaryBehavior = new BehaviorTemplate();
            this.SecondaryBehavior = new BehaviorTemplate();
        }
    }
}
