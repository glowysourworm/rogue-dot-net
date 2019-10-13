using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [UIType(DisplayName = "Identify",
            Description = "Identifies an item in your inventory (with option for all items)",
            ViewType = typeof(IdentifyEffectParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
    public class IdentifyAlterationEffectTemplateViewModel : TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                                                                                IDoodadAlterationEffectTemplateViewModel,
                                                                                ISkillAlterationEffectTemplateViewModel
    {
        bool _identifyAll;

        public bool IdentifyAll
        {
            get { return _identifyAll; }
            set { this.RaiseAndSetIfChanged(ref _identifyAll, value); }
        }

        public IdentifyAlterationEffectTemplateViewModel() { }
    }
}
