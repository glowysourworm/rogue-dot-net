using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Content.Item.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Rogue.NET.Scenario.Content.ViewModel.Content.ScenarioMetaData;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Alteration.Extension;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;

namespace Rogue.NET.Scenario.ViewModel.ItemGrid
{
    public class ItemGridRowViewModel : NotifyViewModel
    {
        static readonly string INFINITY = Encoding.UTF8.GetString(new byte[] { 0xE2, 0x88, 0x9E });

        #region (private) Backing Fields
        string _id;
        bool _consumeEnable;
        bool _identifyEnable;
        bool _equipEnable;
        bool _dropEnable;
        bool _uncurseEnable;
        bool _selectEnable;
        bool _throwEnable;
        bool _enchantArmorEnable;
        bool _enchantWeaponEnable;
        bool _imbueArmorEnable;
        bool _imbueWeaponEnable;
        bool _enhanceArmorEnable;
        bool _enhanceWeaponEnable;
        bool _isEquiped;
        bool _isCursed;
        bool _isObjective;
        bool _isUnique;
        bool _isConsumable;
        bool _isEquipment;
        bool _isWeapon;
        bool _isArmor;
        bool _hasCharacterClassRequirement;
        int _quantity;
        string _uses;
        int _requiredLevel;
        string _type;
        EquipmentType _equipType;
        CharacterBaseAttribute _combatType;
        string _class;
        string _weight;
        string _quality;
        string _attackValue;
        string _defenseValue;
        string _rogueName;
        string _displayName;
        string _characterSymbol;
        string _characterColor;
        ImageResources _icon;
        DisplayImageResources _displayIcon;
        SmileyMoods _smileyMood;
        string _smileyBodyColor;
        string _smileyLineColor;
        string _smileyAuraColor;
        SymbolTypes _symbolType;
        ScenarioImageViewModel _characterClass;
        #endregion

        #region (public) Properties
        public ObservableCollection<AttackAttributeViewModel> AttackAttributes { get; set; }

        public string Id
        {
            get { return _id; }
            set { this.RaiseAndSetIfChanged(ref _id, value); }
        }
        public bool ConsumeEnable
        {
            get { return _consumeEnable; }
            set { this.RaiseAndSetIfChanged(ref _consumeEnable, value); }
        }
        public bool IdentifyEnable
        {
            get { return _identifyEnable; }
            set { this.RaiseAndSetIfChanged(ref _identifyEnable, value); }
        }
        public bool EquipEnable
        {
            get { return _equipEnable; }
            set { this.RaiseAndSetIfChanged(ref _equipEnable, value); }
        }
        public bool DropEnable
        {
            get { return _dropEnable; }
            set { this.RaiseAndSetIfChanged(ref _dropEnable, value); }
        }
        public bool UncurseEnable
        {
            get { return _uncurseEnable; }
            set { this.RaiseAndSetIfChanged(ref _uncurseEnable, value); }
        }
        public bool SelectEnable
        {
            get { return _selectEnable; }
            set { this.RaiseAndSetIfChanged(ref _selectEnable, value); }
        }
        public bool ThrowEnable
        {
            get { return _throwEnable; }
            set { this.RaiseAndSetIfChanged(ref _throwEnable, value); }
        }
        public bool EnchantArmorEnable
        {
            get { return _enchantArmorEnable; }
            set { this.RaiseAndSetIfChanged(ref _enchantArmorEnable, value); }
        }
        public bool EnchantWeaponEnable
        {
            get { return _enchantWeaponEnable; }
            set { this.RaiseAndSetIfChanged(ref _enchantWeaponEnable, value); }
        }
        public bool ImbueArmorEnable
        {
            get { return _imbueArmorEnable; }
            set { this.RaiseAndSetIfChanged(ref _imbueArmorEnable, value); }
        }
        public bool ImbueWeaponEnable
        {
            get { return _imbueWeaponEnable; }
            set { this.RaiseAndSetIfChanged(ref _imbueWeaponEnable, value); }
        }
        public bool EnhanceArmorEnable
        {
            get { return _enhanceArmorEnable; }
            set { this.RaiseAndSetIfChanged(ref _enhanceArmorEnable, value); }
        }
        public bool EnhanceWeaponEnable
        {
            get { return _enhanceWeaponEnable; }
            set { this.RaiseAndSetIfChanged(ref _enhanceWeaponEnable, value); }
        }
        public bool IsEquiped
        {
            get { return _isEquiped; }
            set { this.RaiseAndSetIfChanged(ref _isEquiped, value); }
        }
        public bool IsCursed
        {
            get { return _isCursed; }
            set { this.RaiseAndSetIfChanged(ref _isCursed, value); }
        }
        public bool IsObjective
        {
            get { return _isObjective; }
            set { this.RaiseAndSetIfChanged(ref _isObjective, value); }
        }
        public bool IsUnique
        {
            get { return _isUnique; }
            set { this.RaiseAndSetIfChanged(ref _isUnique, value); }
        }
        public bool IsWeapon
        {
            get { return _isWeapon; }
            set { this.RaiseAndSetIfChanged(ref _isWeapon, value); }
        }
        public bool IsArmor
        {
            get { return _isArmor; }
            set { this.RaiseAndSetIfChanged(ref _isArmor, value); }
        }
        public bool HasCharacterClassRequirement
        {
            get { return _hasCharacterClassRequirement; }
            set { this.RaiseAndSetIfChanged(ref _hasCharacterClassRequirement, value); }
        }
        public int Quantity
        {
            get { return _quantity; }
            set { this.RaiseAndSetIfChanged(ref _quantity, value); }
        }
        public string Uses
        {
            get { return _uses; }
            set { this.RaiseAndSetIfChanged(ref _uses, value); }
        }
        public int RequiredLevel
        {
            get { return _requiredLevel; }
            set { this.RaiseAndSetIfChanged(ref _requiredLevel, value); }
        }
        public string Type
        {
            get { return _type; }
            set { this.RaiseAndSetIfChanged(ref _type, value); }
        }
        public EquipmentType EquipType
        {
            get { return _equipType; }
            set { this.RaiseAndSetIfChanged(ref _equipType, value); }
        }
        public CharacterBaseAttribute CombatType
        {
            get { return _combatType; }
            set { this.RaiseAndSetIfChanged(ref _combatType, value); }
        }
        public string AttackValue
        {
            get { return _attackValue; }
            set { this.RaiseAndSetIfChanged(ref _attackValue, value); }
        }
        public string DefenseValue
        {
            get { return _defenseValue; }
            set { this.RaiseAndSetIfChanged(ref _defenseValue, value); }
        }
        public string Class
        {
            get { return _class; }
            set { this.RaiseAndSetIfChanged(ref _class, value); }
        }
        public string Weight
        {
            get { return _weight; }
            set { this.RaiseAndSetIfChanged(ref _weight, value); }
        }
        public string Quality
        {
            get { return _quality; }
            set { this.RaiseAndSetIfChanged(ref _quality, value); }
        }
        public bool IsConsumable
        {
            get { return _isConsumable; }
            set { this.RaiseAndSetIfChanged(ref _isConsumable, value); }
        }
        public bool IsEquipment
        {
            get { return _isEquipment; }
            set { this.RaiseAndSetIfChanged(ref _isEquipment, value); }
        }
        public string RogueName
        {
            get { return _rogueName; }
            set { this.RaiseAndSetIfChanged(ref _rogueName, value); }
        }
        public string DisplayName
        {
            get { return _displayName; }
            set { this.RaiseAndSetIfChanged(ref _displayName, value); }
        }
        public ScenarioImageViewModel CharacterClass
        {
            get { return _characterClass; }
            set { this.RaiseAndSetIfChanged(ref _characterClass, value); }
        }

        // Symbol Details
        public string CharacterSymbol
        {
            get { return _characterSymbol; }
            set { this.RaiseAndSetIfChanged(ref _characterSymbol, value); }
        }
        public string CharacterColor
        {
            get { return _characterColor; }
            set { this.RaiseAndSetIfChanged(ref _characterColor, value); }
        }
        public ImageResources Icon
        {
            get { return _icon; }
            set { this.RaiseAndSetIfChanged(ref _icon, value); }
        }
        public DisplayImageResources DisplayIcon
        {
            get { return _displayIcon; }
            set { this.RaiseAndSetIfChanged(ref _displayIcon, value); }
        }
        public SmileyMoods SmileyMood
        {
            get { return _smileyMood; }
            set { this.RaiseAndSetIfChanged(ref _smileyMood, value); }
        }
        public string SmileyBodyColor
        {
            get { return _smileyBodyColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyBodyColor, value); }
        }
        public string SmileyLineColor
        {
            get { return _smileyLineColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyLineColor, value); }
        }
        public string SmileyAuraColor
        {
            get { return _smileyAuraColor; }
            set { this.RaiseAndSetIfChanged(ref _smileyAuraColor, value); }
        }
        public SymbolTypes SymbolType
        {
            get { return _symbolType; }
            set { this.RaiseAndSetIfChanged(ref _symbolType, value); }
        }
        #endregion

        public ItemGridRowViewModel(Equipment equipment, ScenarioMetaData metaData)
        {
            this.AttackAttributes = new ObservableCollection<AttackAttributeViewModel>();

            UpdateEquipment(equipment, metaData);
        }
        public ItemGridRowViewModel(Consumable consumable, ScenarioMetaData metaData, bool identifyConsumable, int totalQuantity, int totalUses, double totalWeight)
        {
            this.AttackAttributes = new ObservableCollection<AttackAttributeViewModel>();

            UpdateConsumable(consumable, metaData, identifyConsumable, totalQuantity, totalUses, totalWeight);
        }
        public void UpdateConsumable(Consumable consumable, ScenarioMetaData metaData, bool identifyConsumable, int totalQuantity, int totalUses, double totalWeight)
        {
            this.Id = consumable.Id;
            this.RogueName = consumable.RogueName;

            // Make sure identifyConsumable applies to consume an identify item (there's at least one thing to identify)
            bool isIdentify = consumable.HasAlteration && consumable.Alteration.Effect.IsIdentify();

            // Level and Character Class requirements are handled by other code. Allow user to TRY to consume for those
            bool consumeEnable = (consumable.HasAlteration && consumable.SubType != ConsumableSubType.Ammo && !isIdentify) || 
                                 (consumable.HasLearnedSkillSet) ||
                                 (consumable.SubType == ConsumableSubType.Note) || // Always allow using notes
                                 (isIdentify && identifyConsumable);

            bool throwEnable = consumable.HasProjectileAlteration;

            this.Quality = "-";
            this.Class = "-";

            this.IsEquipment = false;
            this.IsConsumable = true;
            this.IsArmor = false;
            this.IsWeapon = false;

            // Character Class
            this.HasCharacterClassRequirement = consumable.HasCharacterClassRequirement;

            // TODO:CHARACTERCLASS
            //if (consumable.HasCharacterClassRequirement)
            //    this.CharacterClass = new ScenarioImageViewModel(characterClasses.First(x => x.RogueName == consumable.CharacterClass));
                                                

            this.Quantity = totalQuantity;
            this.Weight = totalWeight.ToString("F2");
            this.AttackValue = "-";
            this.DefenseValue = "-";
            this.Type = consumable.SubType.ToString();
            this.Uses = consumable.Type == ConsumableType.UnlimitedUses ? INFINITY :
                        consumable.Type == ConsumableType.MultipleUses ? totalUses.ToString() : "1";
            this.RequiredLevel = consumable.LevelRequired;

            this.EquipEnable = false;
            this.ConsumeEnable = consumeEnable;
            this.IdentifyEnable = !metaData.IsIdentified;
            this.UncurseEnable = false;
            this.DropEnable = true;
            this.SelectEnable = true;
            this.ThrowEnable = throwEnable;
            this.EnchantArmorEnable = false;
            this.EnchantWeaponEnable = false;
            this.ImbueArmorEnable = false;
            this.ImbueWeaponEnable = false;
            this.EnhanceArmorEnable = false;
            this.EnhanceWeaponEnable = false;

            // Symbol Details
            this.CharacterSymbol = consumable.CharacterSymbol;
            this.CharacterColor = consumable.CharacterColor;
            this.Icon = consumable.Icon;
            this.DisplayIcon = consumable.DisplayIcon;
            this.SmileyMood = consumable.SmileyMood;
            this.SmileyBodyColor = consumable.SmileyBodyColor;
            this.SmileyLineColor = consumable.SmileyLineColor;
            this.SmileyAuraColor = consumable.SmileyLightRadiusColor;
            this.SymbolType = consumable.SymbolType;

            this.IsEquiped = false;
            this.IsCursed = false;
            this.IsObjective = (metaData.IsObjective && metaData.IsIdentified);
            this.IsUnique = (metaData.IsUnique && metaData.IsIdentified);

            if (!metaData.IsIdentified)
            {
                this.DisplayName = "???";
            }
            // Different rule for consumable of multiple uses
            else
            {
                if (consumable.Type == ConsumableType.MultipleUses)
                {
                    this.DisplayName = consumable.RogueName + " (" + consumable.Uses.ToString("N0") + ")";
                }

                else if (consumable.Type == ConsumableType.UnlimitedUses)
                {
                    //Infinity symbol
                    this.DisplayName = consumable.RogueName + " (" + INFINITY + ")";
                }

                else
                    this.DisplayName = consumable.RogueName;
            }
        }
        public void UpdateEquipment(Equipment equipment, ScenarioMetaData metaData)
        {
            this.Id = equipment.Id;
            this.RogueName = equipment.RogueName;
            this.DisplayName = metaData.IsIdentified ? equipment.RogueName : "???";
            this.Type = equipment.Type.ToString();
            this.EquipType = equipment.Type;
            this.CombatType = equipment.CombatType;

            this.IsEquiped = equipment.IsEquipped;
            this.IsCursed = equipment.IsCursed && metaData.IsCurseIdentified;
            this.IsObjective = (metaData.IsObjective && metaData.IsIdentified);
            this.IsUnique = (metaData.IsUnique && metaData.IsIdentified);

            this.IsEquipment = true;
            this.IsConsumable = false;
            this.IsArmor = equipment.IsArmorType();
            this.IsWeapon = equipment.IsWeaponType();

            this.Quantity = 1;
            this.Quality = equipment.ClassApplies() && equipment.IsIdentified ? equipment.Quality.ToString("F2") :
                                                       equipment.IsIdentified ? "-" : "?";
            this.Class = equipment.ClassApplies() && equipment.IsIdentified ? equipment.Class.ToString("F0") :
                                                     equipment.IsIdentified ? "-" : "?";

            // TODO:CHARACTERCLASS
            // Character Class
            this.HasCharacterClassRequirement = equipment.HasCharacterClassRequirement;
            //if (equipment.HasCharacterClassRequirement)
            //    this.CharacterClass = new ScenarioImageViewModel(characterClasses.First(x => x.RogueName == equipment.CharacterClass.RogueName));

            // Attack and Defense Value
            if (!equipment.IsIdentified)
            {
                this.AttackValue = "?";
                this.DefenseValue = "?";
            }
            else
            {
                this.AttackValue = equipment.GetAttackValue().ToString("F2");
                this.DefenseValue = equipment.GetDefenseValue().ToString("F2");
            }

            this.Weight = equipment.Weight.ToString("F2");
            this.RequiredLevel = equipment.LevelRequired;
            this.EquipEnable = true;
            this.ConsumeEnable = false;
            this.IdentifyEnable = !metaData.IsIdentified || (!equipment.IsIdentified && equipment.ClassApplies());
            this.UncurseEnable = equipment.IsCursed;
            this.DropEnable = true;
            this.SelectEnable = true;
            this.ThrowEnable = false;
            this.EnchantArmorEnable = equipment.ClassApplies() && equipment.IsArmorType();
            this.EnchantWeaponEnable = equipment.ClassApplies() && equipment.IsWeaponType();
            this.ImbueArmorEnable = equipment.CanImbue() && equipment.IsArmorType();
            this.ImbueWeaponEnable = equipment.CanImbue() && equipment.IsWeaponType();
            this.EnhanceArmorEnable = equipment.ClassApplies() && equipment.IsArmorType();
            this.EnhanceWeaponEnable = equipment.ClassApplies() && equipment.IsWeaponType();

            // Symbol Details
            this.CharacterSymbol = equipment.CharacterSymbol;
            this.CharacterColor = equipment.CharacterColor;
            this.Icon = equipment.Icon;
            this.DisplayIcon = equipment.DisplayIcon;
            this.SmileyMood = equipment.SmileyMood;
            this.SmileyBodyColor = equipment.SmileyBodyColor;
            this.SmileyLineColor = equipment.SmileyLineColor;
            this.SmileyAuraColor = equipment.SmileyLightRadiusColor;
            this.SymbolType = equipment.SymbolType;

            this.AttackAttributes.Clear();

            if (equipment.IsIdentified)
                this.AttackAttributes.AddRange(equipment.AttackAttributes
                                                        .Where(x => x.Resistance > 0 || x.Attack > 0)
                                                        .Select(x => new AttackAttributeViewModel(x)));

            // Fire PropertyChanged event for AttackAttributes to update bindings
            OnPropertyChanged("AttackAttributes");           
        }
    }
}
