using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Consumable
{
    [Serializable]
    public class ConsumableAlterationTemplate : Template
    {
        private AnimationGroupTemplate _animationGroup;
        private AlterationCostTemplate _cost;
        private IConsumableAlterationEffectTemplate _effect;

        public AnimationGroupTemplate AnimationGroup
        {
            get { return _animationGroup; }
            set
            {
                if (_animationGroup != value)
                {
                    _animationGroup = value;
                    OnPropertyChanged("AnimationGroup");
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
        public IConsumableAlterationEffectTemplate Effect
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

        public ConsumableAlterationTemplate()
        {
            this.AnimationGroup = new AnimationGroupTemplate();
            this.Cost = new AlterationCostTemplate()
            {
                Type = AlterationCostType.OneTime
            };
        }
    }
}
