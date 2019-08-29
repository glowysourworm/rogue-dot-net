using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.Core.Model.ScenarioConfiguration.Animation;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class ConsumableTemplate : DungeonObjectTemplate
    {
        private ConsumableType _type;
        private ConsumableSubType _subType;
        private double _weight;
        private int _levelRequired;
        private Range<int> _useCount;
        private bool _hasLearnedSkill;
        private bool _hasAlteration;
        private bool _hasProjectileAlteration;
        private bool _identifyOnUse;
        private SkillSetTemplate _learnedSkill;
        private ConsumableAlterationTemplate _consumableAlteration;
        private ConsumableProjectileAlterationTemplate _consumableProjectileAlteration;
        private AnimationGroupTemplate _ammoAnimationGroup;
        private string _noteMessage;

        private bool _hasCharacterClassRequirement;

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
        public bool HasAlteration
        {
            get { return _hasAlteration; }
            set
            {
                if (_hasAlteration != value)
                {
                    _hasAlteration = value;
                    OnPropertyChanged("HasAlteration");
                }
            }
        }
        public bool HasProjectileAlteration
        {
            get { return _hasProjectileAlteration; }
            set
            {
                if (_hasProjectileAlteration != value)
                {
                    _hasProjectileAlteration = value;
                    OnPropertyChanged("HasProjectileAlteration");
                }
            }
        }
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
        public ConsumableAlterationTemplate ConsumableAlteration
        {
            get { return _consumableAlteration; }
            set
            {
                if (_consumableAlteration != value)
                {
                    _consumableAlteration = value;
                    OnPropertyChanged("ConsumableAlteration");
                }
            }
        }
        public ConsumableProjectileAlterationTemplate ConsumableProjectileAlteration
        {
            get { return _consumableProjectileAlteration; }
            set
            {
                if (_consumableProjectileAlteration != value)
                {
                    _consumableProjectileAlteration = value;
                    OnPropertyChanged("ConsumableProjectileAlteration");
                }
            }
        }
        public AnimationGroupTemplate AmmoAnimationGroup
        {
            get { return _ammoAnimationGroup; }
            set
            {
                if (_ammoAnimationGroup != value)
                {
                    _ammoAnimationGroup = value;
                    OnPropertyChanged("AmmoAnimationGroup");
                }
            }
        }
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

        public bool HasCharacterClassRequirement
        {
            get { return _hasCharacterClassRequirement; }
            set
            {
                if (_hasCharacterClassRequirement != value)
                {
                    _hasCharacterClassRequirement = value;
                    OnPropertyChanged("HasCharacterClassRequirement");
                }
            }
        }

        public ConsumableTemplate()
        {
            this.Type = ConsumableType.OneUse;
            this.SubType = ConsumableSubType.Food;
            this.ConsumableAlteration = new ConsumableAlterationTemplate();
            this.ConsumableProjectileAlteration = new ConsumableProjectileAlterationTemplate();
            this.AmmoAnimationGroup = new AnimationGroupTemplate();
            this.LearnedSkill = new SkillSetTemplate();
            this.UseCount = new Range<int>(0, 0, 0, 20);
            this.IsObjectiveItem = false;
            this.IsUnique = false;
            this.IdentifyOnUse = false;
            this.NoteMessage = "";
        }
    }
}
