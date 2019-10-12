using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    public class DetectAlterationAlterationEffectTemplateViewModel 
                    : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                                         IDoodadAlterationEffectTemplateViewModel,
                                         ISkillAlterationEffectTemplateViewModel
    {
        AlterationCategoryTemplateViewModel _alterationCategory;

        public AlterationCategoryTemplateViewModel AlterationCategory
        {
            get { return _alterationCategory; }
            set { this.RaiseAndSetIfChanged(ref _alterationCategory, value); }
        }

        public DetectAlterationAlterationEffectTemplateViewModel()
        {

        }
    }
}
