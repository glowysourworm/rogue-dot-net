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

namespace Rogue.NET.ScenarioEditor.Views.Controls.Symbol
{
    [Export]
    public partial class CharacterMap : UserControl
    {
        readonly IScenarioResourceService _scenarioResourceService;
        public string SelectedCharacter { get; private set; }
        public string SelectedCategory { get; private set; }
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
                var symbol = e.AddedItems.Count > 0 ? (SvgSymbolViewModel)e.AddedItems[0] : null;

                if (symbol != null && !loading)
                {
                    this.SelectedCategory = symbol.Category;
                    this.SelectedCharacter = symbol.Symbol;

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

                if (viewModel == null)
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
            var assembly = typeof(IRogueEventAggregator).Assembly;

            // For now, just parse the names like the folder structure (figure out more robust way later)
            //
            var prefix = "Rogue.NET.Common.Resource.Svg.Scenario.Character.";

            // Get resources from the character folder -> Parse out category names
            var categories = assembly.GetManifestResourceNames()
                                        .Where(x => x.Contains("Svg.Scenario.Character"))
                                        .Select(x => x.Replace(prefix, ""))
                                        .Select(x =>
                                        {
                                            var pieces = x.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                                            if (pieces.Length != 3)
                                                throw new Exception("Resource file-name format differs from expected");

                                            // [Category].[FileName].svg

                                            return pieces[0];
                                        })
                                        .Distinct()
                                        .Actualize();

            this.CategoryLB.ItemsSource = categories;
        }
        private void LoadCharacters(string category)
        {
            var assembly = typeof(IRogueEventAggregator).Assembly;

            // For now, just parse the names like the folder structure (figure out more robust way later)
            //
            var prefix = "Rogue.NET.Common.Resource.Svg.Scenario.Character.";

            // Get resources from the character folder -> Parse out category names
            var items = assembly.GetManifestResourceNames()
                                .Where(x => x.Contains("Svg.Scenario.Character"))
                                .Where(x => x.Contains("." + category + "."))
                                .Select(x => x.Replace(prefix, ""))
                                .Select(x =>
                                {
                                    var pieces = x.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                                    if (pieces.Length != 3)
                                        throw new Exception("Resource file-name format differs from expected");

                                    // [Category].[FileName].svg

                                    // Load Image Source
                                    var imageSource = _scenarioResourceService.GetImageSource(new ScenarioImage(pieces[1], 
                                                                                                                pieces[1], 
                                                                                                                pieces[0], 
                                                                                                                Colors.White.ToString()), 1.0);

                                    return new SvgSymbolViewModel(imageSource, pieces[0], pieces[1]);
                                })
                                .Actualize();

            this.SymbolLB.ItemsSource = items;
        }
    }
}
