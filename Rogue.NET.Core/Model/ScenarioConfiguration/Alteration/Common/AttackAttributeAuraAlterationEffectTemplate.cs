using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class AttackAttributeAuraAlterationEffectTemplate 
        : Template, IEquipmentCurseAlterationEffectTemplate,
                    IEquipmentEquipAlterationEffectTemplate,
                    ISkillAlterationEffectTemplate
    {
        AlterationAttackAttributeCombatType _combatType;
        SymbolDeltaTemplate _symbolAlteration;

        public AlterationAttackAttributeCombatType CombatType
        {
            get { return _combatType; }
            set
            {
                if (_combatType != value)
                {
                    _combatType = value;
                    OnPropertyChanged("CombatType");
                }
            }
        }
        public SymbolDeltaTemplate SymbolAlteration
        {
            get { return _symbolAlteration; }
            set
            {
                if (_symbolAlteration != value)
                {
                    _symbolAlteration = value;
                    OnPropertyChanged("SymbolAlteration");
                }
            }
        }

        public List<AttackAttributeTemplate> AttackAttributes { get; set; }

        public AttackAttributeAuraAlterationEffectTemplate()
        {
            this.AttackAttributes = new List<AttackAttributeTemplate>();
            this.SymbolAlteration = new SymbolDeltaTemplate();
        }
    }
}
