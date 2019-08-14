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
    [UIType(DisplayName = "Attack Attribute (Combat)", 
            Description = "Creates a one-time hit towards the affected character(s)",
            ViewType = typeof(AttackAttributeMeleeEffectParameters))]
    public class AttackAttributeMeleeAlterationEffectTemplateViewModel 
        : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel, 
                             IConsumableProjectileAlterationEffectTemplateViewModel,
                             IDoodadAlterationEffectTemplateViewModel,
                             IEnemyAlterationEffectTemplateViewModel,
                             IEquipmentAttackAlterationEffectTemplateViewModel,
                             ISkillAlterationEffectTemplateViewModel
    {
        AlterationTargetType _targetType;
        AlterationAttackAttributeCombatType _combatType;


        // TODO:ALTERATION (Remove This)
        public AlterationTargetType TargetType
        {
            get { return _targetType; }
            set { this.RaiseAndSetIfChanged(ref _targetType, value); }
        }

        // TODO:ALTERATION (Remove This)
        public AlterationAttackAttributeCombatType CombatType
        {
            get { return _combatType; }
            set { this.RaiseAndSetIfChanged(ref _combatType, value); }
        }

        public List<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }

        public AttackAttributeMeleeAlterationEffectTemplateViewModel()
        {
            this.AttackAttributes = new List<AttackAttributeTemplateViewModel>();
        }
    }
}
