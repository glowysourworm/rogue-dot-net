using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Consumable;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Interface;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.Common.Extension.Prism.EventAggregator;

using System.ComponentModel.Composition;
using System.Windows.Controls;
using Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.CommonControl;
using System.Windows;
using System;
using Rogue.NET.ScenarioEditor.Views.Extension;

namespace Rogue.NET.ScenarioEditor.Views.Assets.ConsumableControl
{
    [Export]
    public partial class ConsumableAlterationControl : UserControl
    {
        public ConsumableAlterationControl()
        {
            InitializeComponent();
        }

        private void OnAlterationEffectChosen(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as ConsumableAlterationTemplateViewModel;
            if (viewModel == null)
                return;

            // Cast to routed event args
            var effect = (e as AlterationEffectChosenRoutedEventArgs).Effect;

            if (effect is IConsumableAlterationEffectTemplateViewModel)
                viewModel.Effect = (effect as IConsumableAlterationEffectTemplateViewModel);

            // TODO:  NOT SURE WHY BINDING ISN'T UPDATING! SHOULD NOT HAVE TO FORCE UPDATE
            this.ConsumableAlterationRegion.SetCurrentValue(DataContextProperty, viewModel.Effect);
        }
    }
}
