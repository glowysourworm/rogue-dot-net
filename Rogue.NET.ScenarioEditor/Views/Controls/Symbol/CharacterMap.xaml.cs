using Microsoft.Practices.ServiceLocation;

using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Views.Controls.Symbol
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class CharacterMap : UserControl
    {
        readonly IScenarioResourceService _scenarioResourceService;

        public CharacterMap()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();

            InitializeComponent();

            bool loading = false;

            this.CategoryLB.SelectionChanged += (sender, e) =>
            {
                var category = e.AddedItems.Count > 0 ? (string)e.AddedItems[0] : null;

                if (category != null)
                    LoadCharacters(category);
            };

            this.SymbolLB.SelectionChanged += (sender, e) =>
            {
                var symbol = e.AddedItems.Count > 0 ? (SymbolDetailsTemplateViewModel)e.AddedItems[0] : null;
                var viewModel = this.DataContext as SymbolDetailsTemplateViewModel;

                if (symbol != null &&
                    viewModel != null &&
                   !loading)
                {
                    viewModel.SymbolPath = symbol.SymbolPath;

                    // Force Dialog Result to be set here
                    var window = Window.GetWindow(this);
                    if (window != null)
                        window.DialogResult = true;
                }
            };

            this.Loaded += (sender, e) =>
            {
                loading = true;

                LoadCategories();

                var viewModel = this.DataContext as SymbolDetailsTemplateViewModel;

                if (viewModel == null ||
                    string.IsNullOrEmpty(viewModel.SymbolPath))
                    this.CategoryLB.Items.MoveCurrentToFirst();

                else
                {
                    var pathParts = viewModel.SymbolPath.Split(new char[] { '.' });

                    var category = (string)this.CategoryLB.Items[0];

                    if (pathParts.Length == 2)
                        category = pathParts[0];                       

                    LoadCharacters(category);

                    this.CategoryLB.SelectedItem = category;

                    var items = this.SymbolLB.Items.Cast<SymbolDetailsTemplateViewModel>();

                    this.SymbolLB.SelectedItem = items.FirstOrDefault(x => x.SymbolPath == viewModel.SymbolPath);
                }

                loading = false;
            };
        }

        private void LoadCategories()
        {
            this.CategoryLB.ItemsSource = _scenarioResourceService.GetCharacterCategories();
        }
        private void LoadCharacters(string category)
        {
            // Create symbols for the specified category
            this.SymbolLB.ItemsSource = _scenarioResourceService.GetCharacterResourceNames(category).Select(characterName =>
            {
                return new SymbolDetailsTemplateViewModel()
                {
                    SymbolType = SymbolType.Character,
                    SymbolPath = string.Join(".", category, characterName),
                    BackgroundColor = Colors.Transparent.ToString(),
                    SymbolClampColor = Colors.White.ToString(),
                    SymbolEffectType = CharacterSymbolEffectType.ColorClamp,
                    SymbolSize = CharacterSymbolSize.Large
                };
            });
        }
    }
}
