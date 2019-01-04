using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class EnemyTemplate : CharacterTemplate
    {
        public List<AttackAttributeTemplate> AttackAttributes { get; set; }
        public List<AnimationTemplate> DeathAnimations { get; set; }

        private bool _generateOnStep;
        private bool _isInvisible;
        private bool _hasReligiousAffiliation;
        private Range<double> _experienceGiven;
        private BehaviorDetailsTemplate _behaviorDetails;
        private ReligiousAffiliationTemplate _religiousAffiliation;

        public bool HasReligiousAffiliation
        {
            get { return _hasReligiousAffiliation; }
            set
            {
                if (_hasReligiousAffiliation != value)
                {
                    _hasReligiousAffiliation = value;
                    OnPropertyChanged("HasReligiousAffiliation");
                }
            }
        }
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
        public ReligiousAffiliationTemplate ReligiousAffiliation
        {
            get { return _religiousAffiliation; }
            set
            {
                if (_religiousAffiliation != value)
                {
                    _religiousAffiliation = value;
                    OnPropertyChanged("ReligiousAffiliation");
                }
            }
        }

        public EnemyTemplate()
        {
            this.HasReligiousAffiliation = false;
            this.ExperienceGiven = new Range<double>(0, 0, 100, 100000);
            this.BehaviorDetails = new BehaviorDetailsTemplate();
            this.AttackAttributes = new List<AttackAttributeTemplate>();
            this.DeathAnimations = new List<AnimationTemplate>();
            this.ReligiousAffiliation = new ReligiousAffiliationTemplate();
        }
        public EnemyTemplate(DungeonObjectTemplate template) : base(template)
        {
            this.HasReligiousAffiliation = false;
            this.ExperienceGiven = new Range<double>(0, 0, 100, 100000);
            this.BehaviorDetails = new BehaviorDetailsTemplate();
            this.AttackAttributes = new List<AttackAttributeTemplate>();
            this.DeathAnimations = new List<AnimationTemplate>();
            this.ReligiousAffiliation = new ReligiousAffiliationTemplate();
        }
    }
}
