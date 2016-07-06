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

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    public partial class ColorComboBox : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", 
            typeof(string), 
            typeof(ColorComboBox),
            new PropertyMetadata(OnValueChanged));

        public string Value
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public ColorComboBox()
        {
            InitializeComponent();

            /* ItemsSource */
            var properties = typeof(Colors).GetProperties();
            var colors = new List<ColorItem>();
            foreach (var property in properties)
                colors.Add(new ColorItem()
                {
                    Color = new SolidColorBrush((Color)property.GetValue(typeof(Colors))),
                    Name = property.Name
                });

            this.TheComboBox.ItemsSource = colors;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var cb = o as ColorComboBox;
            if (cb.TheComboBox != null)
            {
                List<ColorItem> list = new List<ColorItem>(cb.TheComboBox.ItemsSource.Cast<ColorItem>());
                var converter = new ColorItemConverter();
                var item = (ColorItem)converter.Convert(e.NewValue, null, null, null);
                if (list.Any(z => z.Name == item.Name))
                    cb.TheComboBox.SelectedItem = list.First(z => z.Name == item.Name);
            }
        }
        private void TheComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var converter = new ColorItemConverter();
                this.Value = (string)converter.ConvertBack(e.AddedItems[0], null, null, null);
            }
        }
    }

    public class ColorItem
    {
        public string Name { get; set; }
        public Brush Color { get; set; }
    }
    public class ColorItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return new ColorItem() { Name = "White", Color = Brushes.White };

            var color = (Color)ColorConverter.ConvertFromString(value.ToString());
            var brush = new SolidColorBrush(color);

            var colors = typeof(Colors).GetProperties().Select(z => new { propertyInfo = z, colorValue = z.GetValue(typeof(Colors)) });
            var matchedName = colors.First(z => Color.AreClose((Color)z.colorValue, color)).propertyInfo.Name;
            return new ColorItem() { Color = new SolidColorBrush(color), Name = matchedName };
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var colorItem = value as ColorItem;
            if (colorItem != null)
                return colorItem.Color.ToString();

            return Colors.White.ToString();
        }
    }
}
