using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    public partial class CharacterMap : UserControl
    {
        const int _NUMBER = 150;

        public string SelectedCharacter { get; private set; }

        public CharacterMap()
        {
            InitializeComponent();

            LoadCharacters();
        }

        private void LoadCharacters()
        {
            var characterList = new List<string>();
            var startingNumber = (int)this.CharacterSlider.Value;
            for (int i = startingNumber; i < startingNumber + _NUMBER; i++)
                characterList.Add(new string(new char[]{(char)i}));

            this.CharacterListBox.ItemsSource = characterList;
        }

        private void Character_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            this.SelectedCharacter = button.Tag.ToString();
            ((Window)this.Parent).DialogResult = true;
        }

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            if (e.OriginalSource is Button)
                return;

            LoadCharacters();
        }
    }

    public class DoubleToHexConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return (0).ToString("X4");

            var integer = (int)(double)value;
            return integer.ToString("X4");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class DoubleToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return new string(new char[] { (char)0 });

            var integer = (int)(double)value;
            return new string(new char[] { (char)integer });
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
