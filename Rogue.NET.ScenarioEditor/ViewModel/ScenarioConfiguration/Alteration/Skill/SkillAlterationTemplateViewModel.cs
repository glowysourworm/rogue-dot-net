using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Skill
{
    [Serializable]
    public class SkillAlterationTemplateViewModel : TemplateViewModel
    {
        private AnimationGroupTemplateViewModel _animationGroup;
        private AlterationCostTemplateViewModel _cost;
        private ISkillAlterationEffectTemplateViewModel _effect;
        private AlterationBlockType _blockType;
        private AuraSourceParametersTemplateViewModel _auraParameters;

        public AnimationGroupTemplateViewModel AnimationGroup
        {
            get { return _animationGroup; }
            set { this.RaiseAndSetIfChanged(ref _animationGroup, value); }
        }
        public AlterationCostTemplateViewModel Cost
        {
            get { return _cost; }
            set { this.RaiseAndSetIfChanged(ref _cost, value); }
        }
        public ISkillAlterationEffectTemplateViewModel Effect
        {
            get { return _effect; }
            set { this.RaiseAndSetIfChanged(ref _effect, value); }
        }
        public AlterationBlockType BlockType
        {
            get { return _blockType; }
            set { this.RaiseAndSetIfChanged(ref _blockType, value); }
        }
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
