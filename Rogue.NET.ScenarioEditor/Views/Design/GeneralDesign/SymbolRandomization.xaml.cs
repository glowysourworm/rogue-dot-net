using Rogue.NET.Common.Extension;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.Views.Controls.Symbol;
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

namespace Rogue.NET.ScenarioEditor.Views.Design.GeneralDesign
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public partial class SymbolRandomization : UserControl
    {
        public SymbolRandomization()
        {
            InitializeComponent();

            // Add / Remove Symbol Category
            this.AddSymbolCategoryButton.Click += (sender, e) =>
            {
                var viewModel = this.DataContext as ScenarioConfigurationContainerViewModel;
                var symbolCategoryName = this.SymbolCategoryTB.Text?.Trim();

                // Prevent duplicate categories
                if (viewModel != null &&
                    !string.IsNullOrEmpty(symbolCategoryName) &&
                    viewModel.SymbolPool.None(x => x.SymbolPoolCategory == symbolCategoryName))
                {
                    // Add symbol pool category
                    viewModel.SymbolPool.Add(new SymbolPoolItemTemplateViewModel()
                    {
                        SymbolPoolCategory = symbolCategoryName
                    });
                }
            };
            this.RemoveSymbolCategoryButton.Click += (sender, e) =>
            {
                var viewModel = this.DataContext as ScenarioConfigurationContainerViewModel;
                var symbolPoolItem = this.SymbolCategoryLB.SelectedItem as SymbolPoolItemTemplateViewModel;

                if (viewModel != null &&
                    symbolPoolItem != null)
                    viewModel.SymbolPool.Remove(symbolPoolItem);
            };

            // Edit
            this.EditSymbolButton.Click += (sender, e) =>
            {
                var selectedSymbol = this.SymbolPoolLB.SelectedItem as SymbolDetailsTemplateViewModel;

                if (selectedSymbol != null)
                    EditSymbol(selectedSymbol);
            };

            // Add / Remove
            this.AddSymbolButton.Click += (sender, e) =>
            {
                var selectedPool = this.SymbolCategoryLB.SelectedItem as SymbolPoolItemTemplateViewModel;

                if (selectedPool != null)
                    selectedPool.Symbols.Add(new SymbolDetailsTemplateViewModel());
            };
            this.RemoveSymbolButton.Click += (sender, e) =>
            {
                var selectedPool = this.SymbolCategoryLB.SelectedItem as SymbolPoolItemTemplateViewModel;
                var selectedSymbol = this.SymbolPoolLB.SelectedItem as SymbolDetailsTemplateViewModel;

                if (selectedPool != null &&
                    selectedSymbol != null)
                    selectedPool.Symbols.Remove(selectedSymbol);
            };

            // Selection 
            this.SymbolCategoryLB.SelectionChanged += (sender, e) =>
            {
                var selectedItem = e.AddedItems.Count > 0 ? e.AddedItems[0] as SymbolPoolItemTemplateViewModel : null;
                var viewModel = this.DataContext as ScenarioConfigurationContainerViewModel;

                if (selectedItem != null &&
                    viewModel != null)
                {
                    this.SymbolPoolLB.ItemsSource = selectedItem.Symbols;
                    this.AssetSymbolLB.ItemsSource = viewModel.ConsumableTemplates.Cast<DungeonObjectTemplateViewModel>()
                                                              .Union(viewModel.EquipmentTemplates)
                                                              .Union(viewModel.DoodadTemplates)
                                                              .Where(x => x.SymbolDetails.Randomize)
                                                              .Where(x => x.SymbolDetails.SymbolPoolCategory == selectedItem.SymbolPoolCategory)
                                                              .Select(x => x.SymbolDetails);
                }
            };
        }

        private void EditSymbol(SymbolDetailsTemplateViewModel selectedSymbol)
        {
            DialogWindowFactory.ShowSymbolEditor(selectedSymbol);
        }
    }
}
