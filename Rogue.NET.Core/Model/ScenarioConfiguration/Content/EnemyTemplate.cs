using ProtoBuf;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true, SkipConstructor = true)]
    public class EnemyTemplate : CharacterTemplate
    {
        [ProtoMember(1)]
        public List<AttackAttributeTemplate> AttackAttributes { get; set; }
        [ProtoMember(2)]
        public List<CombatAttributeTemplate> CombatAttributes { get; set; }
        [ProtoMember(3, AsReference = true)]
        public List<AnimationTemplate> DeathAnimations { get; set; }

        private bool _generateOnStep;
        private bool _isInvisible;
        private Range<double> _experienceGiven;
        private BehaviorDetailsTemplate _behaviorDetails;

        [ProtoMember(4)]
        public bool GenerateOnStep
        {
            get { return _generateOnStep; }
            set
            {
                if (_generateOnStep != value)
                {
                    _generateOnStep = value;
                    OnPropertyChanged("GenerateOnStep");
                }
            }
        }
        [ProtoMember(5)]
        public bool IsInvisible
        {
            get { return _isInvisible; }
            set
            {
                if (_isInvisible != value)
                {
                    _isInvisible = value;
                    OnPropertyChanged("IsInvisible");
                }
            }
        }
        [ProtoMember(6)]
        public Range<double> ExperienceGiven
        {
            get { return _experienceGiven; }
            set
            {
                if (_experienceGiven != value)
                {
                    _experienceGiven = value;
                    OnPropertyChanged("ExperienceGiven");
                }
            }
        }
        [ProtoMember(7)]
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

        public EnemyTemplate()
        {
            this.ExperienceGiven = new Range<double>(0, 0, 100, 100000);
            this.BehaviorDetails = new BehaviorDetailsTemplate();
            this.AttackAttributes = new List<AttackAttributeTemplate>();
            this.CombatAttributes = new List<CombatAttributeTemplate>();
            this.DeathAnimations = new List<AnimationTemplate>();
        }
        public EnemyTemplate(DungeonObjectTemplate template) : base(template)
        {
            this.ExperienceGiven = new Range<double>(0, 0, 100, 100000);
            this.BehaviorDetails = new BehaviorDetailsTemplate();
            this.AttackAttributes = new List<AttackAttributeTemplate>();
            this.CombatAttributes = new List<CombatAttributeTemplate>();
            this.DeathAnimations = new List<AnimationTemplate>();
        }
    }
}
