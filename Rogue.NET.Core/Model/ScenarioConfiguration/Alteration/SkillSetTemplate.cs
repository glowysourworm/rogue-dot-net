using ProtoBuf;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Alteration
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true)]
    public class SkillSetTemplate : DungeonObjectTemplate
    {
        private int _levelLearned;

        [ProtoMember(1, AsReference = true)]
        public List<SpellTemplate> Spells { get; set; }
        [ProtoMember(2)]
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

        public SkillSetTemplate()
        {
            this.Spells = new List<SpellTemplate>();
        }
        public SkillSetTemplate(DungeonObjectTemplate obj)
            : base(obj)
        {
            this.Spells = new List<SpellTemplate>();
        }
    }
}
