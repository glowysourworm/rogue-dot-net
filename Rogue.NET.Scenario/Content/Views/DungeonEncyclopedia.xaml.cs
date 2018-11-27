using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Scenario.Content.ViewModel.Content;

namespace Rogue.NET.Scenario.Views
{
    [Export]
    public partial class DungeonEncyclopedia : UserControl
    {
        [ImportingConstructor]
        public DungeonEncyclopedia(RogueEncyclopediaViewModel viewModel)
        {
            this.DataContext = viewModel;

            InitializeComponent();

            this.CategoryLB.SelectionChanged += CategoryLB_SelectionChanged;

            this.Loaded += DungeonEncyclopedia_Loaded;
        }

        private void DungeonEncyclopedia_Loaded(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as RogueEncyclopediaViewModel;
            if (viewModel == null)
                return;

            // Select Initial Category
            var selectedCategory = viewModel.Categories.FirstOrDefault(x => x.IsIdentifiedCategory);
            if (selectedCategory != null)
                this.CategoryLB.SelectedItem = selectedCategory;

            else
                this.CategoryLB.SelectedItem = viewModel.Categories.FirstOrDefault();
        }

        // Select first or default identified item
        private void CategoryLB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedCategory = e.AddedItems[0] as RogueEncyclopediaCategoryViewModel;

                this.CategoryEntryLB.SelectedItem = selectedCategory.Items.FirstOrDefault(x => x.IsIdentified) ??
                                                    selectedCategory.Items.FirstOrDefault();
            }
        }
    }
}
