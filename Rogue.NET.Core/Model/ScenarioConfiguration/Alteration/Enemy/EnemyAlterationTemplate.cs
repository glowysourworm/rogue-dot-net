using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Enemy
{
    [Serializable]
    public class EnemyAlterationTemplate : Template
    {
        private AnimationGroupTemplate _animationGroup;
        private AlterationCostTemplate _cost;
        private IEnemyAlterationEffectTemplate _effect;
        private AlterationBlockType _blockType;

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
        public IEnemyAlterationEffectTemplate Effect
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

        public EnemyAlterationTemplate()
        {
            this.AnimationGroup = new AnimationGroupTemplate();
            this.Cost = new AlterationCostTemplate();
        }
    }
}
