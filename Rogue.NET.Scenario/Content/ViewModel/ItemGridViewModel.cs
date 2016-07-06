using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common;
using Rogue.NET.Common.Collections;
using Rogue.NET.Model.Scenario;
using Rogue.NET.Scenario.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Rogue.NET.Scenario.ViewModel
{
    public class ItemGridViewModel : FrameworkElement
    {
        public ImageSource ImageSource { get; set; }

        #region Dependency Properties
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof(string), typeof(ItemGridViewModel));
        public static readonly DependencyProperty RogueNameProperty = DependencyProperty.Register("RogueName", typeof(string), typeof(ItemGridViewModel));
        public static readonly DependencyProperty EquipTypeProperty = DependencyProperty.Register("EquipType", typeof(EquipmentType), typeof(ItemGridViewModel));
        public static readonly DependencyProperty ConsumeEnableProperty = DependencyProperty.Register("ConsumeEnable", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty IdentifyEnableProperty = DependencyProperty.Register("IdentifyEnable", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty EquipEnableProperty = DependencyProperty.Register("EquipEnable", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty DropEnableProperty = DependencyProperty.Register("DropEnable", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty UncurseEnableProperty = DependencyProperty.Register("UncurseEnable", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty SelectEnableProperty = DependencyProperty.Register("SelectEnable", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty DetailsEnableProperty = DependencyProperty.Register("DetailsEnable", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty ThrowEnableProperty = DependencyProperty.Register("ThrowEnable", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty EnchantEnableProperty = DependencyProperty.Register("EnchantEnable", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty IsMarkedForTradeProperty = DependencyProperty.Register("IsMarkedForTrade", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty IsEquipedProperty = DependencyProperty.Register("IsEquiped", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty IsCursedProperty = DependencyProperty.Register("IsCursed", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty IsObjectiveProperty = DependencyProperty.Register("IsObjective", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty IsUniqueProperty = DependencyProperty.Register("IsUnique", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty ShowButtonProperty = DependencyProperty.Register("ShowButton", typeof(bool), typeof(ItemGridViewModel));
        public static readonly DependencyProperty QuantityProperty = DependencyProperty.Register("Quantity", typeof(int), typeof(ItemGridViewModel));
        public static readonly DependencyProperty ClassProperty = DependencyProperty.Register("Class", typeof(string), typeof(ItemGridViewModel));
        public static readonly DependencyProperty WeightProperty = DependencyProperty.Register("Weight", typeof(string), typeof(ItemGridViewModel));
        public static readonly DependencyProperty QualityProperty = DependencyProperty.Register("Quality", typeof(string), typeof(ItemGridViewModel));
        public static readonly DependencyProperty TotalWeightProperty = DependencyProperty.Register("TotalWeight", typeof(string), typeof(ItemGridViewModel));
        #endregion

        #region Properties

        // dungeon object id
        public string Id
        {
            get { return (string)GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        public bool ShowButton
        {
            get { return (bool)GetValue(ShowButtonProperty); }
            set { SetValue(ShowButtonProperty, value); }
        }
        public bool ConsumeEnable 
        { 
	        get {return (bool)GetValue(ConsumeEnableProperty);}
	        set {SetValue(ConsumeEnableProperty, value);}
        }
        public bool IdentifyEnable 
        { 
	        get {return (bool)GetValue(IdentifyEnableProperty);}
	        set {SetValue(IdentifyEnableProperty, value);}
        }
        public bool EquipEnable 
        { 
	        get {return (bool)GetValue(EquipEnableProperty);}
	        set {SetValue(EquipEnableProperty, value);}
        }
        public bool DropEnable 
        { 
	        get {return (bool)GetValue(DropEnableProperty);}
	        set {SetValue(DropEnableProperty, value);}
        }
        public bool UncurseEnable 
        { 
	        get {return (bool)GetValue(UncurseEnableProperty);}
	        set {SetValue(UncurseEnableProperty, value);}
        }
        public bool SelectEnable 
        { 
	        get {return (bool)GetValue(SelectEnableProperty);}
	        set {SetValue(SelectEnableProperty, value);}
        }
        public bool DetailsEnable 
        { 
	        get {return (bool)GetValue(DetailsEnableProperty);}
	        set {SetValue(DetailsEnableProperty, value);}
        }
        public bool ThrowEnable 
        { 
	        get {return (bool)GetValue(ThrowEnableProperty);}
	        set {SetValue(ThrowEnableProperty, value);}
        }
        public bool EnchantEnable 
        { 
	        get {return (bool)GetValue(EnchantEnableProperty);}
	        set {SetValue(EnchantEnableProperty, value);}
        }
        public bool IsMarkedForTrade 
        {
	        get {return (bool)GetValue(IsMarkedForTradeProperty);}
	        set {SetValue(IsMarkedForTradeProperty, value);}
        }
        public bool IsEquiped 
        {
	        get {return (bool)GetValue(IsEquipedProperty);}
            set 
            {
                SetValue(IsEquipedProperty, value);
            }
        }
        public bool IsCursed 
        { 
	        get {return (bool)GetValue(IsCursedProperty);}
	        set {SetValue(IsCursedProperty, value);}
        }
        public bool IsObjective 
        { 
	        get {return (bool)GetValue(IsObjectiveProperty);}
	        set {SetValue(IsObjectiveProperty, value);}
        }
        public bool IsUnique 
        { 
	        get {return (bool)GetValue(IsUniqueProperty);}
	        set {SetValue(IsUniqueProperty, value);}
        }
        public int Quantity 
        { 
	        get {return (int)GetValue(QuantityProperty);}
	        set {SetValue(QuantityProperty, value);}
        }
        public int ShopValue { get; set; }
        public int Uses { get; set; }
        public string Type { get; set; }
        public EquipmentType EquipType
        {
            get { return (EquipmentType)GetValue(EquipTypeProperty); }
            set { SetValue(EquipTypeProperty, value); }
        }
        public string Class 
        { 
	        get {return (string)GetValue(ClassProperty);}
            set 
            {
                SetValue(ClassProperty, value); 
            }
        }
        public string Weight 
        { 
	        get {return (string)GetValue(WeightProperty);}
	        set {SetValue(WeightProperty, value);}
        }
        public string Quality 
        { 
	        get {return (string)GetValue(QualityProperty);}
	        set {SetValue(QualityProperty, value);}
        }
        public string RogueName
        {
            get { return (string)GetValue(RogueNameProperty); }
            set { SetValue(RogueNameProperty, value); }
        }
        public string RogueSavedName { get; set; }
        #endregion

        /// <summary>
        /// consumable inventory may be null
        /// </summary>
        public ItemGridViewModel(Item item, SerializableDictionary<string, ScenarioMetaData> encyclopedia, SerializableObservableCollection<Consumable> consumableInventory)
        {
            UpdateItem(item, encyclopedia, consumableInventory);
        }

        public void UpdateItem(Item item, SerializableDictionary<string, ScenarioMetaData> encyclopedia, IEnumerable<Consumable> consumableInventory)
        {
            var metaData = encyclopedia[item.RogueName];

            this.Id = item.Id;
            this.RogueSavedName = item.RogueName;

            bool isEquipment = item is Equipment;
            bool isConsumable = item is Consumable;
            bool shouldShowClass = false;
            bool consumeEnable = false;
            bool enchantEnable = false;
            bool uncurseEnable = false;
            bool throwEnable = false;
            bool isEquiped = false;
            bool isCursed = false;

            string quality = "N/A";
            string classs = "N/A";

            if (isEquipment)
            {
                Equipment equipment = item as Equipment;
                shouldShowClass = equipment.Type == EquipmentType.Armor || equipment.Type == EquipmentType.Boots
                    || equipment.Type == EquipmentType.Gauntlets || equipment.Type == EquipmentType.Helmet
                    || equipment.Type == EquipmentType.OneHandedMeleeWeapon || equipment.Type == EquipmentType.Shield
                    || equipment.Type == EquipmentType.RangeWeapon || equipment.Type == EquipmentType.Shoulder
                    || equipment.Type == EquipmentType.TwoHandedMeleeWeapon || equipment.Type == EquipmentType.Belt;

                enchantEnable = equipment.Type == EquipmentType.Belt || equipment.Type == EquipmentType.Shoulder || equipment.Type == EquipmentType.Armor || equipment.Type == EquipmentType.OneHandedMeleeWeapon || equipment.Type == EquipmentType.RangeWeapon || equipment.Type == EquipmentType.TwoHandedMeleeWeapon;
                uncurseEnable = equipment.IsCursed;
                isEquiped = equipment.IsEquiped;
                isCursed = equipment.IsCursed && metaData.IsCurseIdentified;
                quality = shouldShowClass ? equipment.Quality.ToString("F2") : "N/A";
                classs = shouldShowClass ? equipment.Class.ToString("F0") : "N/A";

                if (!equipment.IsIdentified)
                    classs = "?";

                this.EquipType = equipment.Type;
                this.Quantity = 1;
                this.Weight = equipment.Weight.ToString("F2");
                this.Type = equipment.Type.ToString();
            }
            if (isConsumable)
            {
                Consumable consumable = item as Consumable;
                throwEnable = consumable.HasProjectileSpell && consumable.SubType != ConsumableSubType.Ammo;
                consumeEnable = consumable.HasSpell && consumable.SubType != ConsumableSubType.Ammo;

                if (consumableInventory != null)
                {
                    this.Quantity = consumableInventory.Count(z => z.RogueName == consumable.RogueName);
                    this.Weight = (this.Quantity * consumable.Weight).ToString("F2");
                }
                else
                {
                    this.Quantity = 1;
                    this.Weight = consumable.Weight.ToString("F2");
                }

                this.Type = consumable.SubType.ToString();
                this.Uses = consumable.Uses;
            }

            //this.Rogue2DisplayName = obj.Rogue2Name;
            this.EquipEnable = isEquipment;
            this.ConsumeEnable = consumeEnable;
            this.IdentifyEnable = !encyclopedia[item.RogueName].IsIdentified || (!item.IsIdentified && isEquipment && shouldShowClass);
            this.UncurseEnable = uncurseEnable;
            this.DropEnable = true;
            this.SelectEnable = true;
            this.ThrowEnable = throwEnable;
            this.EnchantEnable = enchantEnable;
            
            this.IsEquiped = isEquiped;
            this.IsCursed = isCursed;
            this.IsObjective = (metaData.IsObjective && metaData.IsIdentified);
            this.IsUnique = (metaData.IsUnique && metaData.IsIdentified);

            this.Quality = quality;
            this.Class = classs;
            this.IsMarkedForTrade = item.IsMarkedForTrade;

            this.ShopValue = item.ShopValue;

            if (!metaData.IsIdentified)
            {
                this.RogueName = "???";
                if (shouldShowClass)
                {
                    this.Quality = "?";
                    this.Class = "?";
                }
            }
            else
            {
                if (isConsumable && (item as Consumable).Type == ConsumableType.MultipleUses && consumableInventory != null)
                {
                    IEnumerable<Consumable> consumables = consumableInventory.Where(z => z.Name == item.RogueName).Cast<Consumable>();
                    this.RogueName = item.RogueName + " (" + consumables.Sum(z => z.Uses).ToString("N0") + ")";
                }

                else if (isConsumable && (item as Consumable).Type == ConsumableType.UnlimitedUses && consumableInventory != null)
                {
                    //Infinity symbol
                    this.RogueName = item.RogueName + " (" + Encoding.UTF8.GetString(new byte[] { 0xE2, 0x88, 0x9E }) + ")";
                }

                else
                    this.RogueName = item.RogueName;
            }

            // item display image
            this.ImageSource = item.SymbolInfo.SymbolImageSource;
        }
    }
}
