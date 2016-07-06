using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Rogue.NET.Common;
using Rogue.NET.Model;
using Rogue.NET.Model.Scenario;
using Rogue.NET.Scenario.Content.Views.Gadgets;
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

namespace Rogue.NET.Scenario.Views.Gadgets
{
    /// <summary>
    /// Interaction logic for GadgetContainer.xaml
    /// </summary>
    public partial class GadgetContainer : UserControl
    {
        Equipment _currentGadget;
        CompassGadget _compass;
        EnemyScopeGadget _enemyScope;
        EquipmentGadget _equipmentGadget;

        public GadgetContainer()
        {
            InitializeComponent();

            _compass = new CompassGadget();
            _enemyScope = new EnemyScopeGadget();
            _equipmentGadget = new EquipmentGadget();

            this.DataContextChanged += (obj, e) =>
            {
                if (e.OldValue == null)
                    CycleGadgets(false);
            };
        }

        public void InitializeEvents(IEventAggregator eventAggregator)
        {
            _compass.InitializeEvents(eventAggregator);
            _enemyScope.InitializeEvents(eventAggregator);
            _equipmentGadget.InitializeEvents(eventAggregator);
        }

        private void CycleGadgets(bool backward)
        {
            var model = this.DataContext as LevelData;
            if (model != null)
            {
                var gadgets = model.Player.EquipmentInventory.Where(z => z.IsEquiped && z.IsGadget);
                if (_currentGadget == null)
                    _currentGadget = gadgets.FirstOrDefault();

                var index = gadgets.ToList().IndexOf(_currentGadget);
                if (index == gadgets.Count() - 1 && !backward)
                    index = 0;

                else if (index == 0 && backward)
                    index = gadgets.Count() - 1;

                else
                    index = backward ? index - 1 : index + 1;

                _currentGadget = gadgets.ElementAtOrDefault(index);
                ApplyGadgetData();
            }
        }
        private void ApplyGadgetData()
        {
            this.ActiveGadgetContainer.Children.Clear();
            if (_currentGadget == null)
            {
                this.ActiveGadget.Source = null;
                this.ActiveGadgetName.Text = "";
                this.RightButton.Visibility = System.Windows.Visibility.Hidden;
                this.ActiveGadgetBorder.Visibility = System.Windows.Visibility.Hidden;
                return;
            }

            this.ActiveGadgetName.Text = _currentGadget.RogueName;
            this.ActiveGadget.Source = _currentGadget.SymbolInfo.SymbolImageSource;
            this.RightButton.Visibility = System.Windows.Visibility.Visible;
            this.ActiveGadgetBorder.Visibility = System.Windows.Visibility.Visible;
            switch (_currentGadget.Type)
            {
                case EquipmentType.CompassGadget:
                    this.ActiveGadgetContainer.Children.Add(_compass);
                    break;
                case EquipmentType.EnemyScopeGadet:
                    this.ActiveGadgetContainer.Children.Add(_enemyScope);
                    break;
                case EquipmentType.EquipmentGadget:
                    this.ActiveGadgetContainer.Children.Add(_equipmentGadget);
                    break;
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            CycleGadgets(false);
        }
    }
}
