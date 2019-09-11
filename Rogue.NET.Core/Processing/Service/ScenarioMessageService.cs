using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Processing.Event.Backend;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using Rogue.NET.Core.Processing.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Service
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IScenarioMessageService))]
    public class ScenarioMessageService : IScenarioMessageService
    {
        readonly IRogueEventAggregator _eventAggregator;

        [ImportingConstructor]
        public ScenarioMessageService(IRogueEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void Publish(ScenarioMessagePriority priority, string message)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new NormalMessageData(priority)
            {
                Message = message
            });
        }

        public void Publish(ScenarioMessagePriority priority, string message, params string[] formatArgs)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new NormalMessageData(priority)
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
                IDictionary<ScenarioImage, double> attackAttributeEffect = null)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new AlterationMessageData(priority)
            {
                AlterationDisplayName = alterationDisplayName,
                AttackAttributeEffect = attackAttributeEffect,
                Effect = effect,
                EffectedAttributeName = effectedAttributeName,
                IsCausedByAttackAttributes = isCausedByAttackAttributes
            });
        }

        public void PublishAlterationCombatMessage(CharacterAlignmentType alignmentType,
                                                   string attackerName,
                                                   string defenderName,
                                                   string alterationName)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new CharacterAlterationMessageData(alignmentType == CharacterAlignmentType.PlayerAligned ? ScenarioMessagePriority.Normal : ScenarioMessagePriority.Bad)
            {
                AlterationName = alterationName,
                DefenderName = defenderName,
                AtttackerName = attackerName
            });
        }

        public void PublishMeleeMessage(
                ScenarioMessagePriority priority,
                string attacker,
                string defender,
                double baseHit,
                bool isCriticalHit,
                bool anySpecializedHits = false,
                IDictionary<ScenarioImage, double> specializedHits = null)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new MeleeMessageData(priority)
            {
                DefenderDisplayName = defender,
                AttackerDisplayName = attacker,
                AnySpecializedHits = anySpecializedHits,
                SpecializedHits = specializedHits,
                BaseHit = baseHit,
                IsCriticalHit = isCriticalHit
            });
        }

        public void PublishPlayerAdvancement(ScenarioMessagePriority priority, string playerName, int playerLevel, IList<Tuple<string, double, Color>> attributesChanged)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new PlayerAdvancementMessageData(priority)
            {
                PlayerName = playerName,
                PlayerLevel = playerLevel,
                AttributeChanges = attributesChanged
            });
        }

        public void PublishSkillAdvancement(ScenarioMessagePriority priority, string skillSetName, int skillLevel)
        {
            _eventAggregator.GetEvent<ScenarioMessageEvent>().Publish(new SkillAdvancementMessageData(priority)
            {
                SkillDisplayName = skillSetName,
                SkillLevel = skillLevel
            });
        }
    }
}
