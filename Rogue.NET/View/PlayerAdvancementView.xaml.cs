using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class PlayerAdvancementView : UserControl
    {
        [ImportingConstructor]
        public PlayerAdvancementView()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            SelfDismissEvent(this, new EventArgs());
        }

        public event EventHandler SelfDismissEvent;
    }
}
