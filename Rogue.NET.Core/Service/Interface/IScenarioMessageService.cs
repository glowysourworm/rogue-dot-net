using Rogue.NET.Core.Model.ScenarioMessage;
using System.Collections.Generic;

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
                IDictionary<string, double> attackAttributeEffect = null);

        void PublishEnemyAlterationMessage(ScenarioMessagePriority priority, string enemyDisplayName, string alterationDisplayName);

        void PublishMeleeMessage(
                ScenarioMessagePriority priority,
                string actor,
                string actee,
                double baseHit,
                bool isCriticalHit,
                bool anyAttackAttributes = false,
                IDictionary<string, double> attackAttributeHits = null);

        void PublishPlayerAdvancement(ScenarioMessagePriority priority, IDictionary<string, double> attributesChanged);

        void PublishSkillAdvancement(ScenarioMessagePriority priority, string skillSetName, int skillLevel);

    }
}
