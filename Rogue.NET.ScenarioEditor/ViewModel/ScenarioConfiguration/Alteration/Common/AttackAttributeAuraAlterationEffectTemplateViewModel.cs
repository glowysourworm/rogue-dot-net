using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;
using System.Collections.Generic;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Attack Attribute (Aura)",
            Description = "Creates an Aura surrounding the source character (Friendly / Malign) that affects all characters in a specified range",
            ViewType = typeof(AttackAttributeAuraEffectParameters))]
    public class AttackAttributeAuraAlterationEffectTemplateViewModel 
        : TemplateViewModel, IEquipmentCurseAlterationEffectTemplateViewModel,
                             IEquipmentEquipAlterationEffectTemplateViewModel,
                             ISkillAlterationEffectTemplateViewModel
    {
        AlterationTargetType _targetType;
        AlterationAttackAttributeCombatType _combatType;
        SymbolDeltaTemplateViewModel _symbolAlteration;

        // TODO:ALTERATION (Remove this)
        public AlterationTargetType TargetType
        {
            get { return _targetType; }
            set { this.RaiseAndSetIfChanged(ref _targetType, value); }
        }
        public AlterationAttackAttributeCombatType CombatType
        {
            get { return _combatType; }
            set { this.RaiseAndSetIfChanged(ref _combatType, value); }
        }
        public SymbolDeltaTemplateViewModel SymbolAlteration
        {
            get { return _symbolAlteration; }
            set { this.RaiseAndSetIfChanged(ref _symbolAlteration, value); }
        }

        public List<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }

        public AttackAttributeAuraAlterationEffectTemplateViewModel()
        {
            this.AttackAttributes = new List<AttackAttributeTemplateViewModel>();
            this.SymbolAlteration = new SymbolDeltaTemplateViewModel();
        }
    }
}
