using Rogue.NET.Common.Events.Scenario;
using Rogue.NET.Model;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.View
{
    [Export]
    public partial class CommandPreferencesView : UserControl
    {
        [ImportingConstructor]
        public CommandPreferencesView()
        {
            InitializeComponent();

            this.DataContext = CommandPreferencesViewModel.GetDefaults();
        }

        public event EventHandler<UserCommandEventArgs> UserCommandRequestEvent;

        public bool IsUserCommandRequired()
        {
            return false;
        }

        //private void Save()
        //{
        //    var preferences = this.DataContext as CommandPreferencesViewModel;
        //    if (preferences != null)
        //        ResourceManager.SaveCommandPreferences(preferences);
        //}
        //private void Reset()
        //{
        //    this.DataContext = ResourceManager.GetCommandPreferences();
        //}

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            //Save();
            // TODO
            //this.DialogResult = true;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
           // this.DialogResult = false;
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            //Reset();
        }
    }
}
