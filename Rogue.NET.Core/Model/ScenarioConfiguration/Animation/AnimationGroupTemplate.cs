using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    public class AnimationGroupTemplate : Template
    {
        AlterationTargetType _targetType;

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

        public List<AnimationTemplate> Animations { get; set; }

        public AnimationGroupTemplate()
        {
            this.Animations = new List<AnimationTemplate>();
            this.TargetType = AlterationTargetType.Source;
        }
    }
}
