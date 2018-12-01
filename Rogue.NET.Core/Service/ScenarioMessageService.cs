using Prism.Events;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.ScenarioMessage;
using Rogue.NET.Core.Model.ScenarioMessage.Message;
using Rogue.NET.Core.Service.Interface;
using Rogue.NET.Model.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;

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
                IDictionary<AttackAttribute, double> attackAttributeEffect = null)
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

        public void PublishEnemyAlterationMessage(ScenarioMessagePriority priority, string playerName, string enemyDisplayName, string alterationDisplayName)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new EnemyAlterationMessage(priority)
            {
                PlayerName = playerName,
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
                IDictionary<AttackAttribute, double> attackAttributeHits = null)
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

        public void PublishPlayerAdvancement(ScenarioMessagePriority priority, string playerName, int playerLevel, IList<Tuple<string, double, Color>> attributesChanged)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new PlayerAdvancementMessage(priority)
            {
                PlayerName = playerName,
                PlayerLevel = playerLevel,
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
