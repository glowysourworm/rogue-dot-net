using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario;
using Rogue.NET.Core.Model.Scenario.Alteration;
using Rogue.NET.Core.Model.Scenario.Content.Item;
using Rogue.NET.Scenario.Content.ViewModel.Content;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Scenario.ViewModel.ItemGrid
{
    public class ItemGridRowViewModel : NotifyViewModel
    {

        #region (private) Backing Fields
        string _id;
        bool _consumeEnable;
        bool _identifyEnable;
        bool _equipEnable;
        bool _dropEnable;
        bool _uncurseEnable;
        bool _selectEnable;
        bool _detailsEnable;
        bool _throwEnable;
        bool _enchantEnable;
        bool _isEquiped;
        bool _isCursed;
        bool _isObjective;
        bool _isUnique;
        bool _isConsumable;
        bool _isEquipment;
        int _quantity;
        int _uses;
        string _type;
        EquipmentType _equipType;
        string _class;
        string _weight;
        string _quality;
        string _shortDescription;
        string _longDescription;
        string _usageDescription;
        string _rogueName;
        string _displayName;
        string _characterSymbol;
        string _characterColor;
        ImageResources _icon;
        SmileyMoods _smileyMood;
        string _smileyBodyColor;
        string _smileyLineColor;
        string _smileyAuraColor;
        SymbolTypes _symbolType;
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
        public bool DetailsEnable
        {
            get { return _detailsEnable; }
            set { this.RaiseAndSetIfChanged(ref _detailsEnable, value); }
        }
        public bool ThrowEnable
        {
            get { return _throwEnable; }
            set { this.RaiseAndSetIfChanged(ref _throwEnable, value); }
        }
        public bool EnchantEnable
        {
            get { return _enchantEnable; }
            set { this.RaiseAndSetIfChanged(ref _enchantEnable, value); }
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
        public int Quantity
        {
            get { return _quantity; }
            set { this.RaiseAndSetIfChanged(ref _quantity, value); }
        }
        public int Uses
        {
            get { return _uses; }
            set { this.RaiseAndSetIfChanged(ref _uses, value); }
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
        public string ShortDescription
        {
            get { return _shortDescription; }
            set { this.RaiseAndSetIfChanged(ref _shortDescription, value); }
        }
        public string LongDescription
        {
            get { return _longDescription; }
            set { this.RaiseAndSetIfChanged(ref _longDescription, value); }
        }
        public string UsageDescription
        {
            get { return _usageDescription; }
            set { this.RaiseAndSetIfChanged(ref _usageDescription, value); }
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
            bool isIdentify = consumable.HasSpell && consumable.Spell.OtherEffectType == AlterationMagicEffectType.Identify;

            bool consumeEnable = (consumable.HasSpell && consumable.SubType != ConsumableSubType.Ammo && !isIdentify) || (isIdentify && identifyConsumable);
            bool throwEnable = consumable.HasProjectileSpell && consumable.SubType != ConsumableSubType.Ammo;

            string quality = "N/A";
            string classs = "N/A";

            this.IsEquipment = false;
            this.IsConsumable = true;

            this.Quantity = totalQuantity;
            this.Weight = totalWeight.ToString("F2");
            this.Type = consumable.SubType.ToString();
            this.Uses = totalUses;

            this.EquipEnable = false;
            this.ConsumeEnable = consumeEnable;
            this.IdentifyEnable = !metaData.IsIdentified;
            this.UncurseEnable = false;
            this.DropEnable = true;
            this.SelectEnable = true;
            this.ThrowEnable = throwEnable;
            this.EnchantEnable = false;

            this.UsageDescription = CreateConsumableUsageDescription(consumable.SubType, throwEnable, consumeEnable, this.Quantity, this.Uses);

            // Symbol Details
            this.CharacterSymbol = consumable.CharacterSymbol;
            this.CharacterColor = consumable.CharacterColor;
            this.Icon = consumable.Icon;
            this.SmileyMood = consumable.SmileyMood;
            this.SmileyBodyColor = consumable.SmileyBodyColor;
            this.SmileyLineColor = consumable.SmileyLineColor;
            this.SmileyAuraColor = consumable.SmileyAuraColor;
            this.SymbolType = consumable.SymbolType;

            this.IsEquiped = false;
            this.IsCursed = false;
            this.IsObjective = (metaData.IsObjective && metaData.IsIdentified);
            this.IsUnique = (metaData.IsUnique && metaData.IsIdentified);

            this.Quality = quality;
            this.Class = classs;

            if (!metaData.IsIdentified)
            {
                this.DisplayName = "???";
                this.ShortDescription = "";
                this.LongDescription = "";
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
                    this.DisplayName = consumable.RogueName + " (" + Encoding.UTF8.GetString(new byte[] { 0xE2, 0x88, 0x9E }) + ")";
                }

                else
                    this.DisplayName = consumable.RogueName;

                this.LongDescription = metaData.LongDescription;
                this.ShortDescription = metaData.Description;
            }
        }
        public void UpdateEquipment(Equipment equipment, ScenarioMetaData metaData)
        {
            this.Id = equipment.Id;
            this.RogueName = equipment.RogueName;

            bool shouldShowClass = false;
            bool consumeEnable = false;
            bool enchantEnable = false;
            bool uncurseEnable = false;
            bool throwEnable = false;
            bool isEquiped = false;
            bool isCursed = false;

            string quality = "N/A";
            string classs = "N/A";

            this.IsEquipment = true;
            this.IsConsumable = false;

            shouldShowClass = equipment.Type == EquipmentType.Armor || equipment.Type == EquipmentType.Boots
                || equipment.Type == EquipmentType.Gauntlets || equipment.Type == EquipmentType.Helmet
                || equipment.Type == EquipmentType.OneHandedMeleeWeapon || equipment.Type == EquipmentType.Shield
                || equipment.Type == EquipmentType.RangeWeapon || equipment.Type == EquipmentType.Shoulder
                || equipment.Type == EquipmentType.TwoHandedMeleeWeapon || equipment.Type == EquipmentType.Belt;

            enchantEnable = equipment.Type == EquipmentType.Belt || equipment.Type == EquipmentType.Shoulder || equipment.Type == EquipmentType.Armor || equipment.Type == EquipmentType.OneHandedMeleeWeapon || equipment.Type == EquipmentType.RangeWeapon || equipment.Type == EquipmentType.TwoHandedMeleeWeapon;
            uncurseEnable = equipment.IsCursed;
            isEquiped = equipment.IsEquipped;
            isCursed = equipment.IsCursed && metaData.IsCurseIdentified;
            quality = shouldShowClass ? equipment.Quality.ToString("F2") : "N/A";
            classs = shouldShowClass ? equipment.Class.ToString("F0") : "N/A";

            if (!equipment.IsIdentified)
                classs = "?";

            this.EquipType = equipment.Type;
            this.Quantity = 1;
            this.Weight = equipment.Weight.ToString("F2");
            this.Type = equipment.Type.ToString();
            this.AttackAttributes.Clear();

            if (equipment.IsIdentified)
                this.AttackAttributes.AddRange(equipment.AttackAttributes
                                                        .Where(x => x.Resistance > 0 || x.Attack > 0 || x.Weakness > 0)
                                                        .Select(x => new AttackAttributeViewModel(x)));

            this.UsageDescription = CreateEquipmentUsageDescription(equipment.Type, isEquiped);

            this.EquipEnable = true;
            this.ConsumeEnable = consumeEnable;
            this.IdentifyEnable = !metaData.IsIdentified || (!equipment.IsIdentified && shouldShowClass);
            this.UncurseEnable = uncurseEnable;
            this.DropEnable = true;
            this.SelectEnable = true;
            this.ThrowEnable = throwEnable;
            this.EnchantEnable = enchantEnable;

            // Symbol Details
            this.CharacterSymbol = equipment.CharacterSymbol;
            this.CharacterColor = equipment.CharacterColor;
            this.Icon = equipment.Icon;
            this.SmileyMood = equipment.SmileyMood;
            this.SmileyBodyColor = equipment.SmileyBodyColor;
            this.SmileyLineColor = equipment.SmileyLineColor;
            this.SmileyAuraColor = equipment.SmileyAuraColor;
            this.SymbolType = equipment.SymbolType;
            
            this.IsEquiped = isEquiped;
            this.IsCursed = isCursed;
            this.IsObjective = (metaData.IsObjective && metaData.IsIdentified);
            this.IsUnique = (metaData.IsUnique && metaData.IsIdentified);

            this.Quality = quality;
            this.Class = classs;

            if (!metaData.IsIdentified)
            {
                this.DisplayName = metaData.IsIdentified ? this.RogueName : "???";
                if (shouldShowClass)
                {
                    this.Quality = "?";
                    this.Class = "?";
                }
                this.ShortDescription = "";
                this.LongDescription = "";
            }
            else
            {
                this.DisplayName = equipment.RogueName;
                this.LongDescription = metaData.LongDescription;
                this.ShortDescription = metaData.Description;
            }
        }
        private string CreateEquipmentUsageDescription(EquipmentType type, bool isEquiped)
        {
            switch (type)
            {
                case EquipmentType.None:
                    return "";
                case EquipmentType.Armor:
                    return isEquiped ? "Armor - Click button to the left to un-equip" : "Armor - Click button to the left to equip";
                case EquipmentType.Shoulder:
                    return isEquiped ? "Shoulder - Click button to the left to un-equip" : "Shoulder - Click button to the left to equip";
                case EquipmentType.Belt:
                    return isEquiped ? "Belt - Click button to the left to un-equip" : "Belt - Click button to the left to equip";
                case EquipmentType.Helmet:
                    return isEquiped ? "Helmet - Click button to the left to un-equip" : "Helmet - Click button to the left to equip";
                case EquipmentType.Ring:
                    return isEquiped ? "Ring - Click button to the left to un-equip" : "Ring - Click button to the left to equip";
                case EquipmentType.Amulet:
                    return isEquiped ? "Amulet - Click button to the left to un-equip" : "Amulet - Click button to the left to equip";
                case EquipmentType.Boots:
                    return isEquiped ? "Armor - Click button to the left to un-equip" : "Armor - Click button to the left to equip";
                case EquipmentType.Gauntlets:
                    return isEquiped ? "Gauntlets - Click button to the left to un-equip" : "Gauntlets - Click button to the left to equip";
                case EquipmentType.OneHandedMeleeWeapon:
                    return isEquiped ? "One Handed Weapon - Click button to the left to un-equip" : "One Handed Weapon - Click button to the left to equip";
                case EquipmentType.TwoHandedMeleeWeapon:
                    return isEquiped ? "Two Handed Weapon - Click button to the left to un-equip" : "Two Handed Weapon - Click button to the left to equip";
                case EquipmentType.RangeWeapon:
                    return isEquiped ? "Two Handed Range Weapon - Click button to the left to un-equip" : "Two Handed Range Weapon - Click button to the left to equip";
                case EquipmentType.Shield:
                    return isEquiped ? "Shield - Click button to the left to un-equip" : "Shield - Click button to the left to equip";
                case EquipmentType.Orb:
                    return isEquiped ? "Orb - Click button to the left to un-equip" : "Orb - Click button to the left to equip";
                default:
                    return "";
            }
        }
        private string CreateConsumableUsageDescription(ConsumableSubType subType, bool throwEnable, bool consumeEnable, int quantity, int uses)
        {
            var result = subType.ToString() + " - ";

            if (throwEnable)
                result += "To Throw use \"T\" to target enemy and Click button to the left (IN THROW MODE)";

            if (consumeEnable)
            {
                if (throwEnable)
                    result += "\r\n";

                result += "To Consume Click button on the left (IN CONSUME MODE)";
            }

            if (!consumeEnable && !throwEnable && subType == ConsumableSubType.Ammo)
                result += "To Fire Range Weapon - First equip corresponding range weapon. Use \"T\" to target enemy and \"F\" to Fire";

            return result;
        }
    }
}
