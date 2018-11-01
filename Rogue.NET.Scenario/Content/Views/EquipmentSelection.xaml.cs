using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using Rogue.NET.Core.Graveyard;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Service.Interface;

namespace Rogue.NET.Scenario.Views
{
    [Export]
	public partial class EquipmentSelectionCtrl : UserControl
	{
        readonly IScenarioResourceService _resourceService;

        [ImportingConstructor]
		public EquipmentSelectionCtrl(IScenarioResourceService resourceService)
		{
            _resourceService = resourceService;

			this.InitializeComponent();

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(EquipmentSelectionCtrl_DataContextChanged);
		}

        private void EquipmentSelectionCtrl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //var data = e.NewValue as LevelData;
            //if (data != null)
            //{
            //    data.Player.Equipment += (obj, ev) =>
            //    {
            //        SetFromDataContext();
            //    };

            //    SetFromDataContext();
            //}
        }
        private void SetFromDataContext()
        {
            // TODO
            //var data = this.DataContext as LevelData;

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

            // TODO
            //if (data != null)
            //{
            //    foreach (var eq in data.Player.Equipment.Values)
            //    {
            //        if (!eq.IsEquipped)
            //            continue;

            //        switch (eq.Type)
            //        {
            //            case EquipmentType.Amulet:
            //                this.AmuletImage.Source = _resourceService.GetImageSource(eq);
            //                break;
            //            case EquipmentType.Armor:
            //                this.BodyImage.Source = _resourceService.GetImageSource(eq);
            //                break;
            //            case EquipmentType.Boots:
            //                this.FeetImage.Source = _resourceService.GetImageSource(eq);
            //                break;
            //            case EquipmentType.Gauntlets:
            //                this.RightHandImageGlove.Source = _resourceService.GetImageSource(eq);
            //                this.LeftHandImageGlove.Source = _resourceService.GetImageSource(eq);
            //                break;
            //            case EquipmentType.Helmet:
            //                this.HeadImage.Source = _resourceService.GetImageSource(eq);
            //                break;
            //            case EquipmentType.OneHandedMeleeWeapon:
            //            case EquipmentType.Shield:
            //                if (this.RightHandImageWeapon.Source == null)
            //                    this.RightHandImageWeapon.Source = _resourceService.GetImageSource(eq);
            //                else if (this.LeftHandImageWeapon.Source == null)
            //                    this.LeftHandImageWeapon.Source = _resourceService.GetImageSource(eq);
            //                break;
            //            case EquipmentType.Orb:
            //                this.OrbImage.Source = _resourceService.GetImageSource(eq);
            //                break;
            //            case EquipmentType.Ring:
            //                if (this.LeftRing1.Source == null)
            //                    this.LeftRing1.Source = _resourceService.GetImageSource(eq);

            //                else if (this.RightRing1.Source == null)
            //                    this.RightRing1.Source = _resourceService.GetImageSource(eq);
            //                break;
            //            case EquipmentType.TwoHandedMeleeWeapon:
            //            case EquipmentType.RangeWeapon:
            //                this.RightHandImageWeapon.Source = _resourceService.GetImageSource(eq);
            //                this.LeftHandImageWeapon.Source = _resourceService.GetImageSource(eq);
            //                break;
            //            case EquipmentType.Shoulder:
            //                this.ShoulderImage.Source = _resourceService.GetImageSource(eq);
            //                break;
            //            case EquipmentType.Belt:
            //                this.BeltImage.Source = _resourceService.GetImageSource(eq);
            //                break;
            //        }
            //    }
            //}
        }
    }
}