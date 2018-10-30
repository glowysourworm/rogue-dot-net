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
            behavior.CanOpenDoors = behaviorTemplate.CanOpenDoors;
            behavior.CounterAttackProbability = behaviorTemplate.CounterAttackProbability;
            behavior.CriticalRatio = behaviorTemplate.CriticalRatio;
            behavior.DisengageRadius = behaviorTemplate.DisengageRadius;
            behavior.EnemySkill = _spellGenerator.GenerateSpell(behaviorTemplate.EnemySpell);
            behavior.EngageRadius = behaviorTemplate.EngageRadius;
            behavior.MovementType = behaviorTemplate.MovementType;
            return behavior;
        }
    }
}
