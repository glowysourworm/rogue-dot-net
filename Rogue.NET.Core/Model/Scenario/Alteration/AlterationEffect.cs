﻿using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.Scenario.Alteration
{
    [Serializable]
    public class AlterationEffect : RogueBase
    {
        public SymbolDetailsTemplate SymbolAlteration { get; set; }
        public bool IsSymbolAlteration { get; set; }

        public string PostEffectString { get; set; }
        public string DisplayName { get; set; }

        //Passive Aura's only: Copied to aura effect from alteration
        public double EffectRange { get; set; }

        public CharacterStateType State { get; set; }
        public int EventTime { get; set; }

        public double Strength { get; set; }
        public double Intelligence { get; set; }
        public double Agility { get; set; }
        public double AuraRadius { get; set; }
        public double FoodUsagePerTurn { get; set; }
        public double HpPerStep { get; set; }
        public double MpPerStep { get; set; }

        public double Attack { get; set; }
        public double Defense { get; set; }
        public double MagicBlockProbability { get; set; }
        public double DodgeProbability { get; set; }
        public int ClassEnchant { get; set; }

        public double Experience { get; set; }
        public double Hunger { get; set; }
        public double Hp { get; set; }
        public double Mp { get; set; }
        public double CriticalHit { get; set; }

        public IList<AttackAttribute> AttackAttributes { get; set; }

        /// <summary>
        /// List of spells that are removed via this alteration effect - permanent alterations only
        /// </summary>
        public virtual List<string> RemediedSpellNames { get; set; }

        public AlterationEffect() { this.AttackAttributes = new List<AttackAttribute>(); }
    }
}