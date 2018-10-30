using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.ScenarioConfiguration.Content;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls
{
    public partial class ListBuilder : UserControl
    {
        public ListBuilder()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in this.SourceLB.SelectedItems)
            {
                // Create new instance
                var copy = item.Copy();
                (this.DestinationLB.ItemsSource as IList).Add(copy);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = this.DestinationLB.SelectedItems.Count - 1; i >= 0; i--)
                (this.DestinationLB.ItemsSource as IList).Remove(this.DestinationLB.SelectedItems[i]);
        }
    }

    public class ListBuilderTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;
            if (item is ProbabilityConsumableTemplate)
                return element.FindResource("ProbabilityItemTemplate") as DataTemplate;
            else if (item is ProbabilityEquipmentTemplate)
                return element.FindResource("ProbabilityEquipmentTemplate") as DataTemplate;
            else
                return element.FindResource("ItemTemplate") as DataTemplate;
        }
    }
}
