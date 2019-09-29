﻿using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    public class PassiveAlterationEffect
        : RogueBase, IEquipmentCurseAlterationEffect,
                     IEquipmentEquipAlterationEffect,
                     ISkillAlterationEffect
    {
        public SymbolEffectTemplate SymbolAlteration { get; set; }
        public bool CanSeeInvisibleCharacters { get; set; }
        public double Strength { get; set; }
        public double Intelligence { get; set; }
        public double Agility { get; set; }
        public double Speed { get; set; }
        public double LightRadius { get; set; }
        public double FoodUsagePerTurn { get; set; }
        public double HpPerStep { get; set; }
        public double MpPerStep { get; set; }
        public double Attack { get; set; }
        public double Defense { get; set; }
        public double MagicBlockProbability { get; set; }
        public double DodgeProbability { get; set; }
        public double CriticalHit { get; set; }

        public PassiveAlterationEffect()
        {
            this.SymbolAlteration = new SymbolEffectTemplate();
        }
    }
}
