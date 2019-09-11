using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Skill
{
    [Serializable]
    public class SkillAlterationTemplateViewModel : AlterationTemplateViewModel
    {
        private AuraSourceParametersTemplateViewModel _auraParameters;

        public AuraSourceParametersTemplateViewModel AuraParameters
        {
            get { return _auraParameters; }
            set { this.RaiseAndSetIfChanged(ref _auraParameters, value); }
        }

        public SkillAlterationTemplateViewModel()
        {
            this.AnimationGroup = new AnimationGroupTemplateViewModel();
            this.Cost = new AlterationCostTemplateViewModel();
            this.AuraParameters = new AuraSourceParametersTemplateViewModel();
        }
    }
}
