using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Equipment;
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
        private CharacterBaseAttribute _combatType;
        private EquipmentAttackAlterationTemplate _equipmentAttackAlteration;
        private EquipmentEquipAlterationTemplate _equipmentEquipAlteration;
        private EquipmentCurseAlterationTemplate _equipmentCurseAlteration;
        private ConsumableTemplate _ammoTemplate;
        private double _weight;
        private int _levelRequired;
        private bool _hasAttackAlteration;
        private bool _hasEquipAlteration;
        private bool _hasCurseAlteration;
        private bool _hasCharacterClassRequirement;

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
        public CharacterBaseAttribute CombatType
        {
            get { return _combatType; }
            set
            {
                if (_combatType != value)
                {
                    _combatType = value;
                    OnPropertyChanged("CombatType");
                }
            }
        }
        public EquipmentAttackAlterationTemplate EquipmentAttackAlteration
        {
            get { return _equipmentAttackAlteration; }
            set
            {
                if (_equipmentAttackAlteration != value)
                {
                    _equipmentAttackAlteration = value;
                    OnPropertyChanged("EquipmentAttackAlteration");
                }
            }
        }
        public EquipmentEquipAlterationTemplate EquipmentEquipAlteration
        {
            get { return _equipmentEquipAlteration; }
            set
            {
                if (_equipmentEquipAlteration != value)
                {
                    _equipmentEquipAlteration = value;
                    OnPropertyChanged("EquipmentEquipAlteration");
                }
            }
        }
        public EquipmentCurseAlterationTemplate EquipmentCurseAlteration
        {
            get { return _equipmentCurseAlteration; }
            set
            {
                if (_equipmentCurseAlteration != value)
                {
                    _equipmentCurseAlteration = value;
                    OnPropertyChanged("EquipmentCurseAlteration");
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
        public bool HasAttackAlteration
        {
            get { return _hasAttackAlteration; }
            set
            {
                if (_hasAttackAlteration != value)
                {
                    _hasAttackAlteration = value;
                    OnPropertyChanged("HasAttackAlteration");
                }
            }
        }
        public bool HasEquipAlteration
        {
            get { return _hasEquipAlteration; }
            set
            {
                if (_hasEquipAlteration != value)
                {
                    _hasEquipAlteration = value;
                    OnPropertyChanged("HasEquipAlteration");
                }
            }
        }
        public bool HasCurseAlteration
        {
            get { return _hasCurseAlteration; }
            set
            {
                if (_hasCurseAlteration != value)
                {
                    _hasCurseAlteration = value;
                    OnPropertyChanged("HasCurseAlteration");
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

        public EquipmentTemplate()
        {
            this.Class = new Range<int>(0, 10);
            this.Type = EquipmentType.Ring;
            this.CombatType = CharacterBaseAttribute.Strength;
            this.Quality = new Range<double>(0, 0, 100, 100);
            this.EquipmentAttackAlteration = new EquipmentAttackAlterationTemplate();
            this.EquipmentEquipAlteration = new EquipmentEquipAlterationTemplate();
            this.EquipmentCurseAlteration = new EquipmentCurseAlterationTemplate();
            this.AmmoTemplate = new ConsumableTemplate();
            this.AttackAttributes = new List<AttackAttributeTemplate>();
        }
    }
}
