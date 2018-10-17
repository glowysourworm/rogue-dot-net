using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Schema;
using System.Xml;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Collections.Specialized;
using Rogue.NET.Common;
using Rogue.NET.Model.Logic;
using Rogue.NET.Common.Collections;
using System.Runtime.Serialization;
using System.Reflection;

namespace Rogue.NET.Model.Scenario
{
    [Serializable]
    public abstract class Character : ScenarioObject
    {
        protected const double HP_REGEN_BASE_MULTIPLIER = 0.01;
        protected const double MP_REGEN_BASE_MULTIPLIER = 0.01;
        protected const double HAUL_FOOD_USAGE_DIVISOR = 1000;
        protected const double HAUL_STRENGTH_MULTIPLIER = 5;
        protected const double MAGIC_DEF_BASE = 0.25;

        /// <summary>
        /// Gets a flag to say whether or not the character can cast spells or invoke skills
        /// </summary>
        protected bool IsMutedBase = false;

        protected SymbolDetails SavedSymbolInfo { get; private set; }

        #region Collections
        public SerializableObservableCollection<Equipment> EquipmentInventory { get; set; }
        public SerializableObservableCollection<Consumable> ConsumableInventory { get; set; }

        //Complete list of active character alterations - costs and effects
        /// <summary>
        /// [NOT INTENDED ACCESS - PUBLIC FOR SERIALIZATION]
        /// </summary>
        public SerializableObservableCollection<AlterationEffect> ActiveTemporaryEffects { get; set; }
        /// <summary>
        /// [NOT INTENDED ACCESS - PUBLIC FOR SERIALIZATION]
        /// </summary>
        public SerializableObservableCollection<AlterationEffect> ActivePassiveEffects { get; set; }

        /// <summary>
        /// [NOT INTENDED ACCESS - PUBLIC FOR SERIALIZATION] List of all active aura effects that act on other characters
        /// </summary>
        public SerializableObservableCollection<AlterationEffect> TargetAuraEffects { get; set; }

        /// <summary>
        /// [NOT INTENDED ACCESS - PUBLIC FOR SERIALIZATION] List of all active aura effects that act on character
        /// </summary>
        public SerializableObservableCollection<AlterationEffect> ActiveAuraEffects { get; set; }

        /// <summary>
        /// [NOT INTENDED ACCESS - PUBLIC FOR SERIALIZATION] List of all effects that are acting on character from other characters
        /// </summary>
        public SerializableObservableCollection<AlterationEffect> MalignAuraEffects { get; set; }

        /// <summary>
        /// [NOT INTENDED ACCESS - PUBLIC FOR SERIALIZATION] List that holds set of passive attack attribute alterations - NOT USED IN ALL CALCULATIONS
        /// </summary>
        public SerializableObservableCollection<AlterationEffect> AttackAttributePassiveEffects { get; set; }

        /// <summary>
        /// [NOT INTENDED ACCESS - PUBLIC FOR SERIALIZATION] List of temporary friendly effects using attack attributes only - NOT USED IN ALL CALCULATIONS
        /// </summary>
        public SerializableObservableCollection<AlterationEffect> AttackAttributeTemporaryFriendlyEffects { get; set; }

        /// <summary>
        /// [NOT INTENDED ACCESS - PUBLIC FOR SERIALIZATION] List of temporary malign effects using attack attributes only - NOT USED IN ALL CALCULATIONS.
        /// </summary>
        public SerializableObservableCollection<AlterationEffect> AttackAttributeTemporaryMalignEffects { get; set; }

        protected IEnumerable<AlterationEffect> Alterations
        {
            get
            {
                //// happens during initialization
                if (this.ActiveAuraEffects == null)
                    return new AlterationEffect[] { };

                return this.ActiveTemporaryEffects
                   .Union(this.ActivePassiveEffects)
                   .Union(this.ActiveAuraEffects)
                   .Union(this.MalignAuraEffects);
            }
        }

        public virtual SerializableObservableCollection<AttackAttribute> MeleeAttackAttributes
        {
            get
            {
                return null;
            }
        }
        #endregion

        readonly static string[] _boundProperties = new string[]{
            "HpRegen",
            "MpRegen",
            "Strength",
            "Agility",
            "Intelligence",
            "AuraRadius",
            "MagicBlock",
            "Haul",
            "Dodge"
        };

        double _hpMax;
        double _mpMax;
        double _strengthBase;
        double _intelligenceBase;
        double _agilityBase;
        double _auraRadiusBase;
        double _hp;
        double _mp;

        #region public bound Properties
        public double HpMax
        {
            get { return _hpMax; }
            set
            {
                _hpMax = value;
                OnPropertyChanged("HpMax");
            }
        }
        public double MpMax
        {
            get { return _mpMax; }
            set
            {
                _mpMax = value;
                OnPropertyChanged("MpMax");
            }
        }
        public double StrengthBase
        {
            get { return _strengthBase; }
            set
            {
                _strengthBase = value;
                OnPropertyChanged("Strength");
                OnPropertyChanged("StrengthBase");
                OnPropertyChanged("HpRegen");
                OnPropertyChanged("HpRegenBase");
            }
        }
        public double IntelligenceBase
        {
            get { return _intelligenceBase; }
            set
            {
                _intelligenceBase = value;
                OnPropertyChanged("Intelligence");
                OnPropertyChanged("IntelligenceBase");
                OnPropertyChanged("MpRegen");
                OnPropertyChanged("MpRegenBase");
                OnPropertyChanged("MagicBlock");
                OnPropertyChanged("MagicBlockBase");
            }
        }
        public double AgilityBase
        {
            get { return _agilityBase; }
            set
            {
                _agilityBase = value;
                OnPropertyChanged("Agility");
                OnPropertyChanged("AgilityBase");
                OnPropertyChanged("Dodge");
                OnPropertyChanged("DodgeBase");
            }
        }
        public double HpRegenBase { get { return HP_REGEN_BASE_MULTIPLIER * this.GetStrength(); } }
        public double MpRegenBase { get { return MP_REGEN_BASE_MULTIPLIER * this.GetIntelligence(); } }
        public double AuraRadiusBase
        {
            get { return _auraRadiusBase; }
            set
            {
                _auraRadiusBase = value;
                OnPropertyChanged("AuraRadiusBase");
            }
        }
        public double Hp
        {
            get { return _hp; }
            set
            {
                _hp = value;
                OnPropertyChanged("Hp");
            }
        }
        public double Mp
        {
            get { return _mp; }
            set
            {
                _mp = value;
                OnPropertyChanged("Mp");
            }
        }
        public double HpRegen
        {
            get { return GetHpRegen(true); }
        }
        public double MpRegen
        {
            get { return GetMpRegen(); }
        }
        public double Strength
        {
            get { return GetStrength(); }
        }
        public double Agility
        {
            get { return GetAgility(); }
        }
        public double Intelligence
        {
            get { return GetIntelligence(); }
        }
        public double AuraRadius
        {
            get { return GetAuraRadius(); }
        }
        public double MagicBlock
        {
            get { return GetMagicBlock(); }
        }
        public double Dodge
        {
            get { return GetDodge(); }
        }
        public double Haul
        {
            get { return GetHaul(); }
        }
        #endregion

        public virtual double MagicBlockBase
        {
            get { return this.IntelligenceBase / 100; }
        }
        public virtual double DodgeBase
        {
            get { return this.AgilityBase / 100; }
        }
        public virtual double HaulMax
        {
            get { return this.StrengthBase * HAUL_STRENGTH_MULTIPLIER; }
        }


        #region Methods
        protected virtual double GetMpRegen()
        {
            double d = this.MpRegenBase;
            foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
                d += alt.MpPerStep;

            return d;
        }
        /// <summary>
        /// Returns effective HP regeneration - set to false to calculate for malign effects only
        /// </summary>
        /// <param name="regenerate"></param>
        /// <returns></returns>
        protected virtual double GetHpRegen(bool regenerate)
        {
            double d = regenerate ? this.HpRegenBase : 0;

            //Normal alteration effects
            if (regenerate)
            {
                foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
                    d += alt.HpPerStep;
            }

            //Malign attack attribute contributions
            foreach (AlterationEffect malignEffect in this.AttackAttributeTemporaryMalignEffects)
            {
                foreach (AttackAttribute malignAttribute in malignEffect.AttackAttributes)
                {
                    double resistance = 0;
                    double weakness = 0;
                    double attack = malignAttribute.Attack;

                    //Friendly attack attribute contributions
                    foreach (AlterationEffect friendlyEffect in this.AttackAttributeTemporaryFriendlyEffects)
                    {
                        resistance += friendlyEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Resistance;
                        weakness += friendlyEffect.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Weakness;

                    }

                    //Equipment contributions
                    foreach (Equipment e in this.EquipmentInventory.Where(z => z.IsEquiped))
                    {
                        resistance += e.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Resistance;
                        weakness += e.AttackAttributes.First(z => z.RogueName == malignAttribute.RogueName).Weakness;
                    }

                    d -= Calculator.CalculateAttackAttributeMelee(attack, resistance, weakness);
                }
            }

            return d;
        }
        protected virtual double GetStrength()
        {
            double d = this.StrengthBase;
            foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
                d += alt.Strength;

            return Math.Max(0.1, d);
        }
        protected virtual double GetAgility()
        {
            double d = this.AgilityBase;
            foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
                d += alt.Agility;

            return Math.Max(0.1, d);
        }
        protected virtual double GetIntelligence()
        {
            double d = this.IntelligenceBase;
            foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
                d += alt.Intelligence;

            return Math.Max(0.1, d);
        }
        protected virtual double GetAuraRadius()
        {
            double d = this.AuraRadiusBase;
            foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
                d += alt.AuraRadius;

            return Math.Max(0, d);
        }
        protected virtual double GetMagicBlock()
        {
            double d = this.GetIntelligence() / 100;
            foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
                d += alt.MagicBlockProbability;
            return Math.Max(Math.Min(1, d), 0);
        }
        protected virtual double GetDodge()
        {
            double d = this.GetAgility() / 100;
            foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
                d += alt.DodgeProbability;
            return Math.Max(Math.Min(1, d), 0);
        }
        /// <summary>
        /// Calculates an attack on this character via a attack attribute
        /// </summary>
        /// <param name="attrib">attacking attribute</param>
        /// <returns>resulting attack value</returns>
        public virtual double GetAttackAttributeMelee(AttackAttribute attrib)
        {
            return 0;
        }
        protected virtual double GetHaul()
        {
            double d = 0;
            foreach (Item i in this.EquipmentInventory)
                d += i.Weight;

            foreach (Item i in this.ConsumableInventory)
                d += i.Weight;

            return d;
        }
        #endregion

        public bool IsMuted
        {
            get
            {
                return this.States.Any(z => z == CharacterStateType.Silenced);
            }
        }
        public virtual CharacterStateType[] States
        {
            get
            {
                List<CharacterStateType> list = new List<CharacterStateType>();
                list.AddRange(this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)).Select(z => z.State));
                return list.ToArray();
            }
        }

        public virtual void ApplyTemporaryEffect(AlterationEffect alt)
        {
            this.ActiveTemporaryEffects.Add(alt);
        }
        public virtual void ApplyPassiveEffect(AlterationEffect alt)
        {
            this.ActivePassiveEffects.Add(alt);
        }
        public virtual LevelMessageEventArgs[] ApplyPermanentEffect(AlterationEffect alt)
        {
            return null;
        }
        public virtual void ApplyAuraEffect(AlterationEffect auraEffect, AlterationEffect targetEffect)
        {
            //Gets applied to this character
            this.ActiveAuraEffects.Add(auraEffect);

            //Gets applied to other character targets
            this.TargetAuraEffects.Add(targetEffect);
        }
        public virtual void ApplyAlterationCost(AlterationCost alt)
        {

        }
        public virtual void ApplyAttackAttributePassiveEffect(AlterationEffect alt)
        {
            this.AttackAttributePassiveEffects.Add(alt);
        }
        public virtual void ApplyAttackAttributeTemporaryFriendlyEffect(AlterationEffect alt)
        {
            this.AttackAttributeTemporaryFriendlyEffects.Add(alt);
        }
        public virtual void ApplyAttackAttributeTemporaryMalignEffect(AlterationEffect alt)
        {
            this.AttackAttributeTemporaryMalignEffects.Add(alt);
        }

        protected virtual void OnAlterationsChanged()
        {
            foreach (var property in _boundProperties)
                OnPropertyChanged(property);
        }

        public Character() : base() 
        {
            Initialize(true);
        }
        public Character(string name) : base(name) 
        {
            Initialize(true);
        }
        public Character(string name, string symbol, double scale)
            : base(name, symbol, "#FFFFFFFF", scale)
        {
            Initialize(true);
        }
        public Character(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            var properties = typeof(Character).GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }
            Initialize(false);
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(Character).GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
            {
                info.AddValue(property.Name, property.GetValue(this));
            }
        }

        private void Initialize(bool @new)
        {
            this.SavedSymbolInfo = this.SymbolInfo;

            if (@new)
            {
                this.EquipmentInventory = new SerializableObservableCollection<Equipment>();
                this.ConsumableInventory = new SerializableObservableCollection<Consumable>();
                this.TargetAuraEffects = new SerializableObservableCollection<AlterationEffect>();
                this.MalignAuraEffects = new SerializableObservableCollection<AlterationEffect>();
                this.ActiveAuraEffects = new SerializableObservableCollection<AlterationEffect>();
                this.ActivePassiveEffects = new SerializableObservableCollection<AlterationEffect>();
                this.ActiveTemporaryEffects = new SerializableObservableCollection<AlterationEffect>();
                this.AttackAttributePassiveEffects = new SerializableObservableCollection<AlterationEffect>();
                this.AttackAttributeTemporaryFriendlyEffects = new SerializableObservableCollection<AlterationEffect>();
                this.AttackAttributeTemporaryMalignEffects = new SerializableObservableCollection<AlterationEffect>();
            }
            this.ActivePassiveEffects.CollectionAltered += Alterations_CollectionAltered;
            this.ActiveAuraEffects.CollectionAltered += Alterations_CollectionAltered;
            this.MalignAuraEffects.CollectionAltered += Alterations_CollectionAltered;

            this.EquipmentInventory.CollectionAltered += Inventory_CollectionAltered;
            this.ConsumableInventory.CollectionAltered += Inventory_CollectionAltered;
        }

        public abstract LevelMessageEventArgs[] OnDungeonTick(Random r, IEnumerable<AlterationEffect> passiveAuraEffects, bool regenerate, out PlayerAdvancementEventArgs playerAdvancementEventArgs);

        public virtual IEnumerable<AlterationEffect> GetActiveAuras()
        {
            return this.TargetAuraEffects.AsEnumerable();
        }

        // collection affects other properties - raise property changed for affected properties
        protected virtual void Alterations_CollectionAltered(object sender, CollectionAlteredEventArgs e)
        {
            foreach (var property in _boundProperties)
                OnPropertyChanged(property);

            var affectedSymbol = this.Alterations.Count() == 0 ? this.SavedSymbolInfo : base.SymbolInfo;

            //Combine symbol detail deltas
            foreach (var effect in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)).Where(z => z.IsSymbolAlteration))
            {
                //Returns a new instance with combined traits from the parents
                affectedSymbol = Calculator.CalculateSymbolDelta(effect.SymbolAlteration, affectedSymbol);

                //Combine ID's to identify the alteration
                affectedSymbol.Id += effect.SymbolAlteration.Guid;
            }
            this.SymbolInfo = affectedSymbol;
            InvalidateVisual();
        }
        protected virtual void Inventory_CollectionAltered(object sender, CollectionAlteredEventArgs e)
        {
            foreach (var property in _boundProperties)
                OnPropertyChanged(property);
        }
    }
    [Serializable]
    public class Player : Character
    {
        protected const double CRITICAL_HIT_BASE = 0.1;

        public AttributeEmphasis AttributeEmphasis { get; set; }
        public SerializableObservableCollection<SkillSet> Skills { get; set; }

        /// <summary>
        /// Keeps a list of alteration costs for the player - applied on turn
        /// </summary>
        public SerializableObservableCollection<AlterationCost> PerStepAlterationCosts { get; set; }

        SerializableObservableCollection<AttackAttribute> _meleeAttackAttributes { get; set; }

        string _class;
        int _level;
        double _experience;
        double _experienceNext;
        double _hunger;
        double _attack;
        double _foodUsageBase;

        readonly string[] _boundProperties = new string[]{
            "Class",
            "Level",
            "Experience",
            "ExperienceNext",
            "Hunger",
            "Attack",
            "CriticalHitProbability"
        };

        public string Class
        {
            get { return _class; }
            set
            {
                _class = value;
                OnPropertyChanged("Class");
            }
        }
        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;
                OnPropertyChanged("Level");
            }
        }
        public double Experience
        {
            get { return _experience; }
            set
            {
                _experience = value;
                OnPropertyChanged("Experience");
            }
        }
        public double ExperienceNext
        {
            get { return _experienceNext; }
            set
            {
                _experienceNext = value;
                OnPropertyChanged("ExperienceNext");
            }
        }
        public double Hunger
        {
            get { return _hunger; }
            set
            {
                _hunger = value;
                OnPropertyChanged("Hunger");
            }
        }
        public double Attack
        {
            get { return GetAttack(); }
        }
        public double AttackBase
        {
            get { return this.StrengthBase; }
        }
        public double DefenseBase
        {
            get { return this.StrengthBase / 5.0D; }
        }
        public double Defense
        {
            get { return GetDefense(); }
        }
        public double CriticalHitProbability
        {
            get { return GetCriticalHitProbability(); }
        }
        public double FoodUsagePerTurnBase
        {
            get { return _foodUsageBase; }
            set
            {
                _foodUsageBase = value;
                OnPropertyChanged("FoodUsagePerTurnBase");
            }
        }
        public double FoodUsagePerTurn
        {
            get { return GetFoodUsagePerTurn(); }
            set
            {

            }
        }

        public SkillSet ActiveSkill
        {
            get { return this.Skills.FirstOrDefault(x => x.IsActive); }
        }

        private double GetFoodUsagePerTurn()
        {
            double d = this.FoodUsagePerTurnBase + (GetHaul() / HAUL_FOOD_USAGE_DIVISOR);
            foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
                d += alt.FoodUsagePerTurn;
            return Math.Max(0, d);
        }
        private double GetAttack()
        {
            double a = GetStrength();
            foreach (Item i in this.EquipmentInventory)
            {
                if (i is Equipment)
                {
                    Equipment e = i as Equipment;
                    if (e.IsEquiped)
                    {
                        switch (e.Type)
                        {
                            case EquipmentType.OneHandedMeleeWeapon:
                                a += ((e.Class + 1) * e.Quality);
                                break;
                            case EquipmentType.TwoHandedMeleeWeapon:
                                a += ((e.Class + 1) * e.Quality) * 2;
                                break;
                            case EquipmentType.RangeWeapon:
                                a += ((e.Class + 1) * e.Quality) / 2;
                                break;
                        }
                    }
                }
            }
            foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
                a += alt.Attack;

            return Math.Max(0, a);
        }
        private double GetDefense()
        {
            double a = GetStrength() / 5;
            foreach (Item i in this.EquipmentInventory)
            {
                if (i is Equipment)
                {
                    Equipment e = i as Equipment;
                    if (e.IsEquiped)
                    {
                        switch (e.Type)
                        {
                            case EquipmentType.Armor:
                                a += ((e.Class + 1) * e.Quality);
                                break;
                            case EquipmentType.Shoulder:
                                a += (((e.Class + 1) * e.Quality) / 5);
                                break;
                            case EquipmentType.Boots:
                                a += (((e.Class + 1) * e.Quality) / 10);
                                break;
                            case EquipmentType.Gauntlets:
                                a += (((e.Class + 1) * e.Quality) / 10);
                                break;
                            case EquipmentType.Belt:
                                a += (((e.Class + 1) * e.Quality) / 8);
                                break;
                            case EquipmentType.Shield:
                                a += (((e.Class + 1) * e.Quality) / 5);
                                break;
                            case EquipmentType.Helmet:
                                a += (((e.Class + 1) * e.Quality) / 5);
                                break;
                        }
                    }
                }
            }
            foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
                a += alt.Defense;

            return Math.Max(0, a);
        }
        private double GetCriticalHitProbability()
        {
            double d = CRITICAL_HIT_BASE;
            foreach (AlterationEffect alt in this.Alterations.Where(alt => alt.ProjectCharacterCanSupport(this)))
                d += alt.CriticalHit;

            return Math.Max(0, d);
        }

        protected override void OnAlterationsChanged()
        {
            base.OnAlterationsChanged();

            foreach (var property in _boundProperties)
                OnPropertyChanged(property);
        }

        //Attack Attribute Melee
        public override double GetAttackAttributeMelee(AttackAttribute attrib)
        {
            //Malign attack attribute contributions
            double resistance = 0;
            double weakness = 0;
            double attack = attrib.Attack;

            //Friendly attack attribute contributions
            foreach (AlterationEffect friendlyEffect in this.AttackAttributeTemporaryFriendlyEffects)
                resistance += friendlyEffect.AttackAttributes.First(y => y.RogueName == attrib.RogueName).Resistance;

            //Equipment contributions
            foreach (Equipment equipment in this.EquipmentInventory.Where(z => z.IsEquiped))
            {
                resistance += equipment.AttackAttributes.First(y => y.RogueName == attrib.RogueName).Resistance;
                weakness += equipment.AttackAttributes.First(y => y.RogueName == attrib.RogueName).Weakness;
            }

            return Calculator.CalculateAttackAttributeMelee(attack, resistance, weakness);
        }
        public override SerializableObservableCollection<AttackAttribute> MeleeAttackAttributes
        {
            get { return _meleeAttackAttributes; }
        }

        public Player() : base()
        {
            _meleeAttackAttributes = new SerializableObservableCollection<AttackAttribute>();

            this.Skills = new SerializableObservableCollection<SkillSet>();
            this.ExperienceNext = Calculator.CalculateExpNext(this);
            this.PerStepAlterationCosts = new SerializableObservableCollection<AlterationCost>();
            this.IsPhysicallyVisible = true;

            this.Skills.CollectionAltered += (obj, e) => { OnPropertyChanged("ActiveSkill"); };
        }
        public Player(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _meleeAttackAttributes = new SerializableObservableCollection<AttackAttribute>();

            var properties = typeof(Player).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }

            this.Skills.CollectionAltered += (obj, e) => { OnPropertyChanged("ActiveSkill"); };
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(Player).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
                info.AddValue(property.Name, property.GetValue(this));
        }

        public override LevelMessageEventArgs[] OnDungeonTick(Random r, IEnumerable<AlterationEffect> passiveAuraEffects, bool regenerate, out PlayerAdvancementEventArgs playerAdvancementEventArgs)
        {
            this.MalignAuraEffects.Clear();
            this.MalignAuraEffects.AddRange(passiveAuraEffects);

            playerAdvancementEventArgs = null;
            List<LevelMessageEventArgs> msgs = new List<LevelMessageEventArgs>();

            //Normal turn stuff
            this.Hp += this.GetHpRegen(regenerate);
            this.Mp += this.GetMpRegen();

            this.Hunger += this.GetFoodUsagePerTurn();

            if (this.Experience >= this.ExperienceNext)
            {
                playerAdvancementEventArgs = Calculator.CalculateLevelGains(this, r);
                this.ExperienceNext = Calculator.CalculateExpNext(this);

                //Bonus health and magic refill
                this.Hp = this.HpMax;
                this.Mp = this.MpMax;
            }

            //Normal temporary effects
            for (int i = this.ActiveTemporaryEffects.Count - 1; i >= 0; i--)
            {
                //Check temporary event time
                this.ActiveTemporaryEffects[i].EventTime--;
                if (this.ActiveTemporaryEffects[i].EventTime < 0)
                {
                    msgs.Add(new LevelMessageEventArgs(this.ActiveTemporaryEffects[i].PostEffectString));
                    this.ActiveTemporaryEffects.RemoveAt(i);
                }
            }

            //Attack attribute temporary effects
            for (int i = this.AttackAttributeTemporaryFriendlyEffects.Count - 1; i >= 0; i--)
            {
                this.AttackAttributeTemporaryFriendlyEffects[i].EventTime--;
                if (this.AttackAttributeTemporaryFriendlyEffects[i].EventTime < 0)
                {
                    msgs.Add(new LevelMessageEventArgs(this.AttackAttributeTemporaryFriendlyEffects[i].PostEffectString));
                    this.AttackAttributeTemporaryFriendlyEffects.RemoveAt(i);
                }
            }
            for (int i = this.AttackAttributeTemporaryMalignEffects.Count - 1; i >= 0; i--)
            {
                this.AttackAttributeTemporaryMalignEffects[i].EventTime--;
                if (this.AttackAttributeTemporaryMalignEffects[i].EventTime < 0)
                {
                    msgs.Add(new LevelMessageEventArgs(this.AttackAttributeTemporaryMalignEffects[i].PostEffectString));
                    this.AttackAttributeTemporaryMalignEffects.RemoveAt(i);
                }
            }

            //Apply per step alteration costs
            foreach (AlterationCost alt in this.PerStepAlterationCosts)
            {
                this.AgilityBase -= alt.Agility;
                this.AuraRadiusBase -= alt.AuraRadius;
                this.Experience -= alt.Experience;
                this.FoodUsagePerTurnBase += alt.FoodUsagePerTurn;
                this.Hp -= alt.Hp;
                this.Hunger += alt.Hunger;
                this.IntelligenceBase -= alt.Intelligence;
                this.Mp -= alt.Mp;
                this.StrengthBase -= alt.Strength;
            }

            //Message for effects that player can't support
            foreach (AlterationEffect effect in this.Alterations.Where(alt => !alt.ProjectCharacterCanSupport(this)))
                msgs.Add(new LevelMessageEventArgs(effect.PostEffectString));

            ApplyLimits();

            return msgs.Where(z => z.Message != null).ToArray();
        }
        public override void ApplyAlterationCost(AlterationCost alt)
        {
            if (alt.Type == AlterationCostType.OneTime)
            {
                this.AgilityBase -= alt.Agility;
                this.AuraRadiusBase -= alt.AuraRadius;
                this.Experience -= alt.Experience;
                this.FoodUsagePerTurnBase += alt.FoodUsagePerTurn;
                this.Hp -= alt.Hp;
                this.Hunger += alt.Hunger;
                this.IntelligenceBase -= alt.Intelligence;
                this.Mp -= alt.Mp;
                this.StrengthBase -= alt.Strength;
            }
            else if (alt.Type == AlterationCostType.PerStep)
            {
                this.PerStepAlterationCosts.Add(alt);
            }

            ApplyLimits();
        }
        public override LevelMessageEventArgs[] ApplyPermanentEffect(AlterationEffect alt)
        {
            this.StrengthBase += alt.Strength;
            this.IntelligenceBase += alt.Intelligence;
            this.AgilityBase += alt.Agility;
            this.AuraRadiusBase += alt.AuraRadius;
            this.FoodUsagePerTurnBase += alt.FoodUsagePerTurn;

            //Blockable - if negative then block a fraction of the amount
            this.Experience += /*(alt.Experience < 0) ? alt.Experience * Calculator.CalculateTransendentalFraction(this.MagicDefense() / alt.Experience) :*/ alt.Experience;
            this.Hunger += /*(alt.Hunger < 0) ? alt.Hunger * Calculator.CalculateTransendentalFraction(this.MagicDefense() / alt.Hunger) :*/ alt.Hunger;
            this.Hp += /*(alt.Hp < 0) ? alt.Hp * Calculator.CalculateTransendentalFraction(this.MagicDefense() / alt.Hp) :*/ alt.Hp;
            this.Mp += /*(alt.Mp < 0) ? alt.Mp * Calculator.CalculateTransendentalFraction(this.MagicDefense() / alt.Mp) :*/ alt.Mp;

            List<LevelMessageEventArgs> msgs = new List<LevelMessageEventArgs>();

            //Apply remedies
            for (int i = this.ActiveTemporaryEffects.Count - 1; i >= 0; i--)
            {
                if (alt.RemediedSpellNames.Contains(this.ActiveTemporaryEffects[i].RogueName))
                {
                    msgs.Add(new LevelMessageEventArgs(this.ActiveTemporaryEffects[i].DisplayName + " has been cured!"));                    
                    this.ActiveTemporaryEffects.RemoveAt(i);
                }
            }
            for (int i = this.AttackAttributeTemporaryFriendlyEffects.Count - 1; i >= 0; i--)
            {
                if (alt.RemediedSpellNames.Contains(this.AttackAttributeTemporaryFriendlyEffects[i].RogueName))
                {
                    msgs.Add(new LevelMessageEventArgs(this.AttackAttributeTemporaryFriendlyEffects[i].DisplayName + " has been cured!"));
                    this.AttackAttributeTemporaryFriendlyEffects.RemoveAt(i);
                }
            }
            for (int i = this.AttackAttributeTemporaryMalignEffects.Count - 1; i >= 0; i--)
            {
                if (alt.RemediedSpellNames.Contains(this.AttackAttributeTemporaryMalignEffects[i].RogueName))
                {
                    msgs.Add(new LevelMessageEventArgs(this.AttackAttributeTemporaryMalignEffects[i].DisplayName + " has been cured!"));
                    this.AttackAttributeTemporaryMalignEffects.RemoveAt(i);
                }
            }

            ApplyLimits();
            return msgs.ToArray();
        }
        public void DeactivatePassiveEffect(string spellId)
        {
            AlterationEffect effect = this.ActivePassiveEffects.FirstOrDefault(z => spellId == z.SpellId);
            if (effect != null)
                this.ActivePassiveEffects.Remove(effect);

            AlterationEffect attackAttributeEffect = this.AttackAttributePassiveEffects.FirstOrDefault(z => z.SpellId == spellId);
            if (attackAttributeEffect != null)
                this.AttackAttributePassiveEffects.Remove(attackAttributeEffect);

            AlterationCost cost = this.PerStepAlterationCosts.FirstOrDefault(z => spellId == z.SpellId);
            if (cost != null)
                this.PerStepAlterationCosts.Remove(cost);
        }
        public void DeactivatePassiveAura(string spellId)
        {
            AlterationEffect effect = this.ActiveAuraEffects.FirstOrDefault(z => spellId == z.SpellId);
            if (effect != null)
                this.ActiveAuraEffects.Remove(effect);

            AlterationEffect targetEffect = this.TargetAuraEffects.FirstOrDefault(z => spellId == z.SpellId);
            if (targetEffect != null)
                this.TargetAuraEffects.Remove(targetEffect);

            AlterationCost cost = this.PerStepAlterationCosts.FirstOrDefault(z => spellId == z.SpellId);
            if (cost != null)
                this.PerStepAlterationCosts.Remove(cost);
        }
        protected void ApplyLimits()
        {
            if (this.Mp < 0)
                this.Mp = 0;

            if (this.Hp > this.HpMax)
                this.Hp = this.HpMax;

            if (this.Mp > this.MpMax)
                this.Mp = this.MpMax;

            if (this.Hunger < 0)
                this.Hunger = 0;

            if (this.StrengthBase < 0)
                this.StrengthBase = 0;

            if (this.AgilityBase < 0)
                this.AgilityBase = 0;

            if (this.IntelligenceBase < 0)
                this.IntelligenceBase = 0;

            if (this.AuraRadiusBase < 0)
                this.AuraRadiusBase = 0;

            if (this.FoodUsagePerTurnBase < 0)
                this.FoodUsagePerTurnBase = 0;

            if (this.Experience < 0)
                this.Experience = 0;
        }

        private void RebuildAttackAlterations()
        {
            // rebuild attack attributes list
            List<AttackAttribute> list = new List<AttackAttribute>();
            foreach (AlterationEffect effect in this.AttackAttributeTemporaryFriendlyEffects)
                list.AddRange(effect.AttackAttributes);

            foreach (Equipment e in this.EquipmentInventory.Where(z => z.IsEquiped))
                list.AddRange(e.AttackAttributes);

            // zero out list before re-applying
            foreach (AttackAttribute attrib in _meleeAttackAttributes)
            {
                attrib.Attack = 0;
                attrib.Resistance = 0;
                attrib.Weakness = 0;
            }

            foreach (AttackAttribute attrib in list)
            {
                if (!_meleeAttackAttributes.Any(z => z.RogueName == attrib.RogueName))
                    _meleeAttackAttributes.Add(new AttackAttribute()
                    {
                        RogueName = attrib.RogueName,
                        Attack = attrib.Attack,
                        Weakness = attrib.Weakness,
                        Resistance = attrib.Resistance,
                        SymbolInfo = attrib.SymbolInfo
                    });

                else
                {
                    var existingAttrib = _meleeAttackAttributes.First(z => z.RogueName == attrib.RogueName);
                    existingAttrib.Attack += attrib.Attack;
                    existingAttrib.Resistance += attrib.Resistance;
                    existingAttrib.Weakness += attrib.Weakness;
                }
            }
        }

        protected override void Alterations_CollectionAltered(object sender, CollectionAlteredEventArgs ev)
        {
            RebuildAttackAlterations();

            base.Alterations_CollectionAltered(sender, ev);
        }
        protected override void Inventory_CollectionAltered(object sender, CollectionAlteredEventArgs e)
        {
            RebuildAttackAlterations();

            base.Inventory_CollectionAltered(sender, e);
        }
    }
    [Serializable]
    public class Enemy : Character
    {
        public string CreatureClass { get; set; }
        public BehaviorDetails BehaviorDetails { get; set; }
        public double ExperienceGiven { get; set; }
        public double TurnCounter { get; set; }
        public bool IsEngaged { get; set; }
        public bool WasAttackedByPlayer { get; set; }
        public SerializableObservableCollection<AttackAttribute> AttackAttributes { get; set; }

        public override double GetAttackAttributeMelee(AttackAttribute attrib)
        {
            //Malign attack attribute contributions
            double resistance = this.AttackAttributes.First(z => z.RogueName == attrib.RogueName).Resistance;
            double weakness = this.AttackAttributes.First(z => z.RogueName == attrib.RogueName).Weakness;
            double attack = attrib.Attack;

            //Friendly attack attribute contributions
            foreach (AlterationEffect friendlyEffect in this.AttackAttributeTemporaryFriendlyEffects)
                resistance += friendlyEffect.AttackAttributes.First(y => y.RogueName == attrib.RogueName).Resistance;

            //Equipment contributions
            foreach (Equipment equipment in this.EquipmentInventory.Where(z => z.IsEquiped))
            {
                resistance += equipment.AttackAttributes.First(y => y.RogueName == attrib.RogueName).Resistance;
                weakness += equipment.AttackAttributes.First(y => y.RogueName == attrib.RogueName).Weakness;
            }

            return Calculator.CalculateAttackAttributeMelee(attack, resistance, weakness);
        }
        public override SerializableObservableCollection<AttackAttribute> MeleeAttackAttributes
        {
            get
            {
                return this.AttackAttributes;
            }
        }

        public Enemy() : base()
        {
            this.BehaviorDetails = new BehaviorDetails();
            this.AttackAttributes = new SerializableObservableCollection<AttackAttribute>();
        }
        public Enemy(string symbol, string name) : base(name)
        {
            this.BehaviorDetails = new BehaviorDetails();
            this.SymbolInfo.Type = SymbolTypes.Character;
            this.SymbolInfo.CharacterSymbol = symbol;
            this.AttackAttributes = new SerializableObservableCollection<AttackAttribute>();
        }
        public Enemy(string symbol, string name, double scale)
            : base(name, symbol, scale)
        {
            this.BehaviorDetails = new BehaviorDetails();
            this.SymbolInfo.Type = SymbolTypes.Character;
            this.SymbolInfo.CharacterSymbol = symbol;
            this.AttackAttributes = new SerializableObservableCollection<AttackAttribute>();
        }

        public Enemy(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var properties = typeof(Enemy).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(Enemy).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
                info.AddValue(property.Name, property.GetValue(this));
        }


        public override LevelMessageEventArgs[] OnDungeonTick(Random r, IEnumerable<AlterationEffect> passiveAuraEffects, bool regenerate, out PlayerAdvancementEventArgs playerAdvancementEventArgs)
        {
            this.MalignAuraEffects.Clear();
            this.MalignAuraEffects.AddRange(passiveAuraEffects);

            playerAdvancementEventArgs = null;
            LevelMessageEventArgs[] msgs = new LevelMessageEventArgs[] { };

            if (regenerate)
            {
                this.Hp += this.GetHpRegen(regenerate);
                this.Mp += this.GetMpRegen();
            }

            for (int i = this.ActiveTemporaryEffects.Count - 1; i >= 0; i--)
            {
                //Check temporary event
                this.ActiveTemporaryEffects[i].EventTime--;
                if (this.ActiveTemporaryEffects[i].EventTime < 0)
                {
                    this.ActiveTemporaryEffects.RemoveAt(i);
                    continue;
                }
            }


            //Attack attribute temporary effects
            for (int i = this.AttackAttributeTemporaryFriendlyEffects.Count - 1; i >= 0; i--)
            {
                this.AttackAttributeTemporaryFriendlyEffects[i].EventTime--;
                if (this.AttackAttributeTemporaryFriendlyEffects[i].EventTime < 0)
                {
                    this.AttackAttributeTemporaryFriendlyEffects.RemoveAt(i);
                }
            }
            for (int i = this.AttackAttributeTemporaryMalignEffects.Count - 1; i >= 0; i--)
            {
                this.AttackAttributeTemporaryMalignEffects[i].EventTime--;
                if (this.AttackAttributeTemporaryMalignEffects[i].EventTime < 0)
                {
                    this.AttackAttributeTemporaryMalignEffects.RemoveAt(i);
                }
            }

            ApplyLimits();

            return msgs;
        }
        public override void ApplyAlterationCost(AlterationCost alt)
        {
            if (alt.Type == AlterationCostType.OneTime)
            {
                this.AgilityBase -= alt.Agility;
                this.AuraRadiusBase -= alt.AuraRadius;
                this.Hp -= alt.Hp;
                this.IntelligenceBase -= alt.Intelligence;
                this.Mp -= alt.Mp;
                this.StrengthBase -= alt.Strength;
            }
        }
        public override LevelMessageEventArgs[] ApplyPermanentEffect(AlterationEffect alt)
        {
            this.StrengthBase += alt.Strength;
            this.IntelligenceBase += alt.Intelligence;
            this.AgilityBase += alt.Agility;
            this.AuraRadiusBase += alt.AuraRadius;
            this.Hp += alt.Hp;
            this.Mp += alt.Mp;

            return new LevelMessageEventArgs[] { };
        }
        protected void ApplyLimits()
        {
            if (this.Mp < 0)
                this.Mp = 0;

            if (this.Hp > this.HpMax)
                this.Hp = this.HpMax;

            if (this.Mp > this.MpMax)
                this.Mp = this.MpMax;

            if (this.StrengthBase < 0)
                this.StrengthBase = 0;

            if (this.AgilityBase < 0)
                this.AgilityBase = 0;

            if (this.IntelligenceBase < 0)
                this.IntelligenceBase = 0;
        }
        public override string ToString()
        {
            return this.RogueName + ":" + this.SymbolInfo.ToString() + ":" + this.Id;
        }
    }
}