using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Run Away",
            Description = "Effect that causes an Enemy character to be removed from the scenario",
            ViewType = typeof(RunAwayEffectParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
    public class RunAwayAlterationEffectTemplateViewModel : TemplateViewModel, IEnemyAlterationEffectTemplateViewModel
    {
        public RunAwayAlterationEffectTemplateViewModel() { }
    }
}
