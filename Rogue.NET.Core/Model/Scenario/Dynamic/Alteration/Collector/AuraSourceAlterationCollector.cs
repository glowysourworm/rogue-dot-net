﻿using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Alteration.Common;
using Rogue.NET.Core.Model.Scenario.Alteration.Effect;
using Rogue.NET.Core.Model.Scenario.Alteration.Equipment;
using Rogue.NET.Core.Model.Scenario.Alteration.Interface;
using Rogue.NET.Core.Model.Scenario.Alteration.Skill;
using Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Core.Model.Scenario.Dynamic.Alteration.Collector
{
    [Serializable]
    public class AuraSourceAlterationCollector 
                 : IAlterationCollector,
                   IAlterationEffectCollector,
                   IAlterationAuraSourceCollector
    {
        protected IDictionary<string, AlterationContainer> Alterations { get; set; }

        public AuraSourceAlterationCollector()
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

        public IEnumerable<KeyValuePair<string, IAlterationEffect>> GetEffects(bool includeSourceEffects = false)
        {
            // Aura source not affected by the IAlterationEffect
            return includeSourceEffects ? this.Alterations.ToDictionary(x => x.Key, x => x.Value.Effect)
                                        : new Dictionary<string, IAlterationEffect>();
        }

        public IEnumerable<AuraSourceParameters> GetAuraSourceParameters()
        {
            return GetAuraEffects().Select(x => x.Item2).Actualize();
        }
        public IEnumerable<AlteredCharacterState> GetAlteredStates()
        {
            // Aura sources don't have altered states
            return new List<AlteredCharacterState>();
        }

        public IEnumerable<SymbolDeltaTemplate> GetSymbolChanges()
        {
            // Aura sources don't have symbol changes
            return new List<SymbolDeltaTemplate>();
        }

        public double GetAttributeAggregate(CharacterAttribute attribute)
        {
            // Aura sources don't have additional attributes
            return 0D;
        }

        public IEnumerable<Tuple<IAlterationEffect, AuraSourceParameters>> GetAuraEffects()
        {
            var result = new List<Tuple<IAlterationEffect, AuraSourceParameters>>();

            foreach (var alteration in this.Alterations.Values)
            {
                // Equipment Equip Alteration
                if (alteration is EquipmentEquipAlteration)
                    result.Add(new Tuple<IAlterationEffect,
                                         AuraSourceParameters>(alteration.Effect,
                                                              (alteration as EquipmentEquipAlteration).AuraParameters));

                // Equipment Curse Alteration
                else if (alteration is EquipmentCurseAlteration)
                    result.Add(new Tuple<IAlterationEffect,
                                         AuraSourceParameters>(alteration.Effect,
                                                              (alteration as EquipmentCurseAlteration).AuraParameters));

                // Skill Alteration
                else if (alteration is SkillAlteration)
                    result.Add(new Tuple<IAlterationEffect,
                                         AuraSourceParameters>(alteration.Effect,
                                                              (alteration as SkillAlteration).AuraParameters));

                else
                    throw new Exception("Invalid Aura Alteration Type");
            }

            return result;
        }
    }
}
