using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Processing.Event.Backend.EventData.ScenarioMessage.Enum;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Rogue.NET.Core.Processing.Service.Interface
{
    public interface IScenarioMessageService
    {
        void Publish(ScenarioMessagePriority priority, string message);

        void Publish(ScenarioMessagePriority priority, string message, params string[] formatArgs);

        void PublishAlterationMessage(
                ScenarioMessagePriority priority,
                string alterationDisplayName,
                string effectedAttributeName,
                double effect,
                bool isCausedByAttackAttributes = false,
                IDictionary<ScenarioImage, double> attackAttributeEffect = null);

        void PublishEnemyAlterationMessage(ScenarioMessagePriority priority, string playerName, string enemyDisplayName, string alterationDisplayName);

        void PublishMeleeMessage(
                ScenarioMessagePriority priority,
                string attacker,
                string defender,
                double baseHit,
                bool isCriticalHit,
                bool anySpecializedHits = false,
                IDictionary<ScenarioImage, double> specializedHits = null);

        void PublishPlayerAdvancement(ScenarioMessagePriority priority, string playerName, int playerLevel, IList<Tuple<string, double, Color>> attributesChanged);

        void PublishSkillAdvancement(ScenarioMessagePriority priority, string skillSetName, int skillLevel);

    }
}
