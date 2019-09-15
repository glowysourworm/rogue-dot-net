using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.ScenarioEditor.Events;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Alteration.Common;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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

namespace Rogue.NET.ScenarioEditor.Views.Assets.SharedControl.AlterationControl.EffectControl
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class TransmuteEffectParameters : UserControl
    {
        [ImportingConstructor]
        public TransmuteEffectParameters(
            IRogueEventAggregator eventAggregator,
            IScenarioCollectionProvider scenarioCollectionProvider)
        {
            InitializeComponent();
            Initialize(scenarioCollectionProvider);

            eventAggregator.GetEvent<ScenarioUpdateEvent>()
                           .Subscribe(provider =>
                           {
                               Initialize(provider);
                           });
        }

        private void Initialize(IScenarioCollectionProvider provider)
        {
            this.EquipmentCB.ItemsSource = provider.Equipment.CreateDefaultView();
            this.ConsumableCB.ItemsSource = provider.Consumables.CreateDefaultView();
            this.EquipmentRequirementsLB.SourceItemsSource = provider.Equipment.CreateDefaultView();
            this.ConsumableRequirementsLB.SourceItemsSource = provider.Consumables.CreateDefaultView();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as TransmuteAlterationEffectTemplateViewModel;
            if (viewModel != null)
            {
                var name = NameGenerator.Get(viewModel.TransmuteItems.Select(x => x.Name), "Transmute Item");

                viewModel.TransmuteItems.Add(new TransmuteAlterationEffectItemTemplateViewModel()
                {
                    Name = name
                });
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as TransmuteAlterationEffectTemplateViewModel;
            if (viewModel != null)
            {
                var item = this.TransmuteItemsListBox.SelectedItem as TransmuteAlterationEffectItemTemplateViewModel;
                if (item != null)
                    viewModel.TransmuteItems.Remove(item);
            }
        }

        private void ConsumableRequirementsLB_RemoveEvent(object sender, object e)
        {
            var viewModel = this.DataContext as TransmuteAlterationEffectTemplateViewModel;
            if (viewModel != null)
            {
                var item = this.TransmuteItemsListBox.SelectedItem as TransmuteAlterationEffectItemTemplateViewModel;
                if (item != null)
                    item.ConsumableRequirements.Remove(e as ConsumableTemplateViewModel);
            }
        }

        private void EquipmentRequirementsLB_RemoveEvent(object sender, object e)
        {
            var viewModel = this.DataContext as TransmuteAlterationEffectTemplateViewModel;
            if (viewModel != null)
            {
                var item = this.TransmuteItemsListBox.SelectedItem as TransmuteAlterationEffectItemTemplateViewModel;
                if (item != null)
                    item.EquipmentRequirements.Remove(e as EquipmentTemplateViewModel);
            }
        }
    }
}
