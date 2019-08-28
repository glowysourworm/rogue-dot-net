using Rogue.NET.Core.Model.Attribute;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;

namespace Rogue.NET.Core.Model.Scenario.Alteration.Effect
{
    [Serializable]
    [AlterationBlockable(typeof(IEnemyAlterationEffect),
                         typeof(ISkillAlterationEffect))]
    [AlterationCostSpecifier(AlterationCostType.OneTime,
                             typeof(IConsumableAlterationEffect),
                             typeof(IEnemyAlterationEffect),
                             typeof(ISkillAlterationEffect))]
    public class TemporaryAlterationEffect 
        : RogueBase, IConsumableAlterationEffect, 
                     IConsumableProjectileAlterationEffect,
                     IDoodadAlterationEffect,
                     IEnemyAlterationEffect,
                     ISkillAlterationEffect
    {
        public SymbolDeltaTemplate SymbolAlteration { get; set; }
        public bool CanSeeInvisibleCharacters { get; set; }
        public int EventTime { get; set; }
        public AlteredCharacterState AlteredState { get; set; }
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
        public double MentalBlockProbability { get; set; }
        public double DodgeProbability { get; set; }
        public double CriticalHit { get; set; }

        public bool IsStackable { get; set; }
        public bool HasAlteredState { get; set; }

        public TemporaryAlterationEffect()
        {
            this.SymbolAlteration = new SymbolDeltaTemplate();
            this.AlteredState = new AlteredCharacterState();   
        }
    }
}
