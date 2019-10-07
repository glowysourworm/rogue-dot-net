using Rogue.NET.Core.Model.Enums;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Animation;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration
{
    public class AlterationTemplateViewModel : TemplateViewModel
    {
        private AnimationSequenceTemplateViewModel _animation;
        private AlterationCostTemplateViewModel _cost;
        private IAlterationEffectTemplateViewModel _effect;
        private AlterationBlockType _blockType;

        public AnimationSequenceTemplateViewModel Animation
        {
            get { return _animation; }
            set { this.RaiseAndSetIfChanged(ref _animation, value); }
        }
        public AlterationCostTemplateViewModel Cost
        {
            get { return _cost; }
            set { this.RaiseAndSetIfChanged(ref _cost, value); }
        }
        public IAlterationEffectTemplateViewModel Effect
        {
            get { return _effect; }
            set { this.RaiseAndSetIfChanged(ref _effect, value); }
        }
        public AlterationBlockType BlockType
        {
            get { return _blockType; }
            set { this.RaiseAndSetIfChanged(ref _blockType, value); }
        }

        public AlterationTemplateViewModel()
        {
            this.Animation = new AnimationSequenceTemplateViewModel();
            this.Cost = new AlterationCostTemplateViewModel();
        }
    }
}
