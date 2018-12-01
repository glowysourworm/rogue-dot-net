using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.ScenarioMessage;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Rogue.NET.Core.Service.Interface
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
                IDictionary<AttackAttribute, double> attackAttributeEffect = null);

        void PublishEnemyAlterationMessage(ScenarioMessagePriority priority, string playerName, string enemyDisplayName, string alterationDisplayName);

        void PublishMeleeMessage(
                ScenarioMessagePriority priority,
                string actor,
                string actee,
                double baseHit,
                bool isCriticalHit,
                bool anyAttackAttributes = false,
                IDictionary<AttackAttribute, double> attackAttributeHits = null);

        void PublishPlayerAdvancement(ScenarioMessagePriority priority, string playerName, int playerLevel, IList<Tuple<string, double, Color>> attributesChanged);

        void PublishSkillAdvancement(ScenarioMessagePriority priority, string skillSetName, int skillLevel);

    }
}
