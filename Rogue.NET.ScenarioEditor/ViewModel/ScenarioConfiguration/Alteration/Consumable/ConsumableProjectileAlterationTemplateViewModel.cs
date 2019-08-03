using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Consumable
{
    [Serializable]
    public class ConsumableProjectileAlterationTemplateViewModel : TemplateViewModel
    {
        private AnimationGroupTemplateViewModel _animationGroup;
        private IConsumableProjectileAlterationEffectTemplateViewModel _effect;

        public AnimationGroupTemplateViewModel AnimationGroup
        {
            get { return _animationGroup; }
            set { this.RaiseAndSetIfChanged(ref _animationGroup, value); }
        }
        public IConsumableProjectileAlterationEffectTemplateViewModel Effect
        {
            get { return _effect; }
            set { this.RaiseAndSetIfChanged(ref _effect, value); }
        }

        public ConsumableProjectileAlterationTemplateViewModel()
        {
            this.AnimationGroup = new AnimationGroupTemplateViewModel();
        }
    }
}
