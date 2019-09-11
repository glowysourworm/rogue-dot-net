using Rogue.NET.Core.Model.Enums;
using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Processing.Model.Content.Interface;

namespace Rogue.NET.Core.Model.Scenario.Character.Behavior
{
    [Serializable]
    public abstract class BehaviorDetails : RogueBase
    {
        /// <summary>
        /// This is the default behavior for any Rogue.NET character. It can be used for when
        /// a behavior is undefined or no behavior has entry conditions met. (Must be overridden in inherited class)
        /// </summary>
        public abstract Behavior DefaultBehavior { get; }

        private Behavior _currentBehavior;

        public List<Behavior> Behaviors { get; set; }
        public Behavior CurrentBehavior
        {
            get { return _currentBehavior ?? DefaultBehavior; }
        }
        public bool CanOpenDoors { get; set; }
        public bool UseRandomizer { get; set; }
        public int RandomizerTurnCount { get; set; }

        /// <summary>
        /// Turn counter internal to the Behavior Details that is for use by the
        /// pseudo state machine. It is reset every time the Behavior changes states.
        /// </summary>
        protected int BehaviorTurnCounter { get; set; }

        /// <summary>
        /// Turn counter for the randomizer - separate from the behavior turn counter
        /// </summary>
        protected int RandomizerTurnCounter { get; set; }

        public BehaviorDetails()
        {
            this.Behaviors = new List<Behavior>();
        }

        public void IncrementBehavior(NonPlayerCharacter character, IAlterationProcessor alterationProcessor, bool actionTaken, double randomNumber)
        {
            // Increment Turn Counters
            this.BehaviorTurnCounter++;
            this.RandomizerTurnCounter++;

            // Entry Conditions => (AND), Exit Conditions => (OR)
            var validBehaviors = this.Behaviors.Where(behavior =>
            {
                var entryConditionsFail = false;
                var exitConditionMet = false;

                // Enemy must be able to use Skill
                if (behavior.BehaviorCondition.HasFlag(BehaviorCondition.AttackConditionsMet) &&
                   (behavior.AttackType == CharacterAttackType.Skill ||
                    behavior.AttackType == CharacterAttackType.SkillCloseRange) &&
                   !alterationProcessor.CalculateCharacterMeetsAlterationCost(character, behavior.SkillAlterationCost))
                    entryConditionsFail = true;

                // Enemy must have Low (<= 10%) HP for this behavior
                if (behavior.BehaviorCondition.HasFlag(BehaviorCondition.HpLow) &&
                   ((character.Hp / character.HpMax) > ModelConstants.HpLowFraction))
                    entryConditionsFail = true;

                // Behavior turn counter exit condition
                if (behavior.BehaviorExitCondition.HasFlag(BehaviorExitCondition.BehaviorCounterExpired) &&
                    this.BehaviorTurnCounter >= behavior.BehaviorTurnCounter &&
                    behavior == _currentBehavior) // Counter can ONLY apply to current behavior
                    exitConditionMet = true;

                // Enemy has Low (<= 10%) HP exit condition
                if (behavior.BehaviorExitCondition.HasFlag(BehaviorExitCondition.HpLow) &&
                    ((character.Hp / character.HpMax) <= ModelConstants.HpLowFraction))
                    exitConditionMet = true;

                // Must have ALL Entry conditions met AND NO Exit conditions
                return !entryConditionsFail && !exitConditionMet;
            });

            var nextBehavior = this.DefaultBehavior;

            // Check for Randomizer
            if (this.UseRandomizer && (this.RandomizerTurnCounter % this.RandomizerTurnCount == 0))
                nextBehavior = validBehaviors.Any() ?
                               validBehaviors.PickRandom() :
                               this.DefaultBehavior;

            // Else, pick first or default (null -> Behavior.Default)
            else
                nextBehavior = validBehaviors.FirstOrDefault();

            // Reset turn counter when appropriate
            if (nextBehavior == this.DefaultBehavior ||
                nextBehavior != _currentBehavior)
                this.BehaviorTurnCounter = 0;

            // Reset randomizer turn counter if expired
            if (this.RandomizerTurnCounter % this.RandomizerTurnCount == 0)
                this.RandomizerTurnCounter = 0;

            // Finally, set current behavior
            _currentBehavior = nextBehavior;
        }
    }
}
