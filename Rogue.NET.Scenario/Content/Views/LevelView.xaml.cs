using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Rogue.NET.Common;
using Rogue.NET.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.Scenario.Views
{
    /// <summary>
    /// Interaction logic for LevelView.xaml
    /// </summary>
    public partial class LevelView : UserControl
    {
        public LevelView(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.Loaded += (obj, e) =>
            {
                this.TheLevelCanvas.InitializeEvents(eventAggregator);
                this.SubPanel.InitializeEvents(eventAggregator);
            };

            this.DataContextChanged += LevelView_DataContextChanged;
        }

        private void SetGadgetVisibility(LevelData data)
        {
            // look for gadgets
            foreach (var equip in data.Player.EquipmentInventory)
            {
                switch (equip.Type)
                {
                    case EquipmentType.CompassGadget:
                        //if (equip.IsEquiped)
                        //    this.CompassCtrl.Visibility = System.Windows.Visibility.Visible;
                        //else
                        //    this.CompassCtrl.Visibility = System.Windows.Visibility.Collapsed;
                        //break;
                    case EquipmentType.EnemyScopeGadet:
                        if (equip.IsEquiped)
                        {
                        }
                        else
                        {
                        }
                        break;
                    case EquipmentType.EquipmentGadget:
                        break;
                }
            }
        }

        private void LevelView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var data = e.NewValue as LevelData;
            if (data != null)
            {
                data.Player.EquipmentInventory.CollectionAltered -= EquipmentInventory_CollectionAltered;
                data.Player.EquipmentInventory.CollectionAltered += EquipmentInventory_CollectionAltered;

                SetGadgetVisibility(data);
            }
        }

        private void EquipmentInventory_CollectionAltered(object sender, Common.Collections.CollectionAlteredEventArgs e)
        {
            var data = this.DataContext as LevelData;
            if (data != null)
                SetGadgetVisibility(data);
        }
   }
}
