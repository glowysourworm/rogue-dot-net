using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Linq;
using System.IO;
using Rogue.NET.Common;
using System.Runtime.Serialization;
using Rogue.NET.Common.Model;

namespace Rogue.NET.Model
{
    [Serializable]
    public class ScenarioConfiguration
    {
        public static int CELLHEIGHT = 15;
        public static int CELLWIDTH = 10;

        public DungeonTemplate DungeonTemplate { get; set; }
        public PlayerTemplate PlayerTemplate { get; set; }

        public List<SkillSetTemplate> SkillTemplates { get; set; }
        public List<BrushTemplate> BrushTemplates { get; set; }
        public List<PenTemplate> PenTemplates { get; set; }
        public List<EnemyTemplate> EnemyTemplates { get; set; }
        public List<AnimationTemplate> AnimationTemplates { get; set; }
        public List<EquipmentTemplate> EquipmentTemplates { get; set; }
        public List<ConsumableTemplate> ConsumableTemplates { get; set; }
        public List<SpellTemplate> MagicSpells { get; set; }
        public List<DoodadTemplate> DoodadTemplates { get; set; }
        public List<DungeonObjectTemplate> CharacterClasses { get; set; }
        public List<DungeonObjectTemplate> AttackAttributes { get; set; }

        public ScenarioConfiguration()
        {
            this.DungeonTemplate = new DungeonTemplate();
            this.MagicSpells = new List<SpellTemplate>();
            this.EnemyTemplates = new List<EnemyTemplate>();
            this.BrushTemplates = new List<BrushTemplate>();
            this.EquipmentTemplates = new List<EquipmentTemplate>();
            this.AnimationTemplates = new List<AnimationTemplate>();
            this.ConsumableTemplates = new List<ConsumableTemplate>();
            this.SkillTemplates = new List<SkillSetTemplate>();
            this.PlayerTemplate = new PlayerTemplate();
            this.DoodadTemplates = new List<DoodadTemplate>();
            this.PenTemplates = new List<PenTemplate>();
            this.CharacterClasses = new List<DungeonObjectTemplate>();
            this.AttackAttributes = new List<DungeonObjectTemplate>();
        }
    }

    #region Abstract
    [Serializable]
    public abstract class Template : INotifyPropertyChanged
    {
        public Template()
        {
            this.Name = "New Template";
            this.Guid = System.Guid.NewGuid().ToString();
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private string _name;
        private string _guid;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public string Guid
        {
            get { return _guid; }
            set
            {
                if (_guid != value)
                {
                    _guid = value;
                    OnPropertyChanged("Guid");
                }
            }
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }
    }
    [Serializable]
    public class DungeonObjectTemplate : Template
    {
	    private SymbolDetailsTemplate _symbolDetails;
	    private Range<int> _level;
	    private double _generationRate;
	    private string _shortDescription;
	    private string _longDescription;
	    private bool _isCursed;
	    private bool _isUnique;
	    private bool _isObjectiveItem;
	    private bool _hasBeenGenerated;

	    public SymbolDetailsTemplate SymbolDetails
	    {
		    get{ return _symbolDetails; }
		    set
		    {
			    if(_symbolDetails != value)
			    {
				    _symbolDetails = value;
				    OnPropertyChanged("SymbolDetails");
			    }
		    }
	    }
	    public Range<int> Level
	    {
		    get{ return _level; }
		    set
		    {
			    if(_level != value)
			    {
				    _level = value;
				    OnPropertyChanged("Level");
			    }
		    }
	    }
	    public double GenerationRate
	    {
		    get{ return _generationRate; }
		    set
		    {
			    if(_generationRate != value)
			    {
				    _generationRate = value;
				    OnPropertyChanged("GenerationRate");
			    }
		    }
	    }
	    public string ShortDescription
	    {
		    get{ return _shortDescription; }
		    set
		    {
			    if(_shortDescription != value)
			    {
				    _shortDescription = value;
				    OnPropertyChanged("ShortDescription");
			    }
		    }
	    }
	    public string LongDescription
	    {
		    get{ return _longDescription; }
		    set
		    {
			    if(_longDescription != value)
			    {
				    _longDescription = value;
				    OnPropertyChanged("LongDescription");
			    }
		    }
	    }
	    public bool IsCursed
	    {
		    get{ return _isCursed; }
		    set
		    {
			    if(_isCursed != value)
			    {
				    _isCursed = value;
				    OnPropertyChanged("IsCursed");
			    }
		    }
	    }
	    public bool IsUnique
	    {
		    get{ return _isUnique; }
		    set
		    {
			    if(_isUnique != value)
			    {
				    _isUnique = value;
				    OnPropertyChanged("IsUnique");
			    }
		    }
	    }
	    public bool IsObjectiveItem
	    {
		    get{ return _isObjectiveItem; }
		    set
		    {
			    if(_isObjectiveItem != value)
			    {
				    _isObjectiveItem = value;
				    OnPropertyChanged("IsObjectiveItem");
			    }
		    }
	    }
	    public bool HasBeenGenerated
	    {
		    get{ return _hasBeenGenerated; }
		    set
		    {
			    if(_hasBeenGenerated != value)
			    {
				    _hasBeenGenerated = value;
				    OnPropertyChanged("HasBeenGenerated");
			    }
		    }
	    }

        public DungeonObjectTemplate()
        {
            this.SymbolDetails = new SymbolDetailsTemplate();
            this.Level = new Range<int>(1, 1, 100, 200);

            this.ShortDescription = "";
            this.LongDescription = "";
        }
        public DungeonObjectTemplate(DungeonObjectTemplate tmp)
        {
            this.SymbolDetails = tmp.SymbolDetails;
            this.Level = tmp.Level;
            this.GenerationRate = tmp.GenerationRate;
            this.ShortDescription = tmp.ShortDescription;
            this.LongDescription = tmp.LongDescription;
            this.IsCursed = tmp.IsCursed;
            this.IsUnique = tmp.IsUnique;
            this.IsObjectiveItem = tmp.IsObjectiveItem;
        }
    }

    [Serializable]
    public class SymbolDetailsTemplate : Template
    {
        private SymbolTypes _type;
        private SmileyMoods _smileyMood;
        private string _smileyBodyColor;
        private string _smileyLineColor;
        private string _smileyAuraColor;
        private string _characterSymbol;
        private string _characterColor;
        private ImageResources _icon;
        private bool _isFullSymbolDelta;
        private bool _isImageDelta;
        private bool _isMoodDelta;
        private bool _isBodyDelta;
        private bool _isLineDelta;
        private bool _isAuraDelta;
        private bool _isCharacterDelta;
        private bool _isColorDelta;

        public SymbolTypes Type
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
        public SmileyMoods SmileyMood
        {
            get { return _smileyMood; }
            set
            {
                if (_smileyMood != value)
                {
                    _smileyMood = value;
                    OnPropertyChanged("SmileyMood");
                }
            }
        }
        public string SmileyBodyColor
        {
            get { return _smileyBodyColor; }
            set
            {
                if (_smileyBodyColor != value)
                {
                    _smileyBodyColor = value;
                    OnPropertyChanged("SmileyBodyColor");
                }
            }
        }
        public string SmileyLineColor
        {
            get { return _smileyLineColor; }
            set
            {
                if (_smileyLineColor != value)
                {
                    _smileyLineColor = value;
                    OnPropertyChanged("SmileyLineColor");
                }
            }
        }
        public string SmileyAuraColor
        {
            get { return _smileyAuraColor; }
            set
            {
                if (_smileyAuraColor != value)
                {
                    _smileyAuraColor = value;
                    OnPropertyChanged("SmileyAuraColor");
                }
            }
        }
        public string CharacterSymbol
        {
            get { return _characterSymbol; }
            set
            {
                if (_characterSymbol != value)
                {
                    _characterSymbol = value;
                    OnPropertyChanged("CharacterSymbol");
                }
            }
        }
        public string CharacterColor
        {
            get { return _characterColor; }
            set
            {
                if (_characterColor != value)
                {
                    _characterColor = value;
                    OnPropertyChanged("CharacterColor");
                }
            }
        }
        public ImageResources Icon
        {
            get { return _icon; }
            set
            {
                if (_icon != value)
                {
                    _icon = value;
                    OnPropertyChanged("Icon");
                }
            }
        }
        public bool IsFullSymbolDelta
        {
            get { return _isFullSymbolDelta; }
            set
            {
                if (_isFullSymbolDelta != value)
                {
                    _isFullSymbolDelta = value;
                    OnPropertyChanged("IsFullSymbolDelta");
                }
            }
        }
        public bool IsImageDelta
        {
            get { return _isImageDelta; }
            set
            {
                if (_isImageDelta != value)
                {
                    _isImageDelta = value;
                    OnPropertyChanged("IsImageDelta");
                }
            }
        }
        public bool IsMoodDelta
        {
            get { return _isMoodDelta; }
            set
            {
                if (_isMoodDelta != value)
                {
                    _isMoodDelta = value;
                    OnPropertyChanged("IsMoodDelta");
                }
            }
        }
        public bool IsBodyDelta
        {
            get { return _isBodyDelta; }
            set
            {
                if (_isBodyDelta != value)
                {
                    _isBodyDelta = value;
                    OnPropertyChanged("IsBodyDelta");
                }
            }
        }
        public bool IsLineDelta
        {
            get { return _isLineDelta; }
            set
            {
                if (_isLineDelta != value)
                {
                    _isLineDelta = value;
                    OnPropertyChanged("IsLineDelta");
                }
            }
        }
        public bool IsAuraDelta
        {
            get { return _isAuraDelta; }
            set
            {
                if (_isAuraDelta != value)
                {
                    _isAuraDelta = value;
                    OnPropertyChanged("IsAuraDelta");
                }
            }
        }
        public bool IsCharacterDelta
        {
            get { return _isCharacterDelta; }
            set
            {
                if (_isCharacterDelta != value)
                {
                    _isCharacterDelta = value;
                    OnPropertyChanged("IsCharacterDelta");
                }
            }
        }
        public bool IsColorDelta
        {
            get { return _isColorDelta; }
            set
            {
                if (_isColorDelta != value)
                {
                    _isColorDelta = value;
                    OnPropertyChanged("IsColorDelta");
                }
            }
        }

        public SymbolDetailsTemplate()
        {
            this.Type = SymbolTypes.Image;
            this.Icon = ImageResources.AmuletOrange;

            this.SmileyBodyColor = Colors.Yellow.ToString();
            this.SmileyLineColor = Colors.Black.ToString();
            this.SmileyAuraColor = Colors.Yellow.ToString();

            this.CharacterColor = Colors.White.ToString();
            this.CharacterSymbol = "T";
        }
        public SymbolDetailsTemplate(SymbolDetailsTemplate tmp)
        {
            this.Type = tmp.Type;
            this.Icon = tmp.Icon;
            this.SmileyAuraColor = tmp.SmileyAuraColor;
            this.SmileyBodyColor = tmp.SmileyBodyColor;
            this.SmileyLineColor = tmp.SmileyLineColor;
            this.SmileyMood = tmp.SmileyMood;
            this.Type = tmp.Type;
        }
    }
    #endregion

    #region Layout
    [Serializable]
    public class DungeonTemplate : Template
    {
        private int _numberOfLevels;
        private double _monsterGenerationBase;
        private double _partyRoomGenerationRate;
        private string _objectiveDescription;

        public int NumberOfLevels
        {
            get { return _numberOfLevels; }
            set
            {
                if (_numberOfLevels != value)
                {
                    _numberOfLevels = value;
                    OnPropertyChanged("NumberOfLevels");
                }
            }
        }
        public double MonsterGenerationBase
        {
            get { return _monsterGenerationBase; }
            set
            {
                if (_monsterGenerationBase != value)
                {
                    _monsterGenerationBase = value;
                    OnPropertyChanged("MonsterGenerationBase");
                }
            }
        }
        public double PartyRoomGenerationRate
        {
            get { return _partyRoomGenerationRate; }
            set
            {
                if (_partyRoomGenerationRate != value)
                {
                    _partyRoomGenerationRate = value;
                    OnPropertyChanged("PartyRoomGenerationRate");
                }
            }
        }
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


        public List<LayoutTemplate> LayoutTemplates { get; set; }

        public DungeonTemplate()
        {
            this.LayoutTemplates = new List<LayoutTemplate>();

            this.NumberOfLevels = 100;
            this.MonsterGenerationBase = 0.01;
            this.PartyRoomGenerationRate = 0.1;
        }
    }
    [Serializable]
    public class LayoutTemplate : Template
    {
        private int _numberRoomRows;
        private int _numberRoomCols;
        private int _roomDivCellHeight;
        private int _roomDivCellWidth;
        private int _numberExtraWallRemovals;
        private double _hiddenDoorProbability;
        private double _generationRatio;
        private LayoutType _type;
        private Range<int> _levelRange;

        public int NumberRoomRows
        {
            get { return _numberRoomRows; }
            set
            {
                if (_numberRoomRows != value)
                {
                    _numberRoomRows = value;
                    OnPropertyChanged("NumberRoomRows");
                }
            }
        }
        public int NumberRoomCols
        {
            get { return _numberRoomCols; }
            set
            {
                if (_numberRoomCols != value)
                {
                    _numberRoomCols = value;
                    OnPropertyChanged("NumberRoomCols");
                }
            }
        }
        public int RoomDivCellHeight
        {
            get { return _roomDivCellHeight; }
            set
            {
                if (_roomDivCellHeight != value)
                {
                    _roomDivCellHeight = value;
                    OnPropertyChanged("RoomDivCellHeight");
                }
            }
        }
        public int RoomDivCellWidth
        {
            get { return _roomDivCellWidth; }
            set
            {
                if (_roomDivCellWidth != value)
                {
                    _roomDivCellWidth = value;
                    OnPropertyChanged("RoomDivCellWidth");
                }
            }
        }
        public int NumberExtraWallRemovals
        {
            get { return _numberExtraWallRemovals; }
            set
            {
                if (_numberExtraWallRemovals != value)
                {
                    _numberExtraWallRemovals = value;
                    OnPropertyChanged("NumberExtraWallRemovals");
                }
            }
        }
        public double HiddenDoorProbability
        {
            get { return _hiddenDoorProbability; }
            set
            {
                if (_hiddenDoorProbability != value)
                {
                    _hiddenDoorProbability = value;
                    OnPropertyChanged("HiddenDoorProbability");
                }
            }
        }
        public double GenerationRate
        {
            get { return _generationRatio; }
            set
            {
                if (_generationRatio != value)
                {
                    _generationRatio = value;
                    OnPropertyChanged("GenerationRate");
                }
            }
        }
        public LayoutType Type
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
        public Range<int> Level
        {
            get { return _levelRange; }
            set
            {
                if (_levelRange != value)
                {
                    _levelRange = value;
                    OnPropertyChanged("Level");
                }
            }
        }

        public LayoutTemplate() : base()
        {
            this.Type = LayoutType.Normal;
            this.Level = new Range<int>(1, 1, 100, 100);
            this.NumberRoomRows = 3;
            this.NumberRoomCols = 3;
            this.RoomDivCellHeight = 20;
            this.RoomDivCellWidth = 20;
            this.NumberExtraWallRemovals = 200;
            this.HiddenDoorProbability = 0.2;
            this.GenerationRate = 0.5;
        }
    }
    #endregion

    #region Dungeon Content
    [Serializable]
    public class CharacterTemplate : DungeonObjectTemplate
    {
        public List<ProbabilityEquipmentTemplate> StartingEquipment { get; set; }
        public List<ProbabilityConsumableTemplate> StartingConsumables { get; set; }

        private Range<double> _strength;
        private Range<double> _agility;
        private Range<double> _intelligence;
        private Range<double> _hp;
        private Range<double> _mp;

        public Range<double> Strength
        {
            get { return _strength; }
            set
            {
                if (_strength != value)
                {
                    _strength = value;
                    OnPropertyChanged("Strength");
                }
            }
        }
        public Range<double> Agility
        {
            get { return _agility; }
            set
            {
                if (_agility != value)
                {
                    _agility = value;
                    OnPropertyChanged("Agility");
                }
            }
        }
        public Range<double> Intelligence
        {
            get { return _intelligence; }
            set
            {
                if (_intelligence != value)
                {
                    _intelligence = value;
                    OnPropertyChanged("Intelligence");
                }
            }
        }
        public Range<double> Hp
        {
            get { return _hp; }
            set
            {
                if (_hp != value)
                {
                    _hp = value;
                    OnPropertyChanged("Hp");
                }
            }
        }
        public Range<double> Mp
        {
            get { return _mp; }
            set
            {
                if (_mp != value)
                {
                    _mp = value;
                    OnPropertyChanged("Mp");
                }
            }
        }

        public CharacterTemplate()
        {
            this.Strength = new Range<double>(1, 3, 5, 100);
            this.Agility = new Range<double>(1, 4, 5, 100);
            this.Intelligence = new Range<double>(1, 2, 3, 100);
            this.Hp = new Range<double>(1, 10, 20, 100);
            this.Mp = new Range<double>(1, 2, 5, 100);

            this.StartingConsumables = new List<ProbabilityConsumableTemplate>();
            this.StartingEquipment = new List<ProbabilityEquipmentTemplate>();
        }
        public CharacterTemplate(DungeonObjectTemplate tmp) : base(tmp)
        {
            this.Strength = new Range<double>(1, 3, 5, 100);
            this.Agility = new Range<double>(1, 4, 5, 100);
            this.Intelligence = new Range<double>(1, 2, 3, 100);
            this.Hp = new Range<double>(1, 10, 20, 100);
            this.Mp = new Range<double>(1, 2, 5, 100);

            this.StartingConsumables = new List<ProbabilityConsumableTemplate>();
            this.StartingEquipment = new List<ProbabilityEquipmentTemplate>();
        }
    }
    [Serializable]
    public class PlayerTemplate : CharacterTemplate
    {
        private string _class;
        private double _auraRadius;
        private Range<double> _foodUsage;
        private List<SkillSetTemplate> _skills;

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
    [Serializable]
    public class EquipmentTemplate : DungeonObjectTemplate
    {
        public List<AttackAttributeTemplate> AttackAttributes { get; set; }

        private Range<int> _class;
        private Range<double> _quality;
        private EquipmentType _type;
        private SpellTemplate _equipSpell;
        private SpellTemplate _attackSpell;
        private SpellTemplate _curseSpell;
        private ConsumableTemplate _ammoTemplate;
        private double _weight;
        private bool _hasEquipSpell;
        private bool _hasAttackSpell;
        private bool _hasCurseSpell;

        public Range<int> Class
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
        public Range<double> Quality
        {
            get { return _quality; }
            set
            {
                if (_quality != value)
                {
                    _quality = value;
                    OnPropertyChanged("Quality");
                }
            }
        }
        public EquipmentType Type
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
        public SpellTemplate EquipSpell
        {
            get { return _equipSpell; }
            set
            {
                if (_equipSpell != value)
                {
                    _equipSpell = value;
                    OnPropertyChanged("EquipSpell");
                }
            }
        }
        public SpellTemplate AttackSpell
        {
            get { return _attackSpell; }
            set
            {
                if (_attackSpell != value)
                {
                    _attackSpell = value;
                    OnPropertyChanged("AttackSpell");
                }
            }
        }
        public SpellTemplate CurseSpell
        {
            get { return _curseSpell; }
            set
            {
                if (_curseSpell != value)
                {
                    _curseSpell = value;
                    OnPropertyChanged("CurseSpell");
                }
            }
        }
        public ConsumableTemplate AmmoTemplate
        {
            get { return _ammoTemplate; }
            set
            {
                if (_ammoTemplate != value)
                {
                    _ammoTemplate = value;
                    OnPropertyChanged("AmmoTemplate");
                }
            }
        }
        public bool HasEquipSpell
        {
            get { return _hasEquipSpell; }
            set
            {
                if (_hasEquipSpell != value)
                {
                    _hasEquipSpell = value;
                    OnPropertyChanged("HasEquipSpell");
                }
            }
        }
        public bool HasAttackSpell
        {
            get { return _hasAttackSpell; }
            set
            {
                if (_hasAttackSpell != value)
                {
                    _hasAttackSpell = value;
                    OnPropertyChanged("HasAttackSpell");
                }
            }
        }
        public bool HasCurseSpell
        {
            get { return _hasCurseSpell; }
            set
            {
                if (_hasCurseSpell != value)
                {
                    _hasCurseSpell = value;
                    OnPropertyChanged("HasCurseSpell");
                }
            }
        }


        public EquipmentTemplate()
        {
            this.Class = new Range<int>(0, 10);
            this.Type = EquipmentType.Ring;
            this.Quality = new Range<double>(0, 0, 100, 100);
            this.EquipSpell = new SpellTemplate();
            this.AttackSpell = new SpellTemplate();
            this.CurseSpell = new SpellTemplate();
            this.AmmoTemplate = new ConsumableTemplate();
            this.AttackAttributes = new List<AttackAttributeTemplate>();
        }
        public EquipmentTemplate(DungeonObjectTemplate tmp)
            : base(tmp)
        {
            this.Class = new Range<int>(0, 10);
            this.Type = EquipmentType.Ring;
            this.Quality = new Range<double>(0, 0, 100, 100);
            this.EquipSpell = new SpellTemplate();
            this.AttackSpell = new SpellTemplate();
            this.CurseSpell = new SpellTemplate();
            this.AmmoTemplate = new ConsumableTemplate();
            this.AttackAttributes = new List<AttackAttributeTemplate>();
        }
    }
    [Serializable]
    public class ConsumableTemplate : DungeonObjectTemplate
    {
        private ConsumableType _type;
        private ConsumableSubType _subType;
        private double _weight;
        private Range<int> _useCount;
        private bool _hasLearnedSkill;
        private bool _hasSpell;
        private bool _isProjectile;
        private SpellTemplate _spellTemplate;
        private SkillSetTemplate _learnedSkill;
        private SpellTemplate _projectileSpellTemplate;
        private SpellTemplate _ammoSpellTemplate;

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
        }
    }
    [Serializable]
    public class DoodadTemplate : DungeonObjectTemplate
    {
        private SpellTemplate _automaticMagicSpellTemplate;
        private SpellTemplate _invokedMagicSpellTemplate;
        private bool _isAutomatic;
        private bool _isVisible;
        private bool _isInvoked;
        private bool _isOneUse;

        public SpellTemplate AutomaticMagicSpellTemplate
        {
            get { return _automaticMagicSpellTemplate; }
            set
            {
                if (_automaticMagicSpellTemplate != value)
                {
                    _automaticMagicSpellTemplate = value;
                    OnPropertyChanged("AutomaticMagicSpellTemplate");
                }
            }
        }
        public SpellTemplate InvokedMagicSpellTemplate
        {
            get { return _invokedMagicSpellTemplate; }
            set
            {
                if (_invokedMagicSpellTemplate != value)
                {
                    _invokedMagicSpellTemplate = value;
                    OnPropertyChanged("InvokedMagicSpellTemplate");
                }
            }
        }
        public bool IsAutomatic
        {
            get { return _isAutomatic; }
            set
            {
                if (_isAutomatic != value)
                {
                    _isAutomatic = value;
                    OnPropertyChanged("IsAutomatic");
                }
            }
        }
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }
        public bool IsInvoked
        {
            get { return _isInvoked; }
            set
            {
                if (_isInvoked != value)
                {
                    _isInvoked = value;
                    OnPropertyChanged("IsInvoked");
                }
            }
        }
        public bool IsOneUse
        {
            get { return _isOneUse; }
            set
            {
                if (_isOneUse != value)
                {
                    _isOneUse = value;
                    OnPropertyChanged("IsOneUse");
                }
            }
        }
	
        public DoodadTemplate()
        {
            this.AutomaticMagicSpellTemplate = new SpellTemplate();
            this.InvokedMagicSpellTemplate = new SpellTemplate();
            this.IsUnique = false;
            this.IsOneUse = false;
        }
        public DoodadTemplate(DungeonObjectTemplate tmp) : base(tmp)
        {
            this.AutomaticMagicSpellTemplate = new SpellTemplate();
            this.InvokedMagicSpellTemplate = new SpellTemplate();
            this.IsUnique = false;
            this.IsOneUse = false;
        }
    }
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
    [Serializable]
    public class ProbabilityConsumableTemplate : Template
    {
        private Template _theTemplate;
        private double _generationProbability;

        public Template TheTemplate
        {
            get { return _theTemplate; }
            set
            {
                if (_theTemplate != value)
                {
                    _theTemplate = value;
                    OnPropertyChanged("TheTemplate");
                }
            }
        }
        public double GenerationProbability
        {
            get { return _generationProbability; }
            set
            {
                if (_generationProbability != value)
                {
                    _generationProbability = value;
                    OnPropertyChanged("GenerationProbability");
                }
            }
        }

        public ProbabilityConsumableTemplate()
        {
            this.TheTemplate = new ConsumableTemplate();
        }
    }
    [Serializable]
    public class ProbabilityEquipmentTemplate : Template
    {
        private Template _theTemplate;
        private double _generationProbability;
        private bool _equipOnStartup;

        public Template TheTemplate
        {
            get { return _theTemplate; }
            set
            {
                if (_theTemplate != value)
                {
                    _theTemplate = value;
                    OnPropertyChanged("TheTemplate");
                }
            }
        }
        public double GenerationProbability
        {
            get { return _generationProbability; }
            set
            {
                if (_generationProbability != value)
                {
                    _generationProbability = value;
                    OnPropertyChanged("GenerationProbability");
                }
            }
        }
        public bool EquipOnStartup
        {
            get { return _equipOnStartup; }
            set
            {
                _equipOnStartup = value;
                OnPropertyChanged("EquipOnStartup");
            }
        }

        public ProbabilityEquipmentTemplate()
        {
            this.TheTemplate = new EquipmentTemplate();
        }
    }
    [Serializable]
    public class BehaviorTemplate : Template
    {
        private CharacterMovementType _movementType;
        private CharacterAttackType _attackType;
        private SpellTemplate _enemySpell;
        private bool _canOpenDoors;
        private double _engageRadius;
        private double _disengageRadius;
        private double _criticalRatio;
        private double _counterAttackProbability;

        public CharacterMovementType MovementType
        {
            get { return _movementType; }
            set
            {
                if (_movementType != value)
                {
                    _movementType = value;
                    OnPropertyChanged("MovementType");
                }
            }
        }
        public CharacterAttackType AttackType
        {
            get { return _attackType; }
            set
            {
                if (_attackType != value)
                {
                    _attackType = value;
                    OnPropertyChanged("AttackType");
                }
            }
        }
        public SpellTemplate EnemySpell
        {
            get { return _enemySpell; }
            set
            {
                if (_enemySpell != value)
                {
                    _enemySpell = value;
                    OnPropertyChanged("EnemySpell");
                }
            }
        }
        public bool CanOpenDoors
        {
            get { return _canOpenDoors; }
            set
            {
                if (_canOpenDoors != value)
                {
                    _canOpenDoors = value;
                    OnPropertyChanged("CanOpenDoors");
                }
            }
        }
        public double EngageRadius
        {
            get { return _engageRadius; }
            set
            {
                if (_engageRadius != value)
                {
                    _engageRadius = value;
                    OnPropertyChanged("EngageRadius");
                }
            }
        }
        public double DisengageRadius
        {
            get { return _disengageRadius; }
            set
            {
                if (_disengageRadius != value)
                {
                    _disengageRadius = value;
                    OnPropertyChanged("DisengageRadius");
                }
            }
        }
        public double CriticalRatio
        {
            get { return _criticalRatio; }
            set
            {
                if (_criticalRatio != value)
                {
                    _criticalRatio = value;
                    OnPropertyChanged("CriticalRatio");
                }
            }
        }
        public double CounterAttackProbability
        {
            get { return _counterAttackProbability; }
            set
            {
                if (_counterAttackProbability != value)
                {
                    _counterAttackProbability = value;
                    OnPropertyChanged("CounterAttackProbability");
                }
            }
        }

        public BehaviorTemplate()
        {
            this.EnemySpell = new SpellTemplate();
        }
    }
    [Serializable]
    public class BehaviorDetailsTemplate : Template
    {
        private BehaviorTemplate _primaryBehavior;
        private BehaviorTemplate _secondaryBehavior;
        private SecondaryBehaviorInvokeReason _secondaryReason;
        private double _secondaryProbability;

        public BehaviorTemplate PrimaryBehavior
        {
            get { return _primaryBehavior; }
            set
            {
                if (_primaryBehavior != value)
                {
                    _primaryBehavior = value;
                    OnPropertyChanged("PrimaryBehavior");
                }
            }
        }
        public BehaviorTemplate SecondaryBehavior
        {
            get { return _secondaryBehavior; }
            set
            {
                if (_secondaryBehavior != value)
                {
                    _secondaryBehavior = value;
                    OnPropertyChanged("SecondaryBehavior");
                }
            }
        }
        public SecondaryBehaviorInvokeReason SecondaryReason
        {
            get { return _secondaryReason; }
            set
            {
                if (_secondaryReason != value)
                {
                    _secondaryReason = value;
                    OnPropertyChanged("SecondaryReason");
                }
            }
        }
        public double SecondaryProbability
        {
            get { return _secondaryProbability; }
            set
            {
                if (_secondaryProbability != value)
                {
                    _secondaryProbability = value;
                    OnPropertyChanged("SecondaryProbability");
                }
            }
        }

        public BehaviorDetailsTemplate()
        {
            this.PrimaryBehavior = new BehaviorTemplate();
            this.SecondaryBehavior = new BehaviorTemplate();
        }
    }
    #endregion

    #region Skills / Alterations / Magic
    [Serializable]
    public class AlterationCostTemplate : Template
    {
        private AlterationCostType _type;
        private double _strength;
        private double _intelligence;
        private double _agility;
        private double _foodUsagePerTurn;
        private double _auraRadius;
        private double _experience;
        private double _hunger;
        private double _hp;
        private double _mp;
        private double _hpPerStep;
        private double _mpPerStep;
        private double _hungerPerStep;

        public AlterationCostType Type
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
        public double Strength
        {
            get { return _strength; }
            set
            {
                if (_strength != value)
                {
                    _strength = value;
                    OnPropertyChanged("Strength");
                }
            }
        }
        public double Intelligence
        {
            get { return _intelligence; }
            set
            {
                if (_intelligence != value)
                {
                    _intelligence = value;
                    OnPropertyChanged("Intelligence");
                }
            }
        }
        public double Agility
        {
            get { return _agility; }
            set
            {
                if (_agility != value)
                {
                    _agility = value;
                    OnPropertyChanged("Agility");
                }
            }
        }
        public double FoodUsagePerTurn
        {
            get { return _foodUsagePerTurn; }
            set
            {
                if (_foodUsagePerTurn != value)
                {
                    _foodUsagePerTurn = value;
                    OnPropertyChanged("FoodUsagePerTurn");
                }
            }
        }
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
        public double Experience
        {
            get { return _experience; }
            set
            {
                if (_experience != value)
                {
                    _experience = value;
                    OnPropertyChanged("Experience");
                }
            }
        }
        public double Hunger
        {
            get { return _hunger; }
            set
            {
                if (_hunger != value)
                {
                    _hunger = value;
                    OnPropertyChanged("Hunger");
                }
            }
        }
        public double Hp
        {
            get { return _hp; }
            set
            {
                if (_hp != value)
                {
                    _hp = value;
                    OnPropertyChanged("Hp");
                }
            }
        }
        public double Mp
        {
            get { return _mp; }
            set
            {
                if (_mp != value)
                {
                    _mp = value;
                    OnPropertyChanged("Mp");
                }
            }
        }
        public double HpPerStep
        {
            get { return _hpPerStep; }
            set
            {
                if (_hpPerStep != value)
                {
                    _hpPerStep = value;
                    OnPropertyChanged("HpPerStep");
                }
            }
        }
        public double MpPerStep
        {
            get { return _mpPerStep; }
            set
            {
                if (_mpPerStep != value)
                {
                    _mpPerStep = value;
                    OnPropertyChanged("MpPerStep");
                }
            }
        }
        public double HungerPerStep
        {
            get { return _hungerPerStep; }
            set
            {
                if (_hungerPerStep != value)
                {
                    _hungerPerStep = value;
                    OnPropertyChanged("HungerPerStep");
                }
            }
        }

    }
    [Serializable]
    public class AlterationEffectTemplate : Template
    {
        private SymbolDetailsTemplate _symbolAlteration;
        private bool _isSymbolAlteration;
        private Range<int> _eventTime;
        private CharacterStateType _stateType;
        private string _postEffectText;
        private Range<double> _strengthRange;
        private Range<double> _intelligenceRange;
        private Range<double> _agilityRange;
        private Range<double> _auraRadiusRange;
        private Range<double> _foodUsagePerTurnRange;
        private Range<double> _hpPerStepRange;
        private Range<double> _mpPerStepRange;
        private Range<double> _attackRange;
        private Range<double> _defenseRange;
        private Range<double> _magicBlockProbabilityRange;
        private Range<double> _dodgeProbabilityRange;
        private Range<double> _experienceRange;
        private Range<double> _hungerRange;
        private Range<double> _hpRange;
        private Range<double> _mpRange;
        private Range<double> _criticalHit;
        private bool _isSilence;

        public SymbolDetailsTemplate SymbolAlteration
        {
            get { return _symbolAlteration; }
            set
            {
                if (_symbolAlteration != value)
                {
                    _symbolAlteration = value;
                    OnPropertyChanged("SymbolAlteration");
                }
            }
        }
        public bool IsSymbolAlteration
        {
            get { return _isSymbolAlteration; }
            set
            {
                if (_isSymbolAlteration != value)
                {
                    _isSymbolAlteration = value;
                    OnPropertyChanged("IsSymbolAlteration");
                }
            }
        }
        public Range<int> EventTime
        {
            get { return _eventTime; }
            set
            {
                if (_eventTime != value)
                {
                    _eventTime = value;
                    OnPropertyChanged("EventTime");
                }
            }
        }
        public CharacterStateType StateType
        {
            get { return _stateType; }
            set
            {
                if (_stateType != value)
                {
                    _stateType = value;
                    OnPropertyChanged("StateType");
                }
            }
        }
        public string PostEffectText
        {
            get { return _postEffectText; }
            set
            {
                if (_postEffectText != value)
                {
                    _postEffectText = value;
                    OnPropertyChanged("PostEffectText");
                }
            }
        }
        public Range<double> StrengthRange
        {
            get { return _strengthRange; }
            set
            {
                if (_strengthRange != value)
                {
                    _strengthRange = value;
                    OnPropertyChanged("StrengthRange");
                }
            }
        }
        public Range<double> IntelligenceRange
        {
            get { return _intelligenceRange; }
            set
            {
                if (_intelligenceRange != value)
                {
                    _intelligenceRange = value;
                    OnPropertyChanged("IntelligenceRange");
                }
            }
        }
        public Range<double> AgilityRange
        {
            get { return _agilityRange; }
            set
            {
                if (_agilityRange != value)
                {
                    _agilityRange = value;
                    OnPropertyChanged("AgilityRange");
                }
            }
        }
        public Range<double> AuraRadiusRange
        {
            get { return _auraRadiusRange; }
            set
            {
                if (_auraRadiusRange != value)
                {
                    _auraRadiusRange = value;
                    OnPropertyChanged("AuraRadiusRange");
                }
            }
        }
        public Range<double> FoodUsagePerTurnRange
        {
            get { return _foodUsagePerTurnRange; }
            set
            {
                if (_foodUsagePerTurnRange != value)
                {
                    _foodUsagePerTurnRange = value;
                    OnPropertyChanged("FoodUsagePerTurnRange");
                }
            }
        }
        public Range<double> HpPerStepRange
        {
            get { return _hpPerStepRange; }
            set
            {
                if (_hpPerStepRange != value)
                {
                    _hpPerStepRange = value;
                    OnPropertyChanged("HpPerStepRange");
                }
            }
        }
        public Range<double> MpPerStepRange
        {
            get { return _mpPerStepRange; }
            set
            {
                if (_mpPerStepRange != value)
                {
                    _mpPerStepRange = value;
                    OnPropertyChanged("MpPerStepRange");
                }
            }
        }
        public Range<double> AttackRange
        {
            get { return _attackRange; }
            set
            {
                if (_attackRange != value)
                {
                    _attackRange = value;
                    OnPropertyChanged("AttackRange");
                }
            }
        }
        public Range<double> DefenseRange
        {
            get { return _defenseRange; }
            set
            {
                if (_defenseRange != value)
                {
                    _defenseRange = value;
                    OnPropertyChanged("DefenseRange");
                }
            }
        }
        public Range<double> MagicBlockProbabilityRange
        {
            get { return _magicBlockProbabilityRange; }
            set
            {
                if (_magicBlockProbabilityRange != value)
                {
                    _magicBlockProbabilityRange = value;
                    OnPropertyChanged("MagicBlockProbabilityRange");
                }
            }
        }
        public Range<double> DodgeProbabilityRange
        {
            get { return _dodgeProbabilityRange; }
            set
            {
                if (_dodgeProbabilityRange != value)
                {
                    _dodgeProbabilityRange = value;
                    OnPropertyChanged("DodgeProbabilityRange");
                }
            }
        }
        public Range<double> ExperienceRange
        {
            get { return _experienceRange; }
            set
            {
                if (_experienceRange != value)
                {
                    _experienceRange = value;
                    OnPropertyChanged("ExperienceRange");
                }
            }
        }
        public Range<double> HungerRange
        {
            get { return _hungerRange; }
            set
            {
                if (_hungerRange != value)
                {
                    _hungerRange = value;
                    OnPropertyChanged("HungerRange");
                }
            }
        }
        public Range<double> HpRange
        {
            get { return _hpRange; }
            set
            {
                if (_hpRange != value)
                {
                    _hpRange = value;
                    OnPropertyChanged("HpRange");
                }
            }
        }
        public Range<double> MpRange
        {
            get { return _mpRange; }
            set
            {
                if (_mpRange != value)
                {
                    _mpRange = value;
                    OnPropertyChanged("MpRange");
                }
            }
        }
        public Range<double> CriticalHit
        {
            get { return _criticalHit; }
            set
            {
                if (_criticalHit != value)
                {
                    _criticalHit = value;
                    OnPropertyChanged("CriticalHit");
                }
            }
        }
        public bool IsSilence
        {
            get { return _isSilence; }
            set
            {
                if (_isSilence != value)
                {
                    _isSilence = value;
                    OnPropertyChanged("IsSilence");
                }
            }
        }

        public List<AttackAttributeTemplate> AttackAttributes { get; set; }
        public List<SpellTemplate> RemediedSpells { get; set; }

        public AlterationEffectTemplate()
        {
            this.SymbolAlteration = new SymbolDetailsTemplate();
            this.EventTime = new Range<int>(0, 20, 30, 1000);

            this.AgilityRange = new Range<double>(-100, 0, 0, 100);
            this.AttackRange = new Range<double>(-100, 0, 0, 100);
            this.AuraRadiusRange = new Range<double>(-25, 0, 0, 25);
            this.DefenseRange = new Range<double>(-100, 0, 0, 100);
            this.DodgeProbabilityRange = new Range<double>(-1, 0, 0, 1);
            this.ExperienceRange = new Range<double>(-100000, 0, 0, 100000);
            this.FoodUsagePerTurnRange = new Range<double>(-10, 0, 0, 10);
            this.HpPerStepRange = new Range<double>(-100, 0, 0, 100);
            this.HpRange = new Range<double>(-1000, 0, 0, 1000);
            this.HungerRange = new Range<double>(-100, 0, 0, 100);
            this.IntelligenceRange = new Range<double>(-100, 0, 0, 100);
            this.MagicBlockProbabilityRange = new Range<double>(-1, 0, 0, 1);
            this.MpPerStepRange = new Range<double>(-100, 0, 0, 100);
            this.MpRange = new Range<double>(-100, 0, 0, 100);
            this.StrengthRange = new Range<double>(-100, 0, 0, 100);

            this.CriticalHit = new Range<double>(-1, 0, 0, 1);

            this.AttackAttributes = new List<AttackAttributeTemplate>();
            this.RemediedSpells = new List<SpellTemplate>();
        }
    }
    [Serializable]
    public class SpellTemplate : DungeonObjectTemplate
    {
        public List<AnimationTemplate> Animations { get; set; }
        private AlterationCostTemplate _cost;
        private AlterationEffectTemplate _effect;
        private AlterationEffectTemplate _auraEffect;
        private AlterationType _type;
        private AlterationBlockType _blockType;
        private AlterationMagicEffectType _otherEffectType;
        private AlterationAttackAttributeType _attackAttributeType;
        private double _effectRange;
        private bool _stackable;
        private string _createMonsterEnemy;
        private string _displayName;

        public AlterationCostTemplate Cost
        {
            get { return _cost; }
            set
            {
                if (_cost != value)
                {
                    _cost = value;
                    OnPropertyChanged("Cost");
                }
            }
        }
        public AlterationEffectTemplate Effect
        {
            get { return _effect; }
            set
            {
                if (_effect != value)
                {
                    _effect = value;
                    OnPropertyChanged("Effect");
                }
            }
        }
        public AlterationEffectTemplate AuraEffect
        {
            get { return _auraEffect; }
            set
            {
                if (_auraEffect != value)
                {
                    _auraEffect = value;
                    OnPropertyChanged("AuraEffect");
                }
            }
        }
        public AlterationType Type
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
        public AlterationBlockType BlockType
        {
            get { return _blockType; }
            set
            {
                if (_blockType != value)
                {
                    _blockType = value;
                    OnPropertyChanged("BlockType");
                }
            }
        }
        public AlterationMagicEffectType OtherEffectType
        {
            get { return _otherEffectType; }
            set
            {
                if (_otherEffectType != value)
                {
                    _otherEffectType = value;
                    OnPropertyChanged("OtherEffectType");
                }
            }
        }
        public AlterationAttackAttributeType AttackAttributeType
        {
            get { return _attackAttributeType; }
            set
            {
                if (_attackAttributeType != value)
                {
                    _attackAttributeType = value;
                    OnPropertyChanged("AttackAttributeType");
                }
            }
        }
        public double EffectRange
        {
            get { return _effectRange; }
            set
            {
                if (_effectRange != value)
                {
                    _effectRange = value;
                    OnPropertyChanged("EffectRange");
                }
            }
        }
        public bool Stackable
        {
            get { return _stackable; }
            set
            {
                if (_stackable != value)
                {
                    _stackable = value;
                    OnPropertyChanged("Stackable");
                }
            }
        }
        public string CreateMonsterEnemy
        {
            get { return _createMonsterEnemy; }
            set
            {
                if (_createMonsterEnemy != value)
                {
                    _createMonsterEnemy = value;
                    OnPropertyChanged("CreateMonsterEnemy");
                }
            }
        }
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (_displayName != value)
                {
                    _displayName = value;
                    OnPropertyChanged("DisplayName");
                }
            }
        }

        public SpellTemplate()
        {
            this.Animations = new List<AnimationTemplate>();
            this.Cost = new AlterationCostTemplate();
            this.Effect = new AlterationEffectTemplate();
            this.AuraEffect = new AlterationEffectTemplate();
            
            //Causes circular reference. Have to lazy load
            this.CreateMonsterEnemy = "";
            this.DisplayName = "";
        }
    }
    [Serializable]
    public class SkillSetTemplate : DungeonObjectTemplate
    {
	    private int _levelLearned;

        public List<SpellTemplate> Spells { get; set; }
	    public int LevelLearned
	    {
		    get{ return _levelLearned; }
		    set
		    {
			    if(_levelLearned != value)
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
    #endregion

    #region Attack Attributes
    [Serializable]
    public class AttackAttributeTemplate : DungeonObjectTemplate
    {
        private Range<double> _attack;
        private Range<double> _resistance;
        private Range<int> _weakness;

        public Range<double> Attack
        {
            get { return _attack; }
            set
            {
                if (_attack != value)
                {
                    _attack = value;
                    OnPropertyChanged("Attack");
                }
            }
        }
        public Range<double> Resistance
        {
            get { return _resistance; }
            set
            {
                if (_resistance != value)
                {
                    _resistance = value;
                    OnPropertyChanged("Resistance");
                }
            }
        }
        public Range<int> Weakness
        {
            get { return _weakness; }
            set
            {
                if (_weakness != value)
                {
                    _weakness = value;
                    OnPropertyChanged("Weakness");
                }
            }
        }

        public AttackAttributeTemplate()
        {
            this.Attack = new Range<double>(0, 0, 0, 5000);
            this.Resistance = new Range<double>(0, 0, 0, 5000);
            this.Weakness = new Range<int>(0, 0, 0, 10);
        }
    }
    #endregion

    #region Brushes / Animations

    public enum BrushType
    {
        Solid,
        Linear,
        Radial
    }
    [Serializable]
    public class BrushTemplate : Template
    {
        public List<GradientStopTemplate> GradientStops { get; set; }

        private BrushType _type;
        private double _opacity;
        private string _solidColor;
        private double _gradientStartX;
        private double _gradientStartY;
        private double _gradientEndX;
        private double _gradientEndY;

        public BrushType Type
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
        public double Opacity
        {
            get { return _opacity; }
            set
            {
                if (_opacity != value)
                {
                    _opacity = value;
                    OnPropertyChanged("Opacity");
                }
            }
        }
        public string SolidColor
        {
            get { return _solidColor; }
            set
            {
                if (_solidColor != value)
                {
                    _solidColor = value;
                    OnPropertyChanged("SolidColor");
                }
            }
        }
        public double GradientStartX
        {
            get { return _gradientStartX; }
            set
            {
                if (_gradientStartX != value)
                {
                    _gradientStartX = value;
                    OnPropertyChanged("GradientStartX");
                }
            }
        }
        public double GradientStartY
        {
            get { return _gradientStartY; }
            set
            {
                if (_gradientStartY != value)
                {
                    _gradientStartY = value;
                    OnPropertyChanged("GradientStartY");
                }
            }
        }
        public double GradientEndX
        {
            get { return _gradientEndX; }
            set
            {
                if (_gradientEndX != value)
                {
                    _gradientEndX = value;
                    OnPropertyChanged("GradientEndX");
                }
            }
        }
        public double GradientEndY
        {
            get { return _gradientEndY; }
            set
            {
                if (_gradientEndY != value)
                {
                    _gradientEndY = value;
                    OnPropertyChanged("GradientEndY");
                }
            }
        }

        public BrushTemplate()
        {
            this.GradientStops = new List<GradientStopTemplate>();
            this.Opacity = 1;
            this.SolidColor = Colors.White.ToString();
        }
        public BrushTemplate(string name)
        {
            this.GradientStops = new List<GradientStopTemplate>();
            this.Name = name;
            this.Opacity = 1;
            this.SolidColor = Colors.White.ToString();
        }

        public Brush GenerateBrush()
        {
            switch (this.Type)
            {
                case BrushType.Solid:
                    {
                        SolidColorBrush b = new SolidColorBrush((Color)ColorConverter.ConvertFromString(this.SolidColor));
                        //b.Opacity = this.Opacity;
                        return b;
                    }
                case BrushType.Linear:
                    {
                        LinearGradientBrush b = new LinearGradientBrush();
                        //b.Opacity = this.Opacity;
                        b.StartPoint = new Point(this.GradientStartX, this.GradientStartY);
                        b.EndPoint = new Point(this.GradientEndX, this.GradientEndY);
                        foreach (GradientStopTemplate t in this.GradientStops)
                            b.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(t.GradientColor), t.GradientOffset));

                        return b;
                    }
                case BrushType.Radial:
                    {
                        RadialGradientBrush b = new RadialGradientBrush();
                        //b.Opacity = this.Opacity;
                        b.GradientOrigin = new Point(this.GradientStartX, this.GradientStartY);
                        double x = this.GradientEndX - this.GradientStartX;
                        double y = this.GradientEndY - this.GradientStartY;
                        b.RadiusX = Math.Abs(x);
                        b.RadiusY = Math.Abs(y);
                        foreach (GradientStopTemplate t in this.GradientStops)
                            b.GradientStops.Add(new GradientStop((Color)ColorConverter.ConvertFromString(t.GradientColor), t.GradientOffset));
                        return b;
                    }
            }

            return null;

        }
    }
    [Serializable]
    public class PenTemplate : BrushTemplate
    {
    }
    [Serializable]
    public class GradientStopTemplate : Template
    {
        private string _gradientColor;
        private double _gradientOffset;

        public string GradientColor
        {
            get { return _gradientColor; }
            set
            {
                if (_gradientColor != value)
                {
                    _gradientColor = value;
                    OnPropertyChanged("GradientColor");
                }
            }
        }
        public double GradientOffset
        {
            get { return _gradientOffset; }
            set
            {
                if (_gradientOffset != value)
                {
                    _gradientOffset = value;
                    OnPropertyChanged("GradientOffset");
                }
            }
        }
        public GradientStopTemplate() { }
        public GradientStopTemplate(double offset, Color c)
        {
            this.GradientColor = c.ToString();
            this.GradientOffset = offset;
        }
    }
    [Serializable]
    public class AnimationTemplate : Template
    {
        private int _repeatCount;
        private int _animationTime;
        private bool _autoReverse;
        private bool _constantVelocity;
        private double _accelerationRatio;
        private AnimationType _type;
        private BrushTemplate _fillTemplate;
        private BrushTemplate _strokeTemplate;
        private double _strokeThickness;
        private double _opacity1;
        private double _opacity2;
        private double _height1;
        private double _height2;
        private double _width1;
        private double _width2;
        private int _velocity;
        private int _childCount;
        private int _erradicity;
        private double _radiusFromFocus;
        private double _spiralRate;
        private double _roamRadius;

        public int RepeatCount
        {
            get { return _repeatCount; }
            set
            {
                if (_repeatCount != value)
                {
                    _repeatCount = value;
                    OnPropertyChanged("RepeatCount");
                }
            }
        }
        public int AnimationTime
        {
            get { return _animationTime; }
            set
            {
                if (_animationTime != value)
                {
                    _animationTime = value;
                    OnPropertyChanged("AnimationTime");
                }
            }
        }
        public bool AutoReverse
        {
            get { return _autoReverse; }
            set
            {
                if (_autoReverse != value)
                {
                    _autoReverse = value;
                    OnPropertyChanged("AutoReverse");
                }
            }
        }
        public bool ConstantVelocity
        {
            get { return _constantVelocity; }
            set
            {
                if (_constantVelocity != value)
                {
                    _constantVelocity = value;
                    OnPropertyChanged("ConstantVelocity");
                }
            }
        }
        public double AccelerationRatio
        {
            get { return _accelerationRatio; }
            set
            {
                if (_accelerationRatio != value)
                {
                    _accelerationRatio = value;
                    OnPropertyChanged("AccelerationRatio");
                }
            }
        }
        public AnimationType Type
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
        public BrushTemplate FillTemplate
        {
            get { return _fillTemplate; }
            set
            {
                if (_fillTemplate != value)
                {
                    _fillTemplate = value;
                    OnPropertyChanged("FillTemplate");
                }
            }
        }
        public BrushTemplate StrokeTemplate
        {
            get { return _strokeTemplate; }
            set
            {
                if (_strokeTemplate != value)
                {
                    _strokeTemplate = value;
                    OnPropertyChanged("StrokeTemplate");
                }
            }
        }
        public double StrokeThickness
        {
            get { return _strokeThickness; }
            set
            {
                if (_strokeThickness != value)
                {
                    _strokeThickness = value;
                    OnPropertyChanged("StrokeThickness");
                }
            }
        }
        public double Opacity1
        {
            get { return _opacity1; }
            set
            {
                if (_opacity1 != value)
                {
                    _opacity1 = value;
                    OnPropertyChanged("Opacity1");
                }
            }
        }
        public double Opacity2
        {
            get { return _opacity2; }
            set
            {
                if (_opacity2 != value)
                {
                    _opacity2 = value;
                    OnPropertyChanged("Opacity2");
                }
            }
        }
        public double Height1
        {
            get { return _height1; }
            set
            {
                if (_height1 != value)
                {
                    _height1 = value;
                    OnPropertyChanged("Height1");
                }
            }
        }
        public double Height2
        {
            get { return _height2; }
            set
            {
                if (_height2 != value)
                {
                    _height2 = value;
                    OnPropertyChanged("Height2");
                }
            }
        }
        public double Width1
        {
            get { return _width1; }
            set
            {
                if (_width1 != value)
                {
                    _width1 = value;
                    OnPropertyChanged("Width1");
                }
            }
        }
        public double Width2
        {
            get { return _width2; }
            set
            {
                if (_width2 != value)
                {
                    _width2 = value;
                    OnPropertyChanged("Width2");
                }
            }
        }
        public int Velocity
        {
            get { return _velocity; }
            set
            {
                if (_velocity != value)
                {
                    _velocity = value;
                    OnPropertyChanged("Velocity");
                }
            }
        }
        public int ChildCount
        {
            get { return _childCount; }
            set
            {
                if (_childCount != value)
                {
                    _childCount = value;
                    OnPropertyChanged("ChildCount");
                }
            }
        }
        public int Erradicity
        {
            get { return _erradicity; }
            set
            {
                if (_erradicity != value)
                {
                    _erradicity = value;
                    OnPropertyChanged("Erradicity");
                }
            }
        }
        public double RadiusFromFocus
        {
            get { return _radiusFromFocus; }
            set
            {
                if (_radiusFromFocus != value)
                {
                    _radiusFromFocus = value;
                    OnPropertyChanged("RadiusFromFocus");
                }
            }
        }
        public double SpiralRate
        {
            get { return _spiralRate; }
            set
            {
                if (_spiralRate != value)
                {
                    _spiralRate = value;
                    OnPropertyChanged("SpiralRate");
                }
            }
        }
        public double RoamRadius
        {
            get { return _roamRadius; }
            set
            {
                if (_roamRadius != value)
                {
                    _roamRadius = value;
                    OnPropertyChanged("RoamRadius");
                }
            }
        }

        //Constructors
        public AnimationTemplate()
        {
            this.StrokeTemplate = new BrushTemplate();
            this.FillTemplate = new BrushTemplate();
            this.AccelerationRatio = 1;
            this.AnimationTime = 1000;
            this.AutoReverse = false;
            this.ChildCount = 5;
            this.ConstantVelocity = false;
            this.Erradicity = 1;
            this.Height1 = 4;
            this.Height2 = 4;
            this.Opacity1 = 1;
            this.Opacity2 = 1;
            this.RadiusFromFocus = 20;
            this.RepeatCount = 1;
            this.RoamRadius = 20;
            this.SpiralRate = 10;
            this.StrokeThickness = 1;
            this.Type = AnimationType.ProjectileSelfToTarget;
            this.Velocity = 50;
            this.Width1 = 4;
            this.Width2 = 4;
        }
    }
    #endregion
}
