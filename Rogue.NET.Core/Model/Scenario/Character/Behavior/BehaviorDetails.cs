using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Model.Content.Calculator.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Character.Behavior
{
    [Serializable]
    public class BehaviorDetails : RogueBase
    {
        private Behavior _currentBehavior;

        public List<Behavior> Behaviors { get; set; }
        public Behavior CurrentBehavior
        {
            get { return _currentBehavior ?? Behavior.Default; }
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
            this.RandomizerTurnCount = 1;
        }

        public void IncrementBehavior(NonPlayerCharacter character, IAlterationCalculator alterationCalculator, bool actionTaken, double randomNumber)
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
                if (behavior.BehaviorCondition.Has(BehaviorCondition.AttackConditionsMet) &&
                   (behavior.AttackType == CharacterAttackType.Alteration) &&
                   !alterationCalculator.CalculateCharacterMeetsAlterationCost(character, behavior.Alteration.Cost))
                    entryConditionsFail = true;

                // Enemy must have Low (<= 10%) HP for this behavior
                if (behavior.BehaviorCondition.Has(BehaviorCondition.HpLow) &&
                   ((character.Hp / character.HpMax) > ModelConstants.HpLowFraction))
                    entryConditionsFail = true;

                // Behavior turn counter exit condition
                if (behavior.BehaviorExitCondition.Has(BehaviorExitCondition.BehaviorCounterExpired) &&
                    this.BehaviorTurnCounter >= behavior.BehaviorTurnCounter &&
                    behavior == _currentBehavior) // Counter can ONLY apply to current behavior
                    exitConditionMet = true;

                // Enemy has Low (<= 10%) HP exit condition
                if (behavior.BehaviorExitCondition.Has(BehaviorExitCondition.HpLow) &&
                    ((character.Hp / character.HpMax) <= ModelConstants.HpLowFraction))
                    exitConditionMet = true;

                // Must have ALL Entry conditions met AND NO Exit conditions
                return !entryConditionsFail && !exitConditionMet;
            });

            var nextBehavior = Behavior.Default;

            // Check for Randomizer
            if (this.UseRandomizer && (this.RandomizerTurnCounter % this.RandomizerTurnCount == 0))
                nextBehavior = validBehaviors.Any() ?
                               validBehaviors.PickRandom() :
                               Behavior.Default;

            // Else, pick first or default (null -> Behavior.Default)
            else
                nextBehavior = validBehaviors.FirstOrDefault() ?? Behavior.Default;

            // Reset turn counter when appropriate
            if (nextBehavior == Behavior.Default ||
                nextBehavior != _currentBehavior)
                this.BehaviorTurnCounter = 0;

            // Reset randomizer turn counter if expired
            if (this.UseRandomizer &&
                this.RandomizerTurnCounter % this.RandomizerTurnCount == 0)
                this.RandomizerTurnCounter = 0;

            // Finally, set current behavior
            _currentBehavior = nextBehavior;
        }
    }
}
