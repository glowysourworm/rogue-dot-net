using Rogue.NET.Model;
using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Rogue.NET.View
{
    [Export]
    public partial class HelpView : UserControl
    {
        public HelpView()
        {
            InitializeComponent();

            this.DataContext = CommandPreferencesViewModel.GetDefaults();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
            // this.DialogResult = true;
        }
    }

    public class KeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
