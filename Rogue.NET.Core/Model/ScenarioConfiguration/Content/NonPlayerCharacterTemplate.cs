using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class NonPlayerCharacterTemplate : CharacterTemplate
    {
        private CharacterAlignmentType _alignmentType;
        private BehaviorDetailsTemplate _behaviorDetails;
        private AnimationGroupTemplate _deathAnimation;

        public CharacterAlignmentType AlignmentType
        {
            get { return _alignmentType; }
            set
            {
                if (_alignmentType != value)
                {
                    _alignmentType = value;
                    OnPropertyChanged("AlignmentType");
                }
            }
        }
        public BehaviorDetailsTemplate BehaviorDetails
        {
            get { return _behaviorDetails; }
            set
            {
                if (_behaviorDetails != value)
                {
                    _behaviorDetails = value;
                    OnPropertyChanged("BehaviorDetails");
                }
            }
        }
        public AnimationGroupTemplate DeathAnimation
        {
            get { return _deathAnimation; }
            set
            {
                if (_deathAnimation != value)
                {
                    _deathAnimation = value;
                    OnPropertyChanged("DeathAnimation");
                }
            }
        }

        public NonPlayerCharacterTemplate()
        {
            this.BehaviorDetails = new BehaviorDetailsTemplate();
            this.DeathAnimation = new AnimationGroupTemplate()
            {
                TargetType = AlterationTargetType.Source
            };
        }
    }
}
