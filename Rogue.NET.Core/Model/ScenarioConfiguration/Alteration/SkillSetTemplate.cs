using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    [Serializable]
    public class SkillSetTemplate : DungeonObjectTemplate
    {
        private int _levelLearned;
        private bool _hasReligionRequirement;
        private ReligionTemplate _religion;

        public List<SkillTemplate> Skills { get; set; }

        public int LevelLearned
        {
            get { return _levelLearned; }
            set
            {
                if (_levelLearned != value)
                {
                    _levelLearned = value;
                    OnPropertyChanged("LevelLearned");
                }
            }
        }
        public bool HasReligionRequirement
        {
            get { return _hasReligionRequirement; }
            set
            {
                if (_hasReligionRequirement != value)
                {
                    _hasReligionRequirement = value;
                    OnPropertyChanged("HasReligionRequirement");
                }
            }
        }
        public ReligionTemplate Religion
        {
            get { return _religion; }
            set
            {
                if (_religion != value)
                {
                    _religion = value;
                    OnPropertyChanged("Religion");
                }
            }
        }

        public SkillSetTemplate()
        {
            this.Skills = new List<SkillTemplate>();
            this.HasReligionRequirement = false;
        }
        public SkillSetTemplate(DungeonObjectTemplate obj)
            : base(obj)
        {
            this.Skills = new List<SkillTemplate>();
            this.HasReligionRequirement = false;
        }
    }
}
