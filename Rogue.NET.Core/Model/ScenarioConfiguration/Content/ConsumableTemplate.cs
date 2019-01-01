using ProtoBuf;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;

using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true)]
    public class ConsumableTemplate : DungeonObjectTemplate
    {
        private ConsumableType _type;
        private ConsumableSubType _subType;
        private double _weight;
        private int _levelRequired;
        private Range<int> _useCount;
        private bool _hasLearnedSkill;
        private bool _hasSpell;
        private bool _isProjectile;
        private bool _identifyOnUse;
        private SpellTemplate _spellTemplate;
        private SkillSetTemplate _learnedSkill;
        private SpellTemplate _projectileSpellTemplate;
        private SpellTemplate _ammoSpellTemplate;
        private string _noteMessage;

        [ProtoMember(1)]
        public ConsumableType Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged("Type");
                }
            }
        }
        [ProtoMember(2)]
        public ConsumableSubType SubType
        {
            get { return _subType; }
            set
            {
                if (_subType != value)
                {
                    _subType = value;
                    OnPropertyChanged("SubType");
                }
            }
        }
        [ProtoMember(3)]
        public double Weight
        {
            get { return _weight; }
            set
            {
                if (_weight != value)
                {
                    _weight = value;
                    OnPropertyChanged("Weight");
                }
            }
        }
        [ProtoMember(4)]
        public int LevelRequired
        {
            get { return _levelRequired; }
            set
            {
                if (_levelRequired != value)
                {
                    _levelRequired = value;
                    OnPropertyChanged("LevelRequired");
                }
            }
        }
        [ProtoMember(5)]
        public Range<int> UseCount
        {
            get { return _useCount; }
            set
            {
                if (_useCount != value)
                {
                    _useCount = value;
                    OnPropertyChanged("UseCount");
                }
            }
        }
        [ProtoMember(6)]
        public bool HasLearnedSkill
        {
            get { return _hasLearnedSkill; }
            set
            {
                if (_hasLearnedSkill != value)
                {
                    _hasLearnedSkill = value;
                    OnPropertyChanged("HasLearnedSkill");
                }
            }
        }
        [ProtoMember(7)]
        public bool HasSpell
        {
            get { return _hasSpell; }
            set
            {
                if (_hasSpell != value)
                {
                    _hasSpell = value;
                    OnPropertyChanged("HasSpell");
                }
            }
        }
        [ProtoMember(8)]
        public bool IsProjectile
        {
            get { return _isProjectile; }
            set
            {
                if (_isProjectile != value)
                {
                    _isProjectile = value;
                    OnPropertyChanged("IsProjectile");
                }
            }
        }
        [ProtoMember(9)]
        public bool IdentifyOnUse
        {
            get { return _identifyOnUse; }
            set
            {
                if (_identifyOnUse != value)
                {
                    _identifyOnUse = value;
                    OnPropertyChanged("IdentifyOnUse");
                }
            }
        }
        [ProtoMember(10, AsReference = true)]
        public SpellTemplate SpellTemplate
        {
            get { return _spellTemplate; }
            set
            {
                if (_spellTemplate != value)
                {
                    _spellTemplate = value;
                    OnPropertyChanged("SpellTemplate");
                }
            }
        }
        [ProtoMember(11, AsReference = true)]
        public SkillSetTemplate LearnedSkill
        {
            get { return _learnedSkill; }
            set
            {
                if (_learnedSkill != value)
                {
                    _learnedSkill = value;
                    OnPropertyChanged("LearnedSkill");
                }
            }
        }
        [ProtoMember(12, AsReference = true)]
        public SpellTemplate ProjectileSpellTemplate
        {
            get { return _projectileSpellTemplate; }
            set
            {
                if (_projectileSpellTemplate != value)
                {
                    _projectileSpellTemplate = value;
                    OnPropertyChanged("ProjectileSpellTemplate");
                }
            }
        }
        [ProtoMember(13, AsReference = true)]
        public SpellTemplate AmmoSpellTemplate
        {
            get { return _ammoSpellTemplate; }
            set
            {
                if (_ammoSpellTemplate != value)
                {
                    _ammoSpellTemplate = value;
                    OnPropertyChanged("AmmoSpellTemplate");
                }
            }
        }
        [ProtoMember(14)]
        public string NoteMessage
        {
            get { return _noteMessage; }
            set
            {
                if (_noteMessage != value)
                {
                    _noteMessage = value;
                    OnPropertyChanged("NoteMessage");
                }
            }
        }

        public ConsumableTemplate()
        {
            this.Type = ConsumableType.OneUse;
            this.SubType = ConsumableSubType.Food;
            this.ProjectileSpellTemplate = new SpellTemplate();
            this.SpellTemplate = new SpellTemplate();
            this.AmmoSpellTemplate = new SpellTemplate();
            this.LearnedSkill = new SkillSetTemplate();
            this.UseCount = new Range<int>(0, 0, 0, 20);
            this.IsObjectiveItem = false;
            this.IsUnique = false;
            this.IsProjectile = false;
            this.IdentifyOnUse = false;
            this.NoteMessage = "";
        }
        public ConsumableTemplate(DungeonObjectTemplate tmp) : base(tmp)
        {
            this.Type = ConsumableType.OneUse;
            this.SubType = ConsumableSubType.Food;
            this.ProjectileSpellTemplate = new SpellTemplate();
            this.SpellTemplate = new SpellTemplate();
            this.AmmoSpellTemplate = new SpellTemplate();
            this.LearnedSkill = new SkillSetTemplate();
            this.UseCount = new Range<int>(0, 0, 0, 20);
            this.IsObjectiveItem = false;
            this.IsUnique = false;
            this.IsProjectile = false;
            this.IdentifyOnUse = false;
            this.NoteMessage = "";
        }
    }
}
