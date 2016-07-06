using Rogue.NET.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Rogue.NET.Model.Scenario
{
    /// <summary>
    /// Contains all altering data from the Spell class - with resolved
    /// alterations (cost, effect, aura effect) instead of random variable ranges
    /// </summary>
    [Serializable]
    public class Alteration : NamedObject
    {
        AlterationCost _cost;
        AlterationEffect _effect;
        AlterationEffect _auraEffect;
        AlterationType _type;
        AlterationBlockType _blockType;
        AlterationMagicEffectType _magicEffectType;
        AlterationAttackAttributeType _attributeType;
        double _effectRange;
        bool _stackable;
        string _createMonsterEnemyName;

        /// <summary>
        /// Copy of spell name
        /// </summary>
        public AlterationCost Cost
        {
            get { return _cost; }
            set
            {
                _cost = value;
                OnPropertyChanged("Cost");
            }
        }
        new public AlterationEffect Effect
        {
            get { return _effect; }
            set
            {
                _effect = value;
                OnPropertyChanged("Effect");
            }
        }
        public AlterationEffect AuraEffect
        {
            get { return _auraEffect; }
            set
            {
                _auraEffect = value;
                OnPropertyChanged("AuraEffect");
            }
        }
        public AlterationType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }
        public AlterationBlockType BlockType
        {
            get { return _blockType; }
            set
            {
                _blockType = value;
                OnPropertyChanged("BlockType");
            }
        }
        public AlterationMagicEffectType OtherEffectType
        {
            get { return _magicEffectType; }
            set
            {
                _magicEffectType = value;
                OnPropertyChanged("OtherEffectType");
            }
        }
        public AlterationAttackAttributeType AttackAttributeType
        {
            get { return _attributeType; }
            set
            {
                _attributeType = value;
                OnPropertyChanged("AttackAttributeType");
            }
        }
        public double EffectRange
        {
            get { return _effectRange; }
            set
            {
                _effectRange = value;
                OnPropertyChanged("EffectRange");
            }
        }
        public bool Stackable
        {
            get { return _stackable; }
            set
            {
                _stackable = value;
                OnPropertyChanged("Stackable");
            }
        }

        /// <summary>
        /// Enemy created as an effect of the alteration
        /// </summary>
        public string CreateMonsterEnemy
        {
            get { return _createMonsterEnemyName; }
            set
            {
                _createMonsterEnemyName = value;
                OnPropertyChanged("CreateMonsterEnemy");
            }
        }

        public Alteration() { }
        public Alteration(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this.Cost = (AlterationCost)info.GetValue("Cost", typeof(AlterationCost));
            this.Effect = (AlterationEffect)info.GetValue("Effect", typeof(AlterationEffect));
            this.AuraEffect = (AlterationEffect)info.GetValue("AuraEffect", typeof(AlterationEffect));
            this.Type = (AlterationType)info.GetValue("Type", typeof(AlterationType));
            this.BlockType = (AlterationBlockType)info.GetValue("BlockType", typeof(AlterationBlockType));
            this.OtherEffectType = (AlterationMagicEffectType)info.GetValue("OtherEffectType", typeof(AlterationMagicEffectType));
            this.AttackAttributeType = (AlterationAttackAttributeType)info.GetValue("AttackAttributeType", typeof(AlterationAttackAttributeType));
            this.EffectRange = (double)info.GetValue("EffectRange", typeof(double));
            this.Stackable = (bool)info.GetValue("Stackable", typeof(bool));
            this.CreateMonsterEnemy = (string)info.GetValue("CreateMonsterEnemy", typeof(string));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Cost", this.Cost);
            info.AddValue("Effect", this.Effect);
            info.AddValue("AuraEffect", this.AuraEffect);
            info.AddValue("Type", this.Type);
            info.AddValue("BlockType", this.BlockType);
            info.AddValue("OtherEffectType", this.OtherEffectType);
            info.AddValue("AttackAttributeType", this.AttackAttributeType);
            info.AddValue("EffectRange", this.EffectRange);
            info.AddValue("Stackable", this.Stackable);
            info.AddValue("CreateMonsterEnemy", this.CreateMonsterEnemy);
        }
    }

    #region Alteration Cost
    [Serializable]
    public class AlterationCost : NamedObject
    {
        AlterationCostType _type;
        string _spellId;
        string _spellName;
        double _strength;
        double _intelligence;
        double _agility;
        double _foodUsage;
        double _auraRadius;
        double _experience;
        double _hunger;
        double _hp;
        double _mp;

        public AlterationCostType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }

        /// <summary>
        /// Id unique to spell instance
        /// </summary>
        public string SpellId
        {
            get { return _spellId; }
            set
            {
                _spellId = value;
                OnPropertyChanged("SpellId");
            }
        }
        public string SpellName
        {
            get { return _spellName; }
            set
            {
                _spellName = value;
                OnPropertyChanged("SpellName");
            }
        }

        public virtual double Strength
        {
            get { return _strength; }
            set
            {
                _strength = value;
                OnPropertyChanged("Strength");
            }
        }
        public virtual double Intelligence
        {
            get { return _intelligence; }
            set
            {
                _intelligence = value;
                OnPropertyChanged("Intelligence");
            }
        }
        public virtual double Agility
        {
            get { return _agility; }
            set
            {
                _agility = value;
                OnPropertyChanged("Agility");
            }
        }

        public virtual double FoodUsagePerTurn
        {
            get { return _foodUsage; }
            set
            {
                _foodUsage = value;
                OnPropertyChanged("FoodUsagePerTurn");
            }
        }
        public virtual double AuraRadius
        {
            get { return _auraRadius; }
            set
            {
                _auraRadius = value;
                OnPropertyChanged("AuraRadius");
            }
        }
        public virtual double Experience
        {
            get { return _experience; }
            set
            {
                _experience = value;
                OnPropertyChanged("Experience");
            }
        }

        public virtual double Hunger
        {
            get { return _hunger; }
            set
            {
                _hunger = value;
                OnPropertyChanged("Hunger");
            }
        }
        public virtual double Hp
        {
            get { return _hp; }
            set
            {
                _hp = value;
                OnPropertyChanged("Hp");
            }
        }
        public virtual double Mp
        {
            get { return _mp; }
            set
            {
                _mp = value;
                OnPropertyChanged("Mp");
            }
        }

        public AlterationCost() { }

        public AlterationCost(SerializationInfo info, StreamingContext ctxt) : base(info, ctxt)
        {
            info.AddValue("Type", this.Type);
            info.AddValue("SpellId", this.SpellId);
            info.AddValue("SpellName", this.SpellName);
            info.AddValue("Strength", this.Strength);
            info.AddValue("Intelligence", this.Intelligence);
            info.AddValue("Agility", this.Agility);
            info.AddValue("FoodUsagePerTurn", this.FoodUsagePerTurn);
            info.AddValue("AuraRadius", this.AuraRadius);
            info.AddValue("Experience", this.Experience);
            info.AddValue("Hunger", this.Hunger);
            info.AddValue("Hp", this.Hp);
            info.AddValue("Mp", this.Mp);
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            this.Type = (AlterationCostType)info.GetValue("Type", typeof(AlterationCostType));
            this.SpellId = info.GetString("SpellId");
            this.SpellName = info.GetString("SpellName");
            this.Strength = info.GetDouble("Strength");
            this.Intelligence = info.GetDouble("Intelligence");
            this.Agility = info.GetDouble("Agility");
            this.FoodUsagePerTurn = info.GetDouble("FoodUsagePerTurn");
            this.AuraRadius = info.GetDouble("AuraRadius");
            this.Experience = info.GetDouble("Experience");
            this.Hunger = info.GetDouble("Hunger");
            this.Hp = info.GetDouble("Hp");
            this.Mp = info.GetDouble("Mp");
        }
    }
    #endregion

    #region Alteration Effect
    [Serializable]
    public class AlterationEffect : NamedObject
    {
        public SymbolDetailsTemplate SymbolAlteration { get; set; }
        public bool IsSymbolAlteration { get; set; }

        /// <summary>
        /// Id unique to spell instance
        /// </summary>
        public string SpellId { get; set; }
        public string SpellName { get; set; }
        public string PostEffectString { get; set; }
        public string DisplayName { get; set; }

        //Passive Aura's only: Copied to aura effect from spell
        public double EffectRange { get; set; }

        public CharacterStateType State { get; set; }
        public virtual int EventTime { get; set; }

        public virtual double Strength { get; set; }
        public virtual double Intelligence { get; set; }
        public virtual double Agility { get; set; }
        public virtual double AuraRadius { get; set; }
        public virtual double FoodUsagePerTurn { get; set; }
        public virtual double HpPerStep { get; set; }
        public virtual double MpPerStep { get; set; }

        public virtual double Attack { get; set; }
        public virtual double Defense { get; set; }
        public virtual double MagicBlockProbability { get; set; }
        public virtual double DodgeProbability { get; set; }
        public virtual int ClassEnchant { get; set; }

        public virtual double Experience { get; set; }
        public virtual double Hunger { get; set; }
        public virtual double Hp { get; set; }
        public virtual double Mp { get; set; }
        public virtual double CriticalHit { get; set; }

        /// <summary>
        /// Designates a silence alteration - causes the character to be unable to cast spells for a time
        /// </summary>
        public virtual bool IsSilence { get; set; }

        public virtual List<AttackAttribute> AttackAttributes { get; set; }

        /// <summary>
        /// List of spells that are removed via this alteration effect - permanent alterations only
        /// </summary>
        public virtual List<string> RemediedSpellNames { get; set; }

        public bool ProjectCharacterCanSupport(Character c)
        {
            return (c is Player) ? ProjectPlayerCanSupport(c as Player) : ProjectEnemyCanSupport(c as Enemy);
        }
        private bool ProjectPlayerCanSupport(Player p)
        {
            if (p.StrengthBase + this.Strength < 0)
                return false;

            if (p.IntelligenceBase + this.Intelligence < 0)
                return false;

            if (p.AgilityBase + this.Agility < 0)
                return false;

            if (p.AuraRadiusBase + this.AuraRadius < 0)
                return false;

            if (p.AttackBase + this.Attack < 0)
                return false;

            if (p.DefenseBase + this.Defense < 0)
                return false;

            if (p.MagicBlockBase + this.MagicBlockProbability < 0)
                return false;

            //if (p.Dodge + this.DodgeProbability < 0)
            //    return false;

            if (p.Experience + this.Experience < 0)
                return false;

            if (p.Hunger + this.Hunger > 100)
                return false;

            if (p.Hp + this.Hp <= 0)
                return false;

            if (p.Mp + this.Mp <= 0)
                return false;

            return true;
        }
        private bool ProjectEnemyCanSupport(Enemy e)
        {
            if (e.StrengthBase + this.Strength < 0)
                return false;

            if (e.IntelligenceBase + this.Intelligence < 0)
                return false;

            if (e.AgilityBase + this.Agility < 0)
                return false;

            if (e.AuraRadiusBase + this.AuraRadius < 0)
                return false;

            if (e.Hp + this.Hp <= 0)
                return false;

            if (e.Mp + this.Mp <= 0)
                return false;

            return true;
        }

        public AlterationEffect() { this.AttackAttributes = new List<AttackAttribute>(); }
        public AlterationEffect(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            var properties = typeof(AlterationEffect).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(AlterationEffect).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var property in properties)
                info.AddValue(property.Name, property.GetValue(this));
        }
    }
    #endregion
}
