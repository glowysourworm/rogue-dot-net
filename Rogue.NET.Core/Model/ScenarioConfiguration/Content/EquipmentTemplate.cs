using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Content
{
    [Serializable]
    public class EquipmentTemplate : DungeonObjectTemplate
    {
        public List<AttackAttributeTemplate> AttackAttributes { get; set; }

        private Range<int> _class;
        private Range<double> _quality;
        private EquipmentType _type;
        private SpellTemplate _equipSpell;
        private SpellTemplate _curseSpell;
        private ConsumableTemplate _ammoTemplate;
        private double _weight;
        private int _levelRequired;
        private bool _hasEquipSpell;
        private bool _hasCurseSpell;
        private bool _hasReligionRequirement;
        private ReligionTemplate _religion;

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


        public EquipmentTemplate()
        {
            this.Class = new Range<int>(0, 10);
            this.Type = EquipmentType.Ring;
            this.Quality = new Range<double>(0, 0, 100, 100);
            this.EquipSpell = new SpellTemplate();
            this.CurseSpell = new SpellTemplate();
            this.AmmoTemplate = new ConsumableTemplate();
            this.AttackAttributes = new List<AttackAttributeTemplate>();
            this.HasReligionRequirement = false;
            this.Religion = new ReligionTemplate();
        }
        public EquipmentTemplate(DungeonObjectTemplate tmp)
            : base(tmp)
        {
            this.Class = new Range<int>(0, 10);
            this.Type = EquipmentType.Ring;
            this.Quality = new Range<double>(0, 0, 100, 100);
            this.EquipSpell = new SpellTemplate();
            this.CurseSpell = new SpellTemplate();
            this.AmmoTemplate = new ConsumableTemplate();
            this.AttackAttributes = new List<AttackAttributeTemplate>();
            this.HasReligionRequirement = false;
            this.Religion = new ReligionTemplate();
        }
    }
}
