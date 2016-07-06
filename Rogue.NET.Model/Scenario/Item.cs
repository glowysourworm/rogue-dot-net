using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Windows;
using System.ComponentModel;
using Rogue.NET.Common;
using Rogue.NET.Scenario.Model;
using Rogue.NET.Common.Collections;
using System.Runtime.Serialization;
using System.Reflection;

namespace Rogue.NET.Model.Scenario
{
    [Serializable]
    public class Item : ScenarioObject
    {
        bool _isIdentified;
        bool _isMarkedForTrade;

        /// <summary>
        /// Weight (Haul) of item
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Identified individual item flag (reveals class / cursed / unique)
        /// </summary>
        public bool IsIdentified
        {
            get { return _isIdentified; }
            set
            {
                _isIdentified = value;
                OnPropertyChanged("IsIdentified");
            }
        }

        /// <summary>
        /// Value set in configuration for shop exchange 
        /// </summary>
        public int ShopValue { get; set; }

        /// <summary>
        /// Flags items that are marked for trade at the shop
        /// </summary>
        public bool IsMarkedForTrade
        {
            get { return _isMarkedForTrade; }
            set
            {
                _isMarkedForTrade = value;
                OnPropertyChanged("IsMarkedForTrade");
            }
        }

        public Item()
        {
        }
        public Item(ImageResources icon, string name) : base(name, icon)
        {
        }
        public Item(ImageResources icon, string name, double scale)
            : base(name, icon, scale)
        {
        }
        public Item(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var properties = typeof(Item).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(Item).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
                info.AddValue(property.Name, property.GetValue(this));
        }
    }
    [Serializable]
    public class Equipment : Item
    {
        bool _isEquiped;
        bool _isCursed;

        public int Class { get; set; }
        public double Quality { get; set; }
        public EquipmentType Type { get; set; }

        public bool IsEquiped
        {
            get { return _isEquiped; }
            set
            {
                _isEquiped = value;
                OnPropertyChanged("IsEquiped");
            }
        }
        public bool IsCursed
        {
            get { return _isCursed; }
            set
            {
                _isCursed = value;
                OnPropertyChanged("IsCursed");
            }
        }

        public bool HasAttackSpell { get; set; }
        public bool HasEquipSpell { get; set; }
        public bool HasCurseSpell { get; set; }
        public Spell AttackSpell { get; set; }
        public Spell EquipSpell { get; set; }
        public Spell CurseSpell { get; set; }
        public string AmmoName { get; set; }

        public SerializableObservableCollection<AttackAttribute> AttackAttributes { get; set; }

        public bool IsGadget
        {
            get
            {
                return this.Type == EquipmentType.CompassGadget ||
                       this.Type == EquipmentType.EnemyScopeGadet ||
                       this.Type == EquipmentType.EquipmentGadget;
            }
        }

        public Equipment() : base()
        {
            Weight = 3;
            IsEquiped = false;
            Type = EquipmentType.Armor;
            this.AttackSpell = new Spell();
            this.EquipSpell = new Spell();
            this.CurseSpell = new Spell();
            this.AmmoName = "";
            this.AttackAttributes = new SerializableObservableCollection<AttackAttribute>();
        }
        public Equipment(EquipmentType type, string name, ImageResources icon) : base(icon, name)
        {
            RogueName = name;
            Type = type;
            IsEquiped = false;
            this.AttackSpell = new Spell();
            this.EquipSpell = new Spell();
            this.CurseSpell = new Spell();
            this.AmmoName = "";
            this.AttackAttributes = new SerializableObservableCollection<AttackAttribute>();
        }
        public Equipment(EquipmentType type, string name, ImageResources icon, double scale)
            : base(icon, name, scale)
        {
            RogueName = name;
            Type = type;
            IsEquiped = false;
            this.AttackSpell = new Spell();
            this.EquipSpell = new Spell();
            this.CurseSpell = new Spell();
            this.AmmoName = "";
            this.AttackAttributes = new SerializableObservableCollection<AttackAttribute>();
        }
        public Equipment(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var properties = typeof(Equipment).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(Equipment).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
                info.AddValue(property.Name, property.GetValue(this));
        }
    }
    [Serializable]
    public class Consumable : Item
    {
        public ConsumableType Type { get; set; }
        public ConsumableSubType SubType { get; set; }

        public bool HasSpell { get; set; }
        public bool HasProjectileSpell { get; set; }
        public bool HasLearnedSkillSet { get; set; }

        public int Uses { get; set; }

        public Spell Spell { get; set; }
        public Spell AmmoSpell { get; set; }
        public Spell ProjectileSpell { get; set; }
        public SkillSet LearnedSkill { get; set; }

        public Consumable() : base()
        {
            this.Type = ConsumableType.OneUse;
            this.SubType = ConsumableSubType.Food;
            this.HasSpell = false;
            this.Spell = new Spell();
            this.AmmoSpell = new Spell();
            this.LearnedSkill = new SkillSet("NotNamed", ImageResources.Skill1);
            this.Weight = 0.5;
        }
        public Consumable(ConsumableType type, ConsumableSubType subType, string name, ImageResources icon) 
            : base(icon, name)
        {
            this.Type = type;
            this.SubType = subType;
            this.RogueName = name;
            this.HasSpell = false;
            this.LearnedSkill = new SkillSet("NotNamed", ImageResources.Skill1);
            this.Spell = new Spell();
            this.AmmoSpell = new Spell();
        }
        public Consumable(ConsumableType type, ConsumableSubType subType, string name, ImageResources icon, double scale)
            : base(icon, name, scale)
        {
            this.Type = type;
            this.SubType = subType;
            this.RogueName = name;
            this.HasSpell = false;
            this.LearnedSkill = new SkillSet("NotNamed", ImageResources.Skill1);
            this.Spell = new Spell();
            this.AmmoSpell = new Spell();
        }
        public Consumable(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var properties = typeof(Consumable).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(Consumable).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
                info.AddValue(property.Name, property.GetValue(this));
        }
    }
}
