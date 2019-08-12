using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    public class RemedyAlterationEffectTemplateViewModel : 
        TemplateViewModel, IConsumableAlterationEffectTemplateViewModel,
                           IDoodadAlterationEffectTemplateViewModel,
                           ISkillAlterationEffectTemplateViewModel
    {
        private AlteredCharacterStateTemplateViewModel _remediedState;

        public AlteredCharacterStateTemplateViewModel RemediedState
        {
            get { return _remediedState; }
            set { this.RaiseAndSetIfChanged(ref _remediedState, value); }
        }

        public RemedyAlterationEffectTemplateViewModel()
        {
            this.RemediedState = new AlteredCharacterStateTemplateViewModel();
        }
    }
}
