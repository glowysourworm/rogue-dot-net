using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Behavior;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [Export(typeof(IBehaviorGenerator))]
    public class BehaviorGenerator : IBehaviorGenerator
    {
        [ImportingConstructor]
        public BehaviorGenerator()
        {
        }

        public Behavior GenerateBehavior(BehaviorTemplate behaviorTemplate)
        {
            Behavior behavior = new Behavior();
            behavior.AttackType = behaviorTemplate.AttackType;
            behavior.BehaviorCondition = behaviorTemplate.BehaviorCondition;
            behavior.BehaviorExitCondition = behaviorTemplate.BehaviorExitCondition;
            behavior.BehaviorTurnCounter = behaviorTemplate.BehaviorTurnCounter;
            behavior.SkillAlteration = behaviorTemplate.EnemyAlteration;
            behavior.MovementType = behaviorTemplate.MovementType;
            return behavior;
        }
    }
}
