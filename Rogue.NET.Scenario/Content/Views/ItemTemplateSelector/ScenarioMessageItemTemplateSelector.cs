using Rogue.NET.Scenario.Content.ViewModel.Message;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rogue.NET.Scenario.Content.Views.ItemTemplateSelector
{
    public class ScenarioMessageItemTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var itemsControlElement = container as FrameworkElement;

            if (item is ScenarioAlterationMessageViewModel)
                return itemsControlElement.FindResource("AlterationMessageDataTemplate") as DataTemplate;

            else if (item is ScenarioEnemyAlterationMessageViewModel)
                return itemsControlElement.FindResource("EnemyAlterationMessageDataTemplate") as DataTemplate;

            else if (item is ScenarioMeleeMessageViewModel)
                return itemsControlElement.FindResource("MeleeMessageDataTemplate") as DataTemplate;

            else if (item is ScenarioNormalMessageViewModel)
                return itemsControlElement.FindResource("NormalMessageDataTemplate") as DataTemplate;

            else if (item is ScenarioPlayerAdvancementMessageViewModel)
                return itemsControlElement.FindResource("PlayerAdvancementMessageDataTemplate") as DataTemplate;

            else if (item is ScenarioSkillAdvancementMessageViewModel)
                return itemsControlElement.FindResource("SkillAdvancementMessageDataTemplate") as DataTemplate;

            else
                throw new Exception("Unhandled Scenario message data type");
        }
    }
}
