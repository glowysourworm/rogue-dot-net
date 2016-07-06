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

namespace Rogue.NET.Splash.Views
{
    /// <summary>
    /// Interaction logic for PlayerAdvancementPopupControl.xaml
    /// </summary>
    public partial class PlayerAdvancementPopupControl : UserControl
    {
        public PlayerAdvancementPopupControl()
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
