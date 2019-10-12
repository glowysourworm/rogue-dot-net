using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    public class DetectAlterationAlignmentAlterationEffectTemplateViewModel 
                    : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                                         IDoodadAlterationEffectTemplateViewModel,
                                         ISkillAlterationEffectTemplateViewModel
    {
        AlterationAlignmentType AlterationAlignmentType;

        public AlterationAlignmentType AlterationType
        {
            get { return AlterationAlignmentType; }
            set { this.RaiseAndSetIfChanged(ref AlterationAlignmentType, value); }
        }

        public DetectAlterationAlignmentAlterationEffectTemplateViewModel()
        {

        }
    }
}
