using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Animation
{
    [Serializable]
    public class AnimationSequenceTemplate : Template
    {
        AlterationTargetType _targetType;
        bool _darkenBackground;

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
        public bool DarkenBackground
        {
            get { return _darkenBackground; }
            set
            {
                if (_darkenBackground != value)
                {
                    _darkenBackground = value;
                    OnPropertyChanged("DarkenBackground");
                }
            }
        }
        public List<AnimationBaseTemplate> Animations { get; set; }

        public AnimationSequenceTemplate()
        {
            this.Animations = new List<AnimationBaseTemplate>();
            this.TargetType = AlterationTargetType.Source;
            this.DarkenBackground = false;
        }
    }
}
