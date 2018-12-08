using Rogue.NET.Scenario.ViewModel.ItemGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Rogue.NET.Scenario.Content.Views.ItemTemplateSelector
{
    public class RogueItemGridRowDetailsTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var viewModel = item as ItemGridRowViewModel;
            var itemsControlElement = container as DataGridDetailsPresenter;

            if (viewModel.IsEquipment)
                return itemsControlElement.FindResource("RogueItemGridRowEquipmentDetailsTemplate") as DataTemplate;

            else
                return itemsControlElement.FindResource("RogueItemGridRowConsumableDetailsTemplate") as DataTemplate;
        }
    }
}
