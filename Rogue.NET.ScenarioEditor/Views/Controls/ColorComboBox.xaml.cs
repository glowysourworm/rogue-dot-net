using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Processing.Service.Interface;
using System.Windows.Controls;

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
    }
}
