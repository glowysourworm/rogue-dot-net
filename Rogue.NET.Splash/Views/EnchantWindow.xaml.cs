using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
using Rogue.NET.Common;
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

namespace Rogue.NET.Splash.Views
{
    public partial class EnchantWindow : Window
    {
        public EnchantWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(EnchantWindow_Loaded);
        }

        [InjectionConstructor]
        public EnchantWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();

            this.ItemGrid.EventAggregator = eventAggregator;

            this.Loaded += EnchantWindow_Loaded;
        }

        private void EnchantWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.ItemGrid.ActionSubmittedEvent -= ItemGrid_ActionSubmittedEvent;
            this.ItemGrid.ActionSubmittedEvent += ItemGrid_ActionSubmittedEvent;
        }

        void ItemGrid_ActionSubmittedEvent(object sender, EventArgs e)
        {
            this.DialogResult = true;
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
