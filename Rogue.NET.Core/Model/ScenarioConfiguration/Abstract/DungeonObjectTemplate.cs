using ProtoBuf;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Abstract
{
    [Serializable]
    [ProtoContract(AsReferenceDefault = true, SkipConstructor = true)]
    [ProtoInclude(10, typeof(CharacterTemplate))]
    [ProtoInclude(11, typeof(EquipmentTemplate))]
    [ProtoInclude(12, typeof(DoodadTemplate))]
    [ProtoInclude(13, typeof(ConsumableTemplate))]
    [ProtoInclude(14, typeof(CombatAttributeTemplate))]
    [ProtoInclude(15, typeof(AttackAttributeTemplate))]
    [ProtoInclude(16, typeof(SpellTemplate))]
    [ProtoInclude(17, typeof(SkillSetTemplate))]
    [ProtoInclude(18, typeof(AlteredCharacterStateTemplate))]
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

        [ProtoMember(1)]
        public SymbolDetailsTemplate SymbolDetails
        {
            get { return _symbolDetails; }
            set
            {
                if (_symbolDetails != value)
                {
                    _symbolDetails = value;
                    OnPropertyChanged("SymbolDetails");
                }
            }
        }
        [ProtoMember(2)]
        public Range<int> Level
        {
            get { return _level; }
            set
            {
                if (_level != value)
                {
                    _level = value;
                    OnPropertyChanged("Level");
                }
            }
        }
        [ProtoMember(3)]
        public double GenerationRate
        {
            get { return _generationRate; }
            set
            {
                if (_generationRate != value)
                {
                    _generationRate = value;
                    OnPropertyChanged("GenerationRate");
                }
            }
        }
        [ProtoMember(4)]
        public string ShortDescription
        {
            get { return _shortDescription; }
            set
            {
                if (_shortDescription != value)
                {
                    _shortDescription = value;
                    OnPropertyChanged("ShortDescription");
                }
            }
        }
        [ProtoMember(5)]
        public string LongDescription
        {
            get { return _longDescription; }
            set
            {
                if (_longDescription != value)
                {
                    _longDescription = value;
                    OnPropertyChanged("LongDescription");
                }
            }
        }
        [ProtoMember(6)]
        public bool IsCursed
        {
            get { return _isCursed; }
            set
            {
                if (_isCursed != value)
                {
                    _isCursed = value;
                    OnPropertyChanged("IsCursed");
                }
            }
        }
        [ProtoMember(7)]
        public bool IsUnique
        {
            get { return _isUnique; }
            set
            {
                if (_isUnique != value)
                {
                    _isUnique = value;
                    OnPropertyChanged("IsUnique");
                }
            }
        }
        [ProtoMember(8)]
        public bool IsObjectiveItem
        {
            get { return _isObjectiveItem; }
            set
            {
                if (_isObjectiveItem != value)
                {
                    _isObjectiveItem = value;
                    OnPropertyChanged("IsObjectiveItem");
                }
            }
        }
        [ProtoMember(9)]
        public bool HasBeenGenerated
        {
            get { return _hasBeenGenerated; }
            set
            {
                if (_hasBeenGenerated != value)
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
}
