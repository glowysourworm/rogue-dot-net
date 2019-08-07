using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Doodad
{
    [Serializable]
    public class DoodadAlterationTemplateViewModel : TemplateViewModel
    {
        private AnimationGroupTemplateViewModel _animationGroup;
        private IDoodadAlterationEffectTemplateViewModel _effect;
        private AlterationTargetType _targetType;

        public AnimationGroupTemplateViewModel AnimationGroup
        {
            get { return _animationGroup; }
            set { this.RaiseAndSetIfChanged(ref _animationGroup, value); }
        }
        public IDoodadAlterationEffectTemplateViewModel Effect
        {
            get { return _effect; }
            set { this.RaiseAndSetIfChanged(ref _effect, value); }
        }
        public AlterationTargetType TargetType
        {
            get { return _targetType; }
            set { this.RaiseAndSetIfChanged(ref _targetType, value); }
        }

        public DoodadAlterationTemplateViewModel()
        {
            this.AnimationGroup = new AnimationGroupTemplateViewModel();
        }
    }
}
