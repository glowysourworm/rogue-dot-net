using Rogue.NET.Scenario.Content.ViewModel.Content;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Rogue.NET.Scenario.Content.Views.ItemTemplateSelector
{
    public class AlterationGridDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var itemsControlElement = container as DataGridDetailsPresenter;

            return itemsControlElement.FindResource("AlterationDataTemplate") as DataTemplate;
        }
    }
}
