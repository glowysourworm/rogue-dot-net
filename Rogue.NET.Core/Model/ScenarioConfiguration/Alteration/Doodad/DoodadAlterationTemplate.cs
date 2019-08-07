using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Doodad
{
    [Serializable]
    public class DoodadAlterationTemplate : Template
    {
        private AnimationGroupTemplate _animationGroup;
        private IDoodadAlterationEffectTemplate _effect;
        private AlterationTargetType _targetType;

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
        public IDoodadAlterationEffectTemplate Effect
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
        public AlterationTargetType TargetType
        {
            get { return _targetType; }
            set
            {
                if (_targetType != value)
                {
                    _targetType = value;
                    OnPropertyChanged("TargetType");
                }
            }
        }

        public DoodadAlterationTemplate()
        {
            this.AnimationGroup = new AnimationGroupTemplate();
        }
    }
}
