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
    [UIType(DisplayName = "Modify Equipment",
            Description = "Changes the target character's equipment (good / bad)",
            ViewType = typeof(EquipmentModifyEffectParameters))]
    public class EquipmentModifyAlterationEffectTemplateViewModel 
        : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                    IDoodadAlterationEffectTemplateViewModel,
                    IEnemyAlterationEffectTemplateViewModel,
                    ISkillAlterationEffectTemplateViewModel
    {
        AlterationModifyEquipmentType _type;
        int _classChange;
        double _qualityChange;

        public AlterationModifyEquipmentType Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public int ClassChange
        {
            get { return _classChange; }
            set { this.RaiseAndSetIfChanged(ref _classChange, value); }
        }
        public double QualityChange
        {
            get { return _qualityChange; }
            set { this.RaiseAndSetIfChanged(ref _qualityChange, value); }
        }

        public ObservableCollection<AttackAttributeTemplateViewModel> AttackAttributes { get; set; }

        public EquipmentModifyAlterationEffectTemplateViewModel()
        {
            this.AttackAttributes = new ObservableCollection<AttackAttributeTemplateViewModel>();
        }
    }
}
