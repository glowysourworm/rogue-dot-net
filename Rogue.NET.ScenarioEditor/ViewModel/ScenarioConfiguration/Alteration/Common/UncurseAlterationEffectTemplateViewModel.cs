using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [UIType(DisplayName = "Uncurse",
            Description = "Removes the cursed status from an item in your inventory (with option for all items)",
            ViewType = typeof(UncurseEffectParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
    public class UncurseAlterationEffectTemplateViewModel : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                                                                               IDoodadAlterationEffectTemplateViewModel,
                                                                               ISkillAlterationEffectTemplateViewModel
    {
        bool _uncurseAll;

        public bool UncurseAll
        {
            get { return _uncurseAll; }
            set { this.RaiseAndSetIfChanged(ref _uncurseAll, value); }
        }

        public UncurseAlterationEffectTemplateViewModel() { }
    }
}
