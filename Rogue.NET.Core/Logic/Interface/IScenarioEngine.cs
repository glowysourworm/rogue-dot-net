using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Content;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Logic.Interface
{
    public interface IScenarioEngine : IRogueEngine
    {
        /// <summary>
        /// Processes automatic player action
        /// </summary>
        /// <returns>Continuation action - "DoNothing" means no altered state was processed.</returns>
        LevelContinuationAction ProcessAlteredPlayerState();

        ScenarioObject Move(Compass direction);
        ScenarioObject MoveRandom();

        void Attack(Compass direction);
        LevelContinuationAction Throw(string itemId);
        LevelContinuationAction Consume(string itemId);
        void Identify(string itemId);
        void EnhanceEquipment(EquipmentEnhanceAlterationEffect effect, string itemId);
        void Uncurse(string itemId);
        void Drop(string itemId);
        LevelContinuationAction Fire();
        void Target(Compass direction);
        void SelectSkill(string skillId);
        void CycleActiveSkillSet();
        void ToggleActiveSkillSet(string skillSetId, bool activate);
        void UnlockSkill(string skillId);
        LevelContinuationAction InvokePlayerSkill();
        LevelContinuationAction InvokeDoodad();
        void PlayerAdvancement(double strength, double agility, double intelligence, int skillPoints);
    }
}
