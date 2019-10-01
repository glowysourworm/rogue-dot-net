using System;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls.TemplateSelector
{
    public class ListBuilderTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            var resourceName = item.GetType().Name;
            var result = element.TryFindResource(resourceName);

            if (result == null)
                throw new Exception("Couldn't Find Resource " + resourceName);

            return result as DataTemplate;
        }
    }
}
