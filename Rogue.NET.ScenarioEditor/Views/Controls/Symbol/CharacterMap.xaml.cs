using System;
using System.Linq;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Microsoft.Practices.ServiceLocation;
using Rogue.NET.ScenarioEditor.Views.Controls.Symbol.ViewModel;
using Rogue.NET.Core.Processing.Symbol.Interface;

namespace Rogue.NET.ScenarioEditor.Views.Controls.Symbol
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class CharacterMap : UserControl
    {
        readonly IScenarioResourceService _scenarioResourceService;
        readonly ISvgCache _svgCache;
        public CharacterMap()
        {
            _scenarioResourceService = ServiceLocator.Current.GetInstance<IScenarioResourceService>();
            _svgCache = ServiceLocator.Current.GetInstance<ISvgCache>();
            
            InitializeComponent();

            bool loading = false;

            this.CategoryLB.SelectionChanged += (sender, e) =>
            {
                var category = e.AddedItems.Count > 0 ? (string)e.AddedItems[0] : null;
                var viewModel = this.DataContext as SymbolDetailsTemplateViewModel;

                if (category != null &&
                    viewModel != null)
                {
                    viewModel.CharacterSymbolCategory = category;

                    LoadCharacters(category);
                }
            };

            this.SymbolLB.SelectionChanged += (sender, e) =>
            {
                var symbol = e.AddedItems.Count > 0 ? (SvgSymbolViewModel)e.AddedItems[0] : null;
                var viewModel = this.DataContext as SymbolDetailsTemplateViewModel;

                if (symbol != null && 
                    viewModel != null &&
                   !loading)
                {
                    viewModel.CharacterSymbolCategory = symbol.Category;
                    viewModel.CharacterSymbol = symbol.Character;

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
                    string.IsNullOrEmpty(viewModel.CharacterSymbolCategory))
                    this.CategoryLB.Items.MoveCurrentToFirst();

                else
                {
                    LoadCharacters(viewModel.CharacterSymbolCategory);

                    this.CategoryLB.SelectedItem = viewModel.CharacterSymbolCategory ?? this.CategoryLB.Items[0];

                    var items = this.SymbolLB.Items.Cast<SvgSymbolViewModel>();

                    this.SymbolLB.SelectedItem = items.FirstOrDefault(x => x.Symbol == viewModel.CharacterSymbol);
                }

                loading = false;
            };
        }

        private void LoadCategories()
        {
            this.CategoryLB.ItemsSource = _svgCache.GetCharacterCategories();
        }
        private void LoadCharacters(string category)
        {
            // Create symbols for the specified category
            this.SymbolLB.ItemsSource = _svgCache.GetCharacterResourceNames(category).Select(characterName =>
            {
                // Load Image Source
                var imageSource = _scenarioResourceService.GetImageSource(new ScenarioImage(characterName,
                                                                                            characterName,
                                                                                            category,
                                                                                            Colors.White.ToString(),
                                                                                            1.0), 1.0);

                return new SvgSymbolViewModel(imageSource, category, characterName, 1.0);
            });
        }
    }
}
