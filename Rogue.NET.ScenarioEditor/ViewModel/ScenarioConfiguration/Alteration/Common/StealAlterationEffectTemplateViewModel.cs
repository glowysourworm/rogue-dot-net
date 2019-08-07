using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class StealAlterationEffectTemplateViewModel : TemplateViewModel, IEnemyAlterationEffectTemplateViewModel,
                                                                             ISkillAlterationEffectTemplateViewModel
    {
        public StealAlterationEffectTemplateViewModel() { }
    }
}
