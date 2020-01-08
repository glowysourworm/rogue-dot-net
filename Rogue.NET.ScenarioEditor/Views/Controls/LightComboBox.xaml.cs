using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;

using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    public partial class LightComboBox : UserControl
    {
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(LightTemplateViewModel), typeof(LightComboBox), new PropertyMetadata(OnSelectedValueChanged));

        public LightTemplateViewModel SelectedValue
        {
            get { return (LightTemplateViewModel)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }

        public LightComboBox()
        {
            InitializeComponent();

            this.TheCB.ItemsSource = LightOperations.GetSupportedColors().Actualize();
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LightComboBox;
            var viewModel = e.NewValue as LightTemplateViewModel;

            // Select item programmatically because there's no reference list to match in the converter
            //
            if (control != null &&
                viewModel != null)
            {
                var itemsSource = control.TheCB.ItemsSource as IEnumerable<Color>;

                if (itemsSource != null)
                {
                    var item = itemsSource.FirstOrDefault(color => color.R == viewModel.Red &&
                                                                   color.G == viewModel.Green &&
                                                                   color.B == viewModel.Blue);

                    if (item != null)
                        control.TheCB.SelectedItem = item;
                }
            }
        }


        private void TheCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (Color)this.TheCB.SelectedItem;

            // Update item programmatically 
            //
            if (selectedItem != null)
                this.SelectedValue = new LightTemplateViewModel()
                {
                    Red = selectedItem.R,
                    Green = selectedItem.G,
                    Blue = selectedItem.B,
                    Intensity = 1.0
                };
        }
    }
}
