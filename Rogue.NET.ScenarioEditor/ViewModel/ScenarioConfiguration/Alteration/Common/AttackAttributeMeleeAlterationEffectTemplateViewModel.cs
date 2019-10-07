using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;
using System.Collections.ObjectModel;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Attack Attribute (Combat)", 
            Description = "Creates a one-time hit towards the affected character(s)",
            ViewType = typeof(AttackAttributeMeleeEffectParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
    public class AttackAttributeMeleeAlterationEffectTemplateViewModel : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel, 
                                                                                            IConsumableProjectileAlterationEffectTemplateViewModel,
                                                                                            IDoodadAlterationEffectTemplateViewModel,
                                                                                            IEnemyAlterationEffectTemplateViewModel,
                                                                                            IFriendlyAlterationEffectTemplateViewModel,
                                                                                            ITemporaryCharacterAlterationEffectTemplateViewModel,
                                                                                            IEquipmentAttackAlterationEffectTemplateViewModel,
                                                                                            ISkillAlterationEffectTemplateViewModel
    {
        public ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }

        public AttackAttributeMeleeAlterationEffectTemplateViewModel()
        {
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
        }
    }
}
