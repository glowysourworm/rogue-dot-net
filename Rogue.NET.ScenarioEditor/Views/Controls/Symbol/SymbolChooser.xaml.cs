using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.Views.Controls.Symbol.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls.Symbol
{
    [Export]
    public partial class SymbolChooser : UserControl
    {
        readonly IScenarioResourceService _scenarioResourceService;

        [ImportingConstructor]
        public SymbolChooser(IScenarioResourceService scenarioResourceService)
        {
            _scenarioResourceService = scenarioResourceService;

            InitializeComponent();

            bool loading = false;

            this.BaseSymbolLB.SelectionChanged += (sender, e) =>
            {
                var baseSymbol = e.AddedItems.Count > 0 ? (SvgSymbolViewModel)e.AddedItems[0] : null;

                if (baseSymbol != null)
                    LoadColoredSymbols(baseSymbol);
            };

            this.ColoredSymbolLB.SelectionChanged += (sender, e) =>
            {
                var viewModel = this.DataContext as SymbolDetailsTemplateViewModel;
                var item = e.AddedItems.Count > 0 ? (SvgSymbolViewModel)e.AddedItems[0] : null;

                if (item != null && 
                    viewModel != null &&
                   !loading)
                {
                    viewModel.Symbol = item.Symbol;
                    viewModel.SymbolHue = item.Hue;
                    viewModel.SymbolSaturation = item.Saturation;
                    viewModel.SymbolLightness = item.Lightness;
                }
            };

            this.Loaded += (sender, e) =>
            {
                loading = true;

                LoadBaseSymbols();

                var viewModel = this.DataContext as SymbolDetailsTemplateViewModel;

                if (viewModel == null)
                    this.BaseSymbolLB.Items.MoveCurrentToFirst();

                else
                {
                    var baseSymbols = this.BaseSymbolLB.Items.Cast<SvgSymbolViewModel>();
                    var selectedSymbol = baseSymbols.FirstOrDefault(x => x.Symbol == viewModel.Symbol);

                    if (selectedSymbol != null)
                    {
                        LoadColoredSymbols(selectedSymbol);

                        this.BaseSymbolLB.SelectedItem = selectedSymbol;

                        var items = this.ColoredSymbolLB.Items.Cast<SvgSymbolViewModel>();

                        this.ColoredSymbolLB.SelectedItem = items.FirstOrDefault(x => x.Symbol == viewModel.Symbol &&
                                                                                      x.Hue == viewModel.SymbolHue &&
                                                                                      x.Saturation == viewModel.SymbolSaturation &&
                                                                                      x.Lightness == viewModel.SymbolLightness);
                    }
                }

                loading = false;
            };
        }

        private void LoadBaseSymbols()
        {
            var assembly = typeof(IRogueEventAggregator).Assembly;

            // For now, just parse the names like the folder structure (figure out more robust way later)
            //
            var prefix = "Rogue.NET.Common.Resource.Svg.Scenario.Symbol.";

            // Get resources from the character folder -> Parse out category names
            var symbols = assembly.GetManifestResourceNames()
                                        .Where(x => x.Contains(prefix))
                                        .Select(x => x.Replace(prefix, ""))
                                        .Select(x =>
                                        {
                                            var pieces = x.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

                                            if (pieces.Length != 2)
                                                throw new Exception("Resource file-name format differs from expected");

                                            // [FileName].svg

                                            // Load Image Source
                                            var hue = 0;
                                            var saturation = 1;
                                            var lightness = 1;

                                            var imageSource = _scenarioResourceService.GetImageSource(new ScenarioImage(pieces[0],
                                                                                                                        pieces[0],
                                                                                                                        hue,
                                                                                                                        saturation,
                                                                                                                        lightness), 2.0);

                                            return new SvgSymbolViewModel(imageSource, pieces[0], 0, 1, 1);
                                        })
                                        .Distinct()
                                        .Actualize();

            this.BaseSymbolLB.ItemsSource = symbols;
        }
        private void LoadColoredSymbols(SvgSymbolViewModel viewModel)
        {
            var result = new List<SvgSymbolViewModel>();
            var increment = Math.PI / 4.0D;
            var cursor = 0.0D;

            // Hue Shifts
            while (cursor <= (Math.PI * 2))
            {
                var hue = (viewModel.Hue + cursor);
                var imageSource = _scenarioResourceService.GetImageSource(new ScenarioImage(viewModel.Symbol,
                                                                                            viewModel.Symbol,
                                                                                            hue,
                                                                                            viewModel.Saturation,
                                                                                            viewModel.Lightness), 2.0);

                result.Add(new SvgSymbolViewModel(imageSource, viewModel.Symbol, hue, viewModel.Saturation, viewModel.Lightness));

                cursor += increment;
            }

            //// Light / Dark
            //var lightImageSource = _scenarioResourceService.GetImageSource(new ScenarioImage(viewModel.Symbol,
            //                                                                                 viewModel.Symbol,
            //                                                                                 0.0,
            //                                                                                 0.0,
            //                                                                                 1.0), 2.0);

            //var darkImageSource = _scenarioResourceService.GetImageSource(new ScenarioImage(viewModel.Symbol,
            //                                                                                viewModel.Symbol,
            //                                                                                0.0,
            //                                                                                0.0,
            //                                                                                0.0), 2.0);

            //result.Add(new SvgSymbolViewModel(lightImageSource, viewModel.Symbol, 0.0, 0.0, 1.0));
            //result.Add(new SvgSymbolViewModel(darkImageSource, viewModel.Symbol, 0.0, 0.0, 0.0));


            this.ColoredSymbolLB.ItemsSource = result;
        }
    }
}
