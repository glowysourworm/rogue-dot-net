﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;

namespace Rogue.NET.Core.Logic.Interface
{
    public interface IScenarioEngine
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
        void Enchant(string itemId);
        void Uncurse(string itemId);
        void Drop(string itemId);
        LevelContinuationAction Fire();
        void Target(Compass direction);
        void ToggleActiveSkill(string skillSetId, bool activate);
        void EmphasizeSkillUp(string skillSetId);
        void EmphasizeSkillDown(string skillSetId);
        LevelContinuationAction InvokePlayerSkill();
        LevelContinuationAction InvokeDoodad();
    }
}