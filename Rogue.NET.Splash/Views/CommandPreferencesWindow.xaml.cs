using Rogue.NET.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Splash.Views
{
    public partial class CommandPreferencesWindow : Window
    {
        public CommandPreferencesWindow()
        {
            InitializeComponent();

            this.DataContext = ResourceManager.GetCommandPreferences();
        }

        private void Save()
        {
            var preferences = this.DataContext as CommandPreferences;
            if (preferences != null)
                ResourceManager.SaveCommandPreferences(preferences);
        }
        private void Reset()
        {
            this.DataContext = ResourceManager.GetCommandPreferences();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
            this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }
    }
}
