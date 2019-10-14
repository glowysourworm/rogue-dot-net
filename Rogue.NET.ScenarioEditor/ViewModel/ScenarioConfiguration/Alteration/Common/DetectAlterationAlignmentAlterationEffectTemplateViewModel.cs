using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [UIType(DisplayName = "Detect Effect Alignment",
            Description = "Effect that lets you reveal assets with the specified effect alignment",
            ViewType = typeof(DetectEffectAlignmentParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
    public class DetectAlterationAlignmentAlterationEffectTemplateViewModel 
                    : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                                         IDoodadAlterationEffectTemplateViewModel,
                                         ISkillAlterationEffectTemplateViewModel
    {
        AlterationAlignmentType _alignmentType;
        bool _includeCursedEquipment;

        public AlterationAlignmentType AlignmentType
        {
            get { return _alignmentType; }
            set { this.RaiseAndSetIfChanged(ref _alignmentType, value); }
        }

        public bool IncludeCursedEquipment
        {
            get { return _includeCursedEquipment; }
            set { this.RaiseAndSetIfChanged(ref _includeCursedEquipment, value); }
        }

        public DetectAlterationAlignmentAlterationEffectTemplateViewModel()
        {

        }
    }
}
