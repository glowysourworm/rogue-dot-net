using Rogue.NET.Core.Model.Enums;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class EnemyTemplate : NonPlayerCharacterTemplate
    {
        private bool _generateOnStep;
        private Range<double> _experienceGiven;

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

        public EnemyTemplate()
        {
            this.ExperienceGiven = new Range<double>(0, 0, 100, 100000);
            this.BehaviorDetails = new BehaviorDetailsTemplate();
            this.AlignmentType = CharacterAlignmentType.EnemyAligned;
        }
    }
}
