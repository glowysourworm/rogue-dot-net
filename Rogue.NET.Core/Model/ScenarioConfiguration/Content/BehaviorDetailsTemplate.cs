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

        public BehaviorDetailsTemplate()
        {
            this.PrimaryBehavior = new BehaviorTemplate();
            this.SecondaryBehavior = new BehaviorTemplate();
        }
    }
}
