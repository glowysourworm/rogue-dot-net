using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content;

namespace Rogue.NET.Scenario.Views
{
    [Export]
    public partial class DungeonEncyclopedia : UserControl
    {
        [ImportingConstructor]
        public DungeonEncyclopedia(RogueEncyclopediaViewModel viewModel)
        {
            this.DataContext = viewModel;

            InitializeComponent();
        }

        private void DungeonEncyclopediaCtrl_Loaded(object sender, RoutedEventArgs e)
        {
            //var potionItem = new Consumable(ConsumableType.OneUse, ConsumableSubType.Potion, "NotNamed", ImageResources.PotionGreen, 3);
            //potionItem.Tag = ConsumableSubType.Potion;
            //potionItem.IsPhysicallyVisible = true;

            //var scrollItem = new Consumable(ConsumableType.OneUse, ConsumableSubType.Scroll, "NotNamed", ImageResources.ScrollFuschia, 3);
            //scrollItem.Tag = ConsumableSubType.Scroll;
            //scrollItem.IsPhysicallyVisible = true;

            //var foodItem = new Consumable(ConsumableType.OneUse, ConsumableSubType.Food, "NotNamed", ImageResources.FoodRed, 3);
            //foodItem.Tag = ConsumableSubType.Food;
            //foodItem.IsPhysicallyVisible = true;

            //var manualItem = new Consumable(ConsumableType.OneUse, ConsumableSubType.Manual, "NotNamed", ImageResources.ManualPurple, 3);
            //manualItem.Tag = ConsumableSubType.Manual;
            //manualItem.IsPhysicallyVisible = true;

            //var wandItem = new Consumable(ConsumableType.OneUse, ConsumableSubType.Wand, "NotNamed", ImageResources.WandRed, 3);
            //wandItem.Tag = ConsumableSubType.Wand;
            //wandItem.IsPhysicallyVisible = true;

            //var ammoItem = new Consumable(ConsumableType.OneUse, ConsumableSubType.Ammo, "NotNamed", ImageResources.RockBlack, 3);
            //ammoItem.Tag = ConsumableSubType.Ammo;
            //ammoItem.IsPhysicallyVisible = true;

            //var miscItem = new Consumable(ConsumableType.OneUse, ConsumableSubType.Misc, "NotNamed", ImageResources.MirrorGreen, 3);
            //miscItem.Tag = ConsumableSubType.Misc;
            //miscItem.IsPhysicallyVisible = true;

            //var oneHandedItem = new Equipment(EquipmentType.OneHandedMeleeWeapon, "NotNamed", ImageResources.SwordBlack, 3);
            //oneHandedItem.Tag = EquipmentType.OneHandedMeleeWeapon;
            //oneHandedItem.IsPhysicallyVisible = true;

            //var twoHandedImageItem = new Equipment(EquipmentType.TwoHandedMeleeWeapon, "NotNamed", ImageResources.SwordOrange, 3);
            //twoHandedImageItem.Tag = EquipmentType.TwoHandedMeleeWeapon;
            //twoHandedImageItem.IsPhysicallyVisible = true;

            //var armorItem = new Equipment(EquipmentType.Armor, "NotNamed", ImageResources.ArmorBlack, 3);
            //armorItem.Tag = EquipmentType.Armor;
            //armorItem.IsPhysicallyVisible = true;

            //var shieldItem = new Equipment(EquipmentType.Shield, "NotNamed", ImageResources.ShieldBlack, 3);
            //shieldItem.Tag = EquipmentType.Shield;
            //shieldItem.IsPhysicallyVisible = true;

            //var helmetItem = new Equipment(EquipmentType.Helmet, "NotNamed", ImageResources.HelmetBlack, 3);
            //helmetItem.Tag = EquipmentType.Helmet;
            //helmetItem.IsPhysicallyVisible = true;

            //var gauntletItem = new Equipment(EquipmentType.Gauntlets, "NotNamed", ImageResources.GauntletsBlack, 3);
            //gauntletItem.Tag = EquipmentType.Gauntlets;
            //gauntletItem.IsPhysicallyVisible = true;

            //var beltItem = new Equipment(EquipmentType.Belt, "NotNamed", ImageResources.BeltBlack, 3);
            //beltItem.Tag = EquipmentType.Belt;
            //beltItem.IsPhysicallyVisible = true;

            //var shoulderItem = new Equipment(EquipmentType.Shoulder, "NotNamed", ImageResources.ShoulderBlack, 3);
            //shoulderItem.Tag = EquipmentType.Shoulder;
            //shoulderItem.IsPhysicallyVisible = true;

            //var bootsItem = new Equipment(EquipmentType.Boots, "NotNamed", ImageResources.BootsBlack, 3);
            //bootsItem.Tag = EquipmentType.Boots;
            //bootsItem.IsPhysicallyVisible = true;

            //var amuletItem = new Equipment(EquipmentType.Amulet, "NotNamed", ImageResources.AmuletBlue, 3);
            //amuletItem.Tag = EquipmentType.Amulet;
            //amuletItem.IsPhysicallyVisible = true;

            //var orbItem = new Equipment(EquipmentType.Orb, "NotNamed", ImageResources.OrbBlue, 3);
            //orbItem.Tag = EquipmentType.Orb;
            //orbItem.IsPhysicallyVisible = true;

            //var ringItem = new Equipment(EquipmentType.Ring, "NotNamed", ImageResources.RingGreen, 3);
            //ringItem.Tag = EquipmentType.Ring;
            //ringItem.IsPhysicallyVisible = true;

            //var normalDoodadItem = new DoodadNormal(DoodadNormalType.SavePoint, "NotNamed", "", true, 3);
            //normalDoodadItem.Tag = DoodadType.Normal;
            //normalDoodadItem.IsPhysicallyVisible = true;

            //var magicDoodadItem = new DoodadMagic("NotNamed", null, null, false, false, ImageResources.WellRed, 3);
            //magicDoodadItem.Tag = DoodadType.Magic;
            //magicDoodadItem.IsPhysicallyVisible = true;

            //var enemyItem = new Enemy("T", "NotNamed", 3);
            //enemyItem.Tag = "Enemy";
            //enemyItem.IsPhysicallyVisible = true;

            //var skillItem = new SkillSet("NotNamed", ImageResources.Skill1, 3);
            //skillItem.Tag = "Skill";
            //skillItem.IsPhysicallyVisible = true;

            //this.TypePanel.ClearElements();

            //this.TypePanel.Children.Add(potionItem);
            //this.TypePanel.Children.Add(scrollItem);
            //this.TypePanel.Children.Add(foodItem);
            //this.TypePanel.Children.Add(wandItem);
            //this.TypePanel.Children.Add(ammoItem);
            //this.TypePanel.Children.Add(manualItem);
            //this.TypePanel.Children.Add(miscItem);
            //this.TypePanel.Children.Add(oneHandedItem);
            //this.TypePanel.Children.Add(twoHandedImageItem);
            //this.TypePanel.Children.Add(armorItem);
            //this.TypePanel.Children.Add(shieldItem);
            //this.TypePanel.Children.Add(helmetItem);
            //this.TypePanel.Children.Add(beltItem);
            //this.TypePanel.Children.Add(shoulderItem);
            //this.TypePanel.Children.Add(gauntletItem);
            //this.TypePanel.Children.Add(bootsItem);
            //this.TypePanel.Children.Add(amuletItem);
            //this.TypePanel.Children.Add(orbItem);
            //this.TypePanel.Children.Add(ringItem);
            //this.TypePanel.Children.Add(normalDoodadItem);
            //this.TypePanel.Children.Add(magicDoodadItem);
            //this.TypePanel.Children.Add(enemyItem);
            //this.TypePanel.Children.Add(skillItem);

            //this.TypePanel.InitializeElements();

            //this.TypePanel.InvalidateVisual();
            //InvalidateVisual();
        }
        private void DungeonEncyclopediaCtrl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // TODO
            //var data = e.NewValue as LevelData;
            //if (data != null)
            //    _encyclopedia = data.Encyclopedia;
        }
        private string GetEntryTypeDescription(string type)
        {
            switch (type)
            {
                case "Food":
                    return "Items that asuage hunger";

                case "Potion":
                    return "Items that you can drink";
                    
                case "Scroll":
                    return "Items that you can read";

                case "Manual":
                    return "Items that are instructive";

                case "Wand":
                    return "Items that channel magic";

                case "Ammo":
                    return "Ammunition for range weapons";

                case "Misc":
                    return "Miscellaneous items";

                case "OneHandedMeleeWeapon":
                    return "Items that you can wield with one hand";

                case "TwoHandedMeleeWeapon":
                    return "Items that require two free hands to wield";

                case "Armor":
                    return "Items that you can wear on your torso";

                case "Shield":
                    return "Items that you can use to protect yourself";

                case "Gauntlets":
                    return "Items that you can wear on your hands";
                    
                case "Belt":
                    return "Items that you can wear on your waist";

                case "Shoulder":
                    return "Items that you can wear on your shoulders";

                case "Boots":
                    return "Items that you can wear on your feet";

                case "Helmet":
                    return "Items that you can wear on your head";

                case "Amulet":
                    return "Items you can wear around your neck";

                case "Orb":
                    return "Items that inhabit your aura";

                case "Ring":
                    return "Items you can wear on your fingers";

                case "Normal":
                    return "Dungeon objects that you can use";

                case "Magic":
                    return "Dungeon objects that have magical properties";

                case "Enemy":
                    return "Enemies encountered in the dungeon";
                    
                case "Skill":
                    return "Skills you can learn along your way";

                case "Objective":
                    return "Objects you must encounter or collect in order to complete your mission";

                default:
                    throw new Exception("Unhandled dungeon meta data type");
            }
        }
        private string GetEntryTypeName(string type)
        {
            switch (type)
            {
                case "Food":
                    return "Food";

                case "Potion":
                    return "Potions";

                case "Scroll":
                    return "Scrolls";

                case "Manual":
                    return "Manuals";

                case "Wand":
                    return "Wands";

                case "Ammo":
                    return "Ammunition";

                case "Misc":
                    return "Miscellaneous";

                case "OneHandedMeleeWeapon":
                    return "One Handed Weapons";

                case "TwoHandedMeleeWeapon":
                    return "Two Handed Weapons";

                case "Armor":
                    return "Armor";

                case "Shield":
                    return "Shields";

                case "Helmet":
                    return "Helmets";

                case "Gauntlets":
                    return "Gauntlets";

                case "Belt":
                    return "Belt";

                case "Shoulder":
                    return "Shoulder";

                case "Boots":
                    return "Boots";

                case "Amulet":
                    return "Amulets";

                case "Orb":
                    return "Orbs";

                case "Ring":
                    return "Rings";

                case "Normal":
                    return "Normal Dungeon Objects";

                case "Magic":
                    return "Magic Dungeon Objects";

                case "Enemy":
                    return "Enemies";

                case "Skill":
                    return "Skills";

                case "Objective":
                    return "Objectives";

                default:
                    throw new Exception("Unhandled dungeon meta data type");
            }
        }
        private void SetSubPanel(string type)
        {
            //var data = this.DataContext as LevelData;
            //if (data == null)
            //    return;

            //this.SubPanel.ClearElements();
            //foreach (KeyValuePair<string, ScenarioMetaData> kvp in data.Encyclopedia)
            //{
            //    kvp.Value.Tag = kvp.Key;

            //    if (kvp.Value.Type == type)
            //        this.SubPanel.Children.Add(kvp.Value);

            //    else if (type == "Objective" && kvp.Value.IsObjective)
            //        this.SubPanel.Children.Add(kvp.Value);
            //}
            //this.SubPanel.InitializeElements();

            //// Attempt to invoke dispatcher to seek nearest after animations initialized
            //// ...Need to figure out how to make "InitializeElements()" complete all the 
            //// required tasks to leave the panel in a truly initialized state on this call
            //// stack.. though may not be possible.
            ////
            //// NOTE*** ApplicationIdle priority required to avoid exceptions with animation clock
            ////         initialization. Should probably be moved inside spin panel initialization
            //this.SubPanel.Visibility = Visibility.Hidden;
            //this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle, 
            //                            new Action(() => {
            //                                this.SubPanel.SeekToNearest();
            //                                this.SubPanel.Visibility = Visibility.Visible;
            //                            }));
        }
        private void SubPanel_ObjectCentered(object sender, EventArgs e)
        {
            // TODO
            //var data = this.DataContext as LevelData;
            //if (data == null)
            //    return;

            //if (data != null)
            //{
            //    var m = data.Encyclopedia[((FrameworkElement)sender).Tag.ToString()];
            //    this.SubPanel.CenterText = m.IsIdentified ? m.RogueName : "???";
            //    this.SubPanel.Foreground = m.IsUnique ? (m.IsObjective ? Brushes.Cyan : Brushes.Goldenrod) : Brushes.White;
            //    if (m.IsObjective)
            //        this.SubPanel.Foreground = Brushes.Cyan;

            //    if (!m.IsIdentified)
            //        this.SubPanel.Foreground = Brushes.White;

            //    this.EntryBorder.DataContext = m;
            //}
        }
        private void TypePanel_ObjectCentered(object sender, EventArgs e)
        {
            var view = sender as ScenarioObject;
            //SetSubPanel(view.Tag.ToString());

            //if (view != null)
            //{
            //    this.EntryDescriptionTB.Text = GetEntryTypeDescription(view.Tag.ToString());
            //    this.EntryTypeTB.Text = GetEntryTypeName(view.Tag.ToString());
            //    this.TypePanel.CenterText = GetEntryTypeName(view.Tag.ToString());

            //    int ct = _encyclopedia.Count(z => z.Value.Type == view.Tag.ToString());
            //    int ctIdentified = _encyclopedia.Count(z => z.Value.Type == view.Tag.ToString() && z.Value.IsIdentified);

            //    this.PercentTB.Text = (ct == 0) ? "0%" : (100*(ctIdentified / (double)ct)).ToString("N0") + "%";
            //}
        }
        private void EntryLeftButton_Click(object sender, RoutedEventArgs e)
        {
            //this.SubPanel.SeekPrevious();
        }

        private void EntryRightButton_Click(object sender, RoutedEventArgs e)
        {
            //this.SubPanel.SeekNext();
        }

        private void TypeLeftButton_Click(object sender, RoutedEventArgs e)
        {
            //this.TypePanel.SeekPrevious();
        }
        private void TypeRightButton_Click(object sender, RoutedEventArgs e)
        {
            //this.TypePanel.SeekNext();
        }
    }
}
