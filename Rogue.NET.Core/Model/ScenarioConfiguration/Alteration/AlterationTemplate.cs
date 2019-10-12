using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    [Serializable]
    public class AlterationTemplate : Template
    {
        private string _description;
        private AlterationCategoryTemplate _alterationCategory;
        private AnimationSequenceTemplate _animation;
        private AlterationCostTemplate _cost;
        private IAlterationEffectTemplate _effect;
        private AlterationBlockType _blockType;

        public string Description
        {
            get { return _description; }
            set { this.RaiseAndSetIfChanged(ref _description, value); }
        }
        public AlterationCategoryTemplate AlterationCategory
        {
            get { return _alterationCategory; }
            set { this.RaiseAndSetIfChanged(ref _alterationCategory, value); }
        }
        public AnimationSequenceTemplate Animation
        {
            get { return _animation; }
            set { this.RaiseAndSetIfChanged(ref _animation, value); }
        }
        public AlterationCostTemplate Cost
        {
            get { return _cost; }
            set { this.RaiseAndSetIfChanged(ref _cost, value); }
        }
        public IAlterationEffectTemplate Effect
        {
            get { return _effect; }
            set { this.RaiseAndSetIfChanged(ref _effect, value); }
        }
        public AlterationBlockType BlockType
        {
            get { return _blockType; }
            set { this.RaiseAndSetIfChanged(ref _blockType, value); }
        }

        public AlterationTemplate()
        {
            this.Animation = new AnimationSequenceTemplate();
            this.Cost = new AlterationCostTemplate();
            this.Description = "";
        }
    }
}
