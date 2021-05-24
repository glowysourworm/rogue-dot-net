using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Design
{
    [Serializable]
    public class ScenarioTemplate : Template
    {
        private string _objectiveDescription;
        private int _skillPointsPerCharacterPoint;
        private int _healthPerCharacterPoint;
        private int _staminaPerCharacterPoint;
        private double _strengthPerCharacterPoint;
        private double _agilityPerCharacterPoint;
        private double _intelligencePerCharacterPoint;

        public string ObjectiveDescription
        {
            get { return _objectiveDescription; }
            set
            {
                if (_objectiveDescription != value)
                {
                    _objectiveDescription = value;
                    OnPropertyChanged("ObjectiveDescription");
                }
            }
        }
        public int SkillPointsPerCharacterPoint
        {
            get { return _skillPointsPerCharacterPoint; }
            set
            {
                if (_skillPointsPerCharacterPoint != value)
                {
                    _skillPointsPerCharacterPoint = value;
                    OnPropertyChanged("SkillPointsPerCharacterPoint");
                }
            }
        }
        public int HealthPerCharacterPoint
        {
            get { return _healthPerCharacterPoint; }
            set
            {
                if (_healthPerCharacterPoint != value)
                {
                    _healthPerCharacterPoint = value;
                    OnPropertyChanged("HealthPerCharacterPoint");
                }
            }
        }
        public int StaminaPerCharacterPoint
        {
            get { return _staminaPerCharacterPoint; }
            set
            {
                if (_staminaPerCharacterPoint != value)
                {
                    _staminaPerCharacterPoint = value;
                    OnPropertyChanged("StaminaPerCharacterPoint");
                }
            }
        }
        public double StrengthPerCharacterPoint
        {
            get { return _strengthPerCharacterPoint; }
            set
            {
                if (_strengthPerCharacterPoint != value)
                {
                    _strengthPerCharacterPoint = value;
                    OnPropertyChanged("StrengthPerCharacterPoint");
                }
            }
        }
        public double AgilityPerCharacterPoint
        {
            get { return _agilityPerCharacterPoint; }
            set
            {
                if (_agilityPerCharacterPoint != value)
                {
                    _agilityPerCharacterPoint = value;
                    OnPropertyChanged("AgilityPerCharacterPoint");
                }
            }
        }
        public double IntelligencePerCharacterPoint
        {
            get { return _intelligencePerCharacterPoint; }
            set
            {
                if (_intelligencePerCharacterPoint != value)
                {
                    _intelligencePerCharacterPoint = value;
                    OnPropertyChanged("IntelligencePerCharacterPoint");
                }
            }
        }


        public List<LevelTemplate> LevelDesigns { get; set; }


        public ScenarioTemplate()
        {
            this.LevelDesigns = new List<LevelTemplate>();
            this.ObjectiveDescription = "Objective Description (Goes Here)";

            this.HealthPerCharacterPoint = 5;
            this.StaminaPerCharacterPoint = 2;

            this.SkillPointsPerCharacterPoint = 1;

            this.AgilityPerCharacterPoint = 1;
            this.IntelligencePerCharacterPoint = 1;
            this.StrengthPerCharacterPoint = 1;
        }
    }
}
