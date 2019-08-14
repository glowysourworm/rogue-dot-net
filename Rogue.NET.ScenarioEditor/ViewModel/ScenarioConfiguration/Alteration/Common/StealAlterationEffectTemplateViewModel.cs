using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Steal",
            Description = "Allows a source character to steal a random item from a target character",
            ViewType = typeof(StealEffectParameters))]
    public class StealAlterationEffectTemplateViewModel : TemplateViewModel, IEnemyAlterationEffectTemplateViewModel,
                                                                             ISkillAlterationEffectTemplateViewModel
    {
        public StealAlterationEffectTemplateViewModel() { }
    }
}
