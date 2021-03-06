﻿using Rogue.NET.ScenarioEditor.ViewModel.Attribute;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common
{
    [Serializable]
    [UIType(DisplayName = "Remedy",
            Description = "Removes all Temporary effects with the specified Altered Character State",
            ViewType = typeof(RemedyEffectParameters),
            BaseType = UITypeAttributeBaseType.Alteration)]
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
