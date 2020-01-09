using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.Views.Controls.Symbol.ViewModel;
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
            this.CategoryLB.ItemsSource = _scenarioResourceService.GetCharacterCategories();
        }
        private void LoadCharacters(string category)
        {
            // Create symbols for the specified category
            this.SymbolLB.ItemsSource = _scenarioResourceService.GetCharacterResourceNames(category).Select(characterName =>
            {
                // Load Image Source
                var imageSource = _scenarioResourceService.GetImageSource(ScenarioImage.CreateCharacterSymbol(characterName,
                                                                                                            characterName,
                                                                                                            category,
                                                                                                            Colors.White.ToString(),
                                                                                                            1.0), 1.0, 1.0, Light.White);

                return SvgSymbolViewModel.CreateCharacterSymbol(imageSource, category, characterName, 1.0);
            });
        }
    }
}
