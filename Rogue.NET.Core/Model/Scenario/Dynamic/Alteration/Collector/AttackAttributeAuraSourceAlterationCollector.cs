using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using Rogue.NET.Core.Model.Scenario.Alteration.Equipment;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    /// <summary>
    /// Collects alterations that are produced by the character
    /// </summary>
    [Serializable]
    public class AttackAttributeAuraSourceAlterationCollector 
                 : IAlterationCollector,
                   IAlterationEffectCollector,
                   IAlterationAuraSourceCollector<AttackAttributeAuraAlterationEffect>
    {
        protected IDictionary<string, AlterationContainer> Alterations { get; set; }

        public AttackAttributeAuraSourceAlterationCollector()
        {
            this.Alterations = new Dictionary<string, AlterationContainer>();
        }

        public bool Apply(AlterationContainer alteration)
        {
            if (!this.Alterations.ContainsKey(alteration.RogueName))
                this.Alterations.Add(alteration.RogueName, alteration);

            else
                return false;

            return true;
        }

        public IEnumerable<AlterationContainer> Filter(string alterationName)
        {
            return this.Alterations.Filter(x => x.Key == alterationName).Values;
        }

        public IEnumerable<KeyValuePair<string, AlterationCost>> GetCosts()
        {
            return this.Alterations
                       .ToDictionary(x => x.Key, x => x.Value.Cost);
        }

        public IEnumerable<KeyValuePair<string, IAlterationEffect>> GetEffects()
        {
            // Aura source not affected by the IAlterationEffect
            return new Dictionary<string, IAlterationEffect>();
        }

        public IEnumerable<AuraSourceParameters> GetAuraSourceParameters()
        {
            return GetAuraEffects().Select(x => x.Item2).Actualize();
        }

        public IEnumerable<Tuple<AttackAttributeAuraAlterationEffect, AuraSourceParameters>> GetAuraEffects()
        {
            var result = new List<Tuple<AttackAttributeAuraAlterationEffect, AuraSourceParameters>>();

            foreach (var alteration in this.Alterations.Values)
            {
                // Equipment Equip Alteration
                if (alteration is EquipmentEquipAlteration)
                    result.Add(new Tuple<AttackAttributeAuraAlterationEffect,
                                         AuraSourceParameters>(alteration.Effect as AttackAttributeAuraAlterationEffect,
                                                              (alteration as EquipmentEquipAlteration).AuraParameters));

                // Equipment Curse Alteration
                else if (alteration is EquipmentCurseAlteration)
                    result.Add(new Tuple<AttackAttributeAuraAlterationEffect,
                                         AuraSourceParameters>(alteration.Effect as AttackAttributeAuraAlterationEffect,
                                                              (alteration as EquipmentCurseAlteration).AuraParameters));

                // Skill Alteration
                else if (alteration is SkillAlteration)
                    result.Add(new Tuple<AttackAttributeAuraAlterationEffect,
                                         AuraSourceParameters>(alteration.Effect as AttackAttributeAuraAlterationEffect,
                                                              (alteration as SkillAlteration).AuraParameters));

                else
                    throw new Exception("Invalid Aura Alteration Type");
            }

            return result;
        }

        public IEnumerable<AlteredCharacterState> GetAlteredStates()
        {
            // Aura Sources don't have altered states
            return new List<AlteredCharacterState>();
        }

        public IEnumerable<SymbolDeltaTemplate> GetSymbolChanges()
        {
            // Aura Sources don't have symbol changes
            return new List<SymbolDeltaTemplate>();
        }

        public double GetAttributeAggregate(CharacterAttribute attribute)
        {
            return 0D;
        }
    }
}
