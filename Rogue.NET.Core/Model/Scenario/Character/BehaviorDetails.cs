using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.Scenario.Character
{
    [Serializable]
    public class BehaviorDetails : RogueBase
    {
        public Behavior CurrentBehavior
        {
            get
            {
                if (this.SecondaryReason == SecondaryBehaviorInvokeReason.SecondaryNotInvoked)
                    return this.PrimaryBehavior;

                else
                    return this.IsSecondaryBehavior ? this.SecondaryBehavior : this.PrimaryBehavior; 
            }
        }
        public Behavior PrimaryBehavior { get; set; }
        public Behavior SecondaryBehavior { get; set; }
        public SecondaryBehaviorInvokeReason SecondaryReason { get; set; }
        public double SecondaryProbability { get; set; }
        public bool IsSecondaryBehavior { get; set; }
        public bool CanOpenDoors { get; set; }
        public double EngageRadius { get; set; }
        public double DisengageRadius { get; set; }
        public double CriticalRatio { get; set; }
        public double CounterAttackProbability { get; set; }

        public BehaviorDetails()
        {
            this.PrimaryBehavior = new Behavior();
            this.SecondaryBehavior = new Behavior();
        }
    }
}
