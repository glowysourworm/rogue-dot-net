using System;
using System.Collections.Generic;
using System.Linq;
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
using Rogue.NET.Common.Collections;
using Rogue.NET.Common;
using Rogue.NET.Model.Scenario;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;

namespace Rogue.NET.Scenario.Views
{
    public partial class InventorySelectionCtrl : UserControl
    {
        IEventAggregator _eventAggregator;

        public InventorySelectionCtrl()
        {
            InitializeComponent();

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(InventorySelectionCtrl_DataContextChanged);
        }

        public InventorySelectionCtrl(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            InitializeComponent();

            this.DataContextChanged += new DependencyPropertyChangedEventHandler(InventorySelectionCtrl_DataContextChanged);
        }

        private void InventorySelectionCtrl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //ScenarioViewModel view = e.NewValue as ScenarioViewModel;
            //if (view != null)
            //    this.ItemGrid.SetItemsSource(view.Level.Player.Inventory);
        }
    }
}
