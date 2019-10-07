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
        private AnimationSequenceTemplate _animation;
        private AlterationCostTemplate _cost;
        private IAlterationEffectTemplate _effect;
        private AlterationBlockType _blockType;

        public AnimationSequenceTemplate Animation
        {
            get { return _animation; }
            set
            {
                if (_animation != value)
                {
                    _animation = value;
                    OnPropertyChanged("Animation");
                }
            }
        }
        public AlterationCostTemplate Cost
        {
            get { return _cost; }
            set
            {
                if (_cost != value)
                {
                    _cost = value;
                    OnPropertyChanged("Cost");
                }
            }
        }
        public IAlterationEffectTemplate Effect
        {
            get { return _effect; }
            set
            {
                if (_effect != value)
                {
                    _effect = value;
                    OnPropertyChanged("Effect");
                }
            }
        }
        public AlterationBlockType BlockType
        {
            get { return _blockType; }
            set
            {
                if (_blockType != value)
                {
                    _blockType = value;
                    OnPropertyChanged("BlockType");
                }
            }
        }

        public AlterationTemplate()
        {
            this.Animation = new AnimationSequenceTemplate();
            this.Cost = new AlterationCostTemplate();
        }
    }
}
