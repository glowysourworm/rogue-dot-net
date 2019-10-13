using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [UIType(DisplayName = "Detect Effect",
            Description = "Effect that lets you reveal assets with the specified effect category",
            ViewType = typeof(DetectEffectParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
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
