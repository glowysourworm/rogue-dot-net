using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class EnemyTemplate : CharacterTemplate
    {
        public List<AttackAttributeTemplate> AttackAttributes { get; set; }

        private DungeonObjectTemplate _creatureClass;
        private Range<double> _experienceGiven;
        private BehaviorDetailsTemplate _behaviorDetails;

        public DungeonObjectTemplate CreatureClass
        {
            get { return _creatureClass; }
            set
            {
                if (_creatureClass != value)
                {
                    _creatureClass = value;
                    OnPropertyChanged("CreatureClass");
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

        public EnemyTemplate()
        {
            this.ExperienceGiven = new Range<double>(0, 0, 100, 100000);
            this.BehaviorDetails = new BehaviorDetailsTemplate();
            this.AttackAttributes = new List<AttackAttributeTemplate>();
            this.CreatureClass = new DungeonObjectTemplate();
        }
        public EnemyTemplate(DungeonObjectTemplate template) : base(template)
        {
            this.ExperienceGiven = new Range<double>(0, 0, 100, 100000);
            this.BehaviorDetails = new BehaviorDetailsTemplate();
            this.AttackAttributes = new List<AttackAttributeTemplate>();
            this.CreatureClass = new DungeonObjectTemplate();
        }
    }
}
