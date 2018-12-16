using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration;
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

        /// <summary>
        /// Calculates new visibility for Level surrounding Player; applys end-of-turn for Player and 
        /// for the Level (Generate new monster, etc...)
        /// </summary>
        void ProcessEndOfTurn(bool regenerate);

        ScenarioObject Move(Compass direction);
        ScenarioObject MoveRandom();

        void Attack(Compass direction);
        LevelContinuationAction Throw(string itemId);
        LevelContinuationAction Consume(string itemId);
        void Identify(string itemId);
        void Enchant(string itemId);
        void ImbueArmor(string itemId, IEnumerable<AttackAttribute> attackAttributes);
        void ImbueWeapon(string itemId, IEnumerable<AttackAttribute> attackAttributes);
        void Uncurse(string itemId);
        void Drop(string itemId);
        LevelContinuationAction Fire();
        void Target(Compass direction);
        void CycleActiveSkill();
        void ToggleActiveSkill(string skillSetId, bool activate);
        void EmphasizeSkillUp(string skillSetId);
        void EmphasizeSkillDown(string skillSetId);
        LevelContinuationAction InvokePlayerSkill();
        LevelContinuationAction InvokeDoodad();
    }
}
