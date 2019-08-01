using Rogue.NET.Core.Model.Generator.Interface;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Model.Generator
{
    [Export(typeof(IBehaviorGenerator))]
    public class BehaviorGenerator : IBehaviorGenerator
    {
        private readonly ISpellGenerator _spellGenerator;

        [ImportingConstructor]
        public BehaviorGenerator(ISpellGenerator spellGenerator)
        {
            _spellGenerator = spellGenerator;
        }

        public Behavior GenerateBehavior(BehaviorTemplate behaviorTemplate)
        {
            Behavior behavior = new Behavior();
            behavior.AttackType = behaviorTemplate.AttackType;
            behavior.BehaviorCondition = behaviorTemplate.BehaviorCondition;
            behavior.BehaviorExitCondition = behaviorTemplate.BehaviorExitCondition;
            behavior.BehaviorTurnCounter = behaviorTemplate.BehaviorTurnCounter;
            behavior.EnemySkill = _spellGenerator.GenerateSpell(behaviorTemplate.EnemySpell);
            behavior.MovementType = behaviorTemplate.MovementType;
            return behavior;
        }
    }
}
