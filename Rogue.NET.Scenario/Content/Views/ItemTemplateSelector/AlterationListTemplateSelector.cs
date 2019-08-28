using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Common;
using Rogue.NET.Scenario.Content.ViewModel.Content.Alteration.Effect;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Rogue.NET.Scenario.Content.Views.ItemTemplateSelector
{
    public class AlterationListTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var presenter = container as ContentPresenter;

            var viewModel = item as AlterationListViewModel;

            if (viewModel.Effect is AttackAttributeAuraAlterationEffectViewModel)
                return presenter.FindResource("AttackAttributeAuraAlterationEffectDataTemplate") as DataTemplate;

            else if (viewModel.Effect is AttackAttributePassiveAlterationEffectViewModel)
                return presenter.FindResource("AttackAttributePassiveAlterationEffectDataTemplate") as DataTemplate;

            else if (viewModel.Effect is AttackAttributeTemporaryAlterationEffectViewModel)
                return presenter.FindResource("AttackAttributeTemporaryAlterationEffectDataTemplate") as DataTemplate;

            else if (viewModel.Effect is AuraAlterationEffectViewModel)
                return presenter.FindResource("AuraAlterationEffectDataTemplate") as DataTemplate;

            else if (viewModel.Effect is PassiveAlterationEffectViewModel)
                return presenter.FindResource("PassiveAlterationEffectDataTemplate") as DataTemplate;

            else if (viewModel.Effect is TemporaryAlterationEffectViewModel)
                return presenter.FindResource("TemporaryAlterationEffectDataTemplate") as DataTemplate;

            else
                throw new Exception("Unknown Data Tempalte (AlterationListTemplateSelector)");
        }
    }
}
