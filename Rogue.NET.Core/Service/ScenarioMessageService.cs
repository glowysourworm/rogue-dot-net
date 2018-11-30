using Prism.Events;
using Rogue.NET.Core.Model.ScenarioMessage;
using Rogue.NET.Core.Model.ScenarioMessage.Message;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioMessageService))]
    public class ScenarioMessageService : IScenarioMessageService
    {
        readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public ScenarioMessageService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Publish(ScenarioMessagePriority priority, string message)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new NormalMessage(priority)
            {
                Message = message
            });
        }

        public void Publish(ScenarioMessagePriority priority, string message, params string[] formatArgs)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new NormalMessage(priority)
            {
                Message = string.Format(message, formatArgs)
            });
        }

        public void PublishAlterationMessage(
                ScenarioMessagePriority priority,
                string alterationDisplayName, 
                string effectedAttributeName, 
                double effect, 
                bool isCausedByAttackAttributes = false, 
                IDictionary<string, double> attackAttributeEffect = null)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new AlterationMessage(priority)
            {
                AlterationDisplayName = alterationDisplayName,
                AttackAttributeEffect = attackAttributeEffect,
                Effect = effect,
                EffectedAttributeName = effectedAttributeName,
                IsCausedByAttackAttributes = isCausedByAttackAttributes
            });
        }

        public void PublishEnemyAlterationMessage(ScenarioMessagePriority priority, string enemyDisplayName, string alterationDisplayName)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new EnemyAlterationMessage(priority)
            {
                AlterationDisplayName = alterationDisplayName,
                EnemyDisplayName = enemyDisplayName                
            });
        }

        public void PublishMeleeMessage(
                ScenarioMessagePriority priority,
                string actor, 
                string actee, 
                double baseHit, 
                bool isCriticalHit, 
                bool anyAttackAttributes = false, 
                IDictionary<string, double> attackAttributeHits = null)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new MeleeMessage(priority)
            {
                ActeeDisplayName = actee,
                ActorDisplayName = actor,
                AnyAttackAttributes = anyAttackAttributes,
                AttackAttributeHit = attackAttributeHits,
                BaseHit = baseHit,
                IsCriticalHit = isCriticalHit
            });
        }

        public void PublishPlayerAdvancement(ScenarioMessagePriority priority, IDictionary<string, double> attributesChanged)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new PlayerAdvancementMessage(priority)
            {
                AttributeChanges = attributesChanged
            });
        }

        public void PublishSkillAdvancement(ScenarioMessagePriority priority, string skillSetName, int skillLevel)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new SkillAdvancementMessage(priority)
            {
                SkillDisplayName = skillSetName,
                SkillLevel = skillLevel
            });
        }
    }
}
