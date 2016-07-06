using Rogue.NET.Model;
using System;
using System.Collections;
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
                var copy = ResourceManager.CreateDeepCopy(item);
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
