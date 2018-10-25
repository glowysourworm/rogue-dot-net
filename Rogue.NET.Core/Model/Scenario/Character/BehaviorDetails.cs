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

        public BehaviorDetails() { }
    }
}
