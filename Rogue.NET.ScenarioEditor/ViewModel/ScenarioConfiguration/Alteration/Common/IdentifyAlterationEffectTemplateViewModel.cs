using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
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
