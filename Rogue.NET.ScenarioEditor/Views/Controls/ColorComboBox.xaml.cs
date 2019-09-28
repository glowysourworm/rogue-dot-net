using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Processing.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    public partial class ColorComboBox : ComboBox
    {
        public ColorComboBox()
        {
            SetResourceReference(StyleProperty, typeof(ComboBox));

            var resourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();

            InitializeComponent();

            this.ItemsSource = resourceService.GetColors();
        }

        public void SetColors(IEnumerable<Color> colors)
        {
            this.ItemsSource = colors.Select(color => new ColorViewModel()
            {
                Brush = new SolidColorBrush(color),
                Color = color,
                ColorString = color.ToString(),
                Name = color.ToString()
            });
        }
    }
}
