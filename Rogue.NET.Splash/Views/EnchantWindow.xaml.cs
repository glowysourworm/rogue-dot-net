using Prism.Events;
using System;
using System.ComponentModel.Composition;
using System.Windows;

namespace Rogue.NET.Splash.Views
{
    [Export]
    public partial class EnchantWindow : Window
    {
        public EnchantWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(EnchantWindow_Loaded);
        }

        [ImportingConstructor]
        public EnchantWindow(IEventAggregator eventAggregator)
        {
            InitializeComponent();

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
