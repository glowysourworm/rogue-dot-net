using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Attack Attribute (Passive)",
            Description = "Creates a (Friendly or Malign) Attack Attribute affect on the source character",
            ViewType = typeof(AttackAttributePassiveEffectParameters))]
    public class AttackAttributePassiveAlterationEffectTemplateViewModel 
        : TemplateViewModel, IEquipmentCurseAlterationEffectTemplateViewModel,
                             IEquipmentEquipAlterationEffectTemplateViewModel,
                             ISkillAlterationEffectTemplateViewModel
    {
        AlterationAttackAttributeCombatType _combatType;
        SymbolEffectTemplateViewModel _symbolAlteration;

        public AlterationAttackAttributeCombatType CombatType
        {
            get { return _combatType; }
            set { this.RaiseAndSetIfChanged(ref _combatType, value); }
        }

        public SymbolEffectTemplateViewModel SymbolAlteration
        {
            get { return _symbolAlteration; }
            set { this.RaiseAndSetIfChanged(ref _symbolAlteration, value); }
        }

        public ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }

        public AttackAttributePassiveAlterationEffectTemplateViewModel()
        {
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
            this.SymbolAlteration = new SymbolEffectTemplateViewModel();
        }
    }
}
