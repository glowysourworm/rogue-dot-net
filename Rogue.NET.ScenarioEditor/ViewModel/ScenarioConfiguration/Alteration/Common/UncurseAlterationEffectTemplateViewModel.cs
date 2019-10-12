using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
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
