using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [UIType(DisplayName = "Block Effect",
            Description = "Effect that lets you block other effects from a specified category",
            ViewType = typeof(BlockEffectParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
    public class BlockAlterationAlterationEffectTemplateViewModel
                    : TemplateViewModel, IEquipmentEquipAlterationEffectTemplateViewModel,
                                         ISkillAlterationEffectTemplateViewModel
    {
        AlterationCategoryTemplateViewModel _alterationCategory;
        SymbolEffectTemplateViewModel _symbolAlteration;

        public AlterationCategoryTemplateViewModel AlterationCategory
        {
            get { return _alterationCategory; }
            set { this.RaiseAndSetIfChanged(ref _alterationCategory, value); }
        }
        public SymbolEffectTemplateViewModel SymbolAlteration
        {
            get { return _symbolAlteration; }
            set { this.RaiseAndSetIfChanged(ref _symbolAlteration, value); }
        }

        public BlockAlterationAlterationEffectTemplateViewModel()
        {
            this.SymbolAlteration = new SymbolEffectTemplateViewModel();
        }
    }
}
