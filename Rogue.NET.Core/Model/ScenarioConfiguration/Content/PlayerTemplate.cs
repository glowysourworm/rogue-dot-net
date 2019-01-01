using ProtoBuf;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true)]
    public class PlayerTemplate : CharacterTemplate
    {
        private string _class;
        private double _auraRadius;
        private Range<double> _foodUsage;
        private List<SkillSetTemplate> _skills;

        [ProtoMember(1)]
        public string Class
        {
            get { return _class; }
            set
            {
                if (_class != value)
                {
                    _class = value;
                    OnPropertyChanged("Class");
                }
            }
        }
        [ProtoMember(2)]
        public double AuraRadius
        {
            get { return _auraRadius; }
            set
            {
                if (_auraRadius != value)
                {
                    _auraRadius = value;
                    OnPropertyChanged("AuraRadius");
                }
            }
        }
        [ProtoMember(3)]
        public Range<double> FoodUsage
        {
            get { return _foodUsage; }
            set
            {
                if (_foodUsage != value)
                {
                    _foodUsage = value;
                    OnPropertyChanged("FoodUsage");
                }
            }
        }
        [ProtoMember(4, AsReference = true)]
        public List<SkillSetTemplate> Skills
        {
            get { return _skills; }
            set
            {
                if (_skills != value)
                {
                    _skills = value;
                    OnPropertyChanged("Skills");
                }
            }
        }

        public PlayerTemplate()
        {
            this.Skills = new List<SkillSetTemplate>();
            this.FoodUsage = new Range<double>(0.0001, 0.005, 0.01, 1);
            this.Class = "Fighter";
            this.AuraRadius = 5;
            this.SymbolDetails.Type = SymbolTypes.Smiley;
        }
    }
}
