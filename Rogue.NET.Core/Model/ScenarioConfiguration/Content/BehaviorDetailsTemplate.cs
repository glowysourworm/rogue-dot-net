using ProtoBuf;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true)]
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

        [ProtoMember(1, AsReference = true)]
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
        [ProtoMember(2, AsReference = true)]
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
        [ProtoMember(3)]
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
        [ProtoMember(4)]
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
        [ProtoMember(5)]
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
        [ProtoMember(6)]
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
        [ProtoMember(7)]
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
        [ProtoMember(8)]
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
        [ProtoMember(9)]
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
