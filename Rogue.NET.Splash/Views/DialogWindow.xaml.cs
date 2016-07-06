using Rogue.NET.Common.Collections;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rogue.NET.Splash.Views
{
    public partial class DialogWindow : Window
    {
        public DialogWindow()
        {
            InitializeComponent();

            this.DataContextChanged += Dialog_DataContextChanged;
        }

        void Dialog_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var data = e.NewValue as LevelData;
            if (data != null)
                _lb.ItemsSource = data.DialogMessages.
                    Select(m => m.Timestamp.ToString() + "\t" + m.Message).
                    Reverse().
                    Take(20);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
