using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.Specialized;
using System.ComponentModel;
using Rogue.NET.Common;
using Rogue.NET.Model;

namespace Rogue.NET.Scenario.Views
{
	public partial class EquipmentSelectionCtrl : UserControl
	{
		public EquipmentSelectionCtrl()
		{
			this.InitializeComponent();

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(EquipmentSelectionCtrl_DataContextChanged);
		}

        private void EquipmentSelectionCtrl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var data = e.NewValue as LevelData;
            if (data != null)
            {
                data.Player.EquipmentInventory.CollectionAltered += (obj, ev) =>
                {
                    SetFromDataContext();
                };

                data.Player.MeleeAttackAttributes.CollectionAltered += (obj, ev) =>
                {
                    //var view = CollectionViewSource.GetDefaultView(this.AttackAttributesLB.ItemsSource);
                    //if (view != null)
                    //    view.Refresh();

                    
                };

                SetFromDataContext();
            }
        }
        private void SetFromDataContext()
        {
            var data = this.DataContext as LevelData;

            //Reset pictures to null
            this.HeadImage.Source = null;
            this.BodyImage.Source = null;
            this.FeetImage.Source = null;
            this.AmuletImage.Source = null;
            this.OrbImage.Source = null;
            this.LeftHandImageGlove.Source = null;
            this.LeftHandImageWeapon.Source = null;
            this.RightHandImageGlove.Source = null;
            this.RightHandImageWeapon.Source = null;
            this.LeftRing1.Source = null;
            this.RightRing1.Source = null;
            this.ShoulderImage.Source = null;
            this.BeltImage.Source = null;
            this.EquipmentGadget.Source = null;
            this.EnemyScopeGadget.Source = null;
            this.CompassGadget.Source = null;

            if (data != null)
            {
                foreach (var eq in data.Player.EquipmentInventory)
                {
                    if (!eq.IsEquiped)
                        continue;

                    switch (eq.Type)
                    {
                        case EquipmentType.Amulet:
                            this.AmuletImage.Source = eq.SymbolInfo.SymbolImageSource;
                            break;
                        case EquipmentType.Armor:
                            this.BodyImage.Source = eq.SymbolInfo.SymbolImageSource;
                            break;
                        case EquipmentType.Boots:
                            this.FeetImage.Source = eq.SymbolInfo.SymbolImageSource;
                            break;
                        case EquipmentType.Gauntlets:
                            this.RightHandImageGlove.Source = eq.SymbolInfo.SymbolImageSource;
                            this.LeftHandImageGlove.Source = eq.SymbolInfo.SymbolImageSource;
                            break;
                        case EquipmentType.Helmet:
                            this.HeadImage.Source = eq.SymbolInfo.SymbolImageSource;
                            break;
                        case EquipmentType.OneHandedMeleeWeapon:
                        case EquipmentType.Shield:
                            if (this.RightHandImageWeapon.Source == null)
                                this.RightHandImageWeapon.Source = eq.SymbolInfo.SymbolImageSource;
                            else if (this.LeftHandImageWeapon.Source == null)
                                this.LeftHandImageWeapon.Source = eq.SymbolInfo.SymbolImageSource;
                            break;
                        case EquipmentType.Orb:
                            this.OrbImage.Source = eq.SymbolInfo.SymbolImageSource;
                            break;
                        case EquipmentType.Ring:
                            if (this.LeftRing1.Source == null)
                                this.LeftRing1.Source = eq.SymbolInfo.SymbolImageSource;

                            else if (this.RightRing1.Source == null)
                                this.RightRing1.Source = eq.SymbolInfo.SymbolImageSource;
                            break;
                        case EquipmentType.TwoHandedMeleeWeapon:
                        case EquipmentType.RangeWeapon:
                            this.RightHandImageWeapon.Source = eq.SymbolInfo.SymbolImageSource;
                            this.LeftHandImageWeapon.Source = eq.SymbolInfo.SymbolImageSource;
                            break;
                        case EquipmentType.Shoulder:
                            this.ShoulderImage.Source = eq.SymbolInfo.SymbolImageSource;
                            break;
                        case EquipmentType.Belt:
                            this.BeltImage.Source = eq.SymbolInfo.SymbolImageSource;
                            break;
                        case EquipmentType.CompassGadget:
                            this.CompassGadget.Source = eq.SymbolInfo.SymbolImageSource;
                            break;
                        case EquipmentType.EnemyScopeGadet:
                            this.EnemyScopeGadget.Source = eq.SymbolInfo.SymbolImageSource;
                            break;
                        case EquipmentType.EquipmentGadget:
                            this.EquipmentGadget.Source = eq.SymbolInfo.SymbolImageSource;
                            break;
                    }
                }
            }
        }
    }
}