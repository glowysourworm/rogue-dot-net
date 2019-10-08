using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Processing.Symbol.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.Views.Controls.Symbol.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace Rogue.NET.ScenarioEditor.Views.Controls.Symbol
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class SymbolChooser : UserControl
    {
        readonly IScenarioResourceService _scenarioResourceService;
        readonly ISvgCache _svgCache;

        bool _loading = false;
        bool _loaded = false;

        [ImportingConstructor]
        public SymbolChooser(IScenarioResourceService scenarioResourceService, ISvgCache svgCache)
        {
            _scenarioResourceService = scenarioResourceService;
            _svgCache = svgCache;

            InitializeComponent();

            bool loading = false;
            bool loaded = false;

            this.BaseSymbolLB.SelectionChanged += (sender, e) =>
            {
                var viewModel = this.DataContext as SymbolDetailsTemplateViewModel;
                var baseSymbol = e.AddedItems.Count > 0 ? (SvgSymbolViewModel)e.AddedItems[0] : null;

                if (baseSymbol != null &&
                    viewModel != null)
                    LoadColoredSymbols(baseSymbol, viewModel.SymbolUseColorMask);
            };

            this.ColoredSymbolLB.SelectionChanged += (sender, e) =>
            {
                var viewModel = this.DataContext as SymbolDetailsTemplateViewModel;
                var item = e.AddedItems.Count > 0 ? (SvgSymbolViewModel)e.AddedItems[0] : null;

                if (item != null && 
                    viewModel != null &&
                   !_loading)
                {
                    viewModel.Symbol = item.Symbol;
                    viewModel.SymbolHue = item.Hue;
                    viewModel.SymbolSaturation = item.Saturation;
                    viewModel.SymbolLightness = item.Lightness;
                    //viewModel.SymbolUseColorMask = item.UseColorMask;
                }
            };

            this.DataContextChanged += (sender, e) =>
            {
                var viewModel = e.NewValue as SymbolDetailsTemplateViewModel;

                if (viewModel != null)
                {
                    if (!_loaded)
                        Initialize();

                    this.BaseSymbolLB.SelectedItem = this.BaseSymbolLB
                                                         .Items
                                                         .Cast<SvgSymbolViewModel>()
                                                         .FirstOrDefault(x => x.SymbolType == viewModel.SymbolType &&
                                                                              x.Symbol == viewModel.Symbol);

                    this.ColoredSymbolLB.SelectedItem = this.ColoredSymbolLB
                                                            .Items
                                                            .Cast<SvgSymbolViewModel>()
                                                            .FirstOrDefault(x => x.SymbolType == viewModel.SymbolType &&
                                                                                 x.Symbol == viewModel.Symbol &&
                                                                                 x.Hue == viewModel.SymbolHue &&
                                                                                 x.Saturation == viewModel.SymbolSaturation &&
                                                                                 x.Lightness == viewModel.SymbolLightness &&
                                                                                 x.UseColorMask == viewModel.SymbolUseColorMask);

                    // Scroll into view
                    if (this.BaseSymbolLB.SelectedIndex >= 0 &&
                        this.BaseSymbolLB.SelectedIndex < this.BaseSymbolLB.Items.Count)
                    {
                        var index = this.BaseSymbolLB.SelectedIndex;
                        var item = this.BaseSymbolLB.Items.GetItemAt(index);
                        this.BaseSymbolLB.ScrollIntoView(item);
                    }
                }
            };
        }

        private void Initialize()
        {
            var viewModel = this.DataContext as SymbolDetailsTemplateViewModel;

            if (viewModel == null)
                return;

            _loading = true;

            LoadBaseSymbols();

            var baseSymbols = this.BaseSymbolLB.Items.Cast<SvgSymbolViewModel>();
            var selectedSymbol = baseSymbols.FirstOrDefault(x => x.Symbol == viewModel.Symbol);

            if (selectedSymbol != null)
            {
                LoadColoredSymbols(selectedSymbol, viewModel.SymbolUseColorMask);

                this.BaseSymbolLB.SelectedItem = selectedSymbol;

                var items = this.ColoredSymbolLB.Items.Cast<SvgSymbolViewModel>();

                this.ColoredSymbolLB.SelectedItem = items.FirstOrDefault(x => x.Symbol == viewModel.Symbol &&
                                                                              x.Hue == viewModel.SymbolHue &&
                                                                              x.Saturation == viewModel.SymbolSaturation &&
                                                                              x.Lightness == viewModel.SymbolLightness);
            }

            _loaded = true;
            _loading = false;
        }

        private void LoadBaseSymbols()
        {
            this.BaseSymbolLB.ItemsSource = _svgCache.GetResourceNames(SymbolType.Symbol).Select(symbol =>
            {
                var imageSource = _scenarioResourceService.GetImageSource(new ScenarioImage(symbol, symbol, 0, 0, 0, false), 2.0);

                // Input color mask indicator only. The HSL parameters are based at zero because they're effects
                return new SvgSymbolViewModel(imageSource, symbol, 0, 0, 0, false);
            });
        }
        private void LoadColoredSymbols(SvgSymbolViewModel viewModel, bool useColorMask)
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
                                                                                            viewModel.Lightness,
                                                                                            useColorMask), 2.0);

                result.Add(new SvgSymbolViewModel(imageSource, viewModel.Symbol, hue, viewModel.Saturation, viewModel.Lightness, viewModel.UseColorMask));

                cursor += increment;
            }

            // Light / Dark
            var lightImageSource = _scenarioResourceService.GetImageSource(new ScenarioImage(viewModel.Symbol,
                                                                                             viewModel.Symbol,
                                                                                             0.0,
                                                                                             0.0,
                                                                                             1.0,
                                                                                             useColorMask), 2.0);

            var darkImageSource = _scenarioResourceService.GetImageSource(new ScenarioImage(viewModel.Symbol,
                                                                                            viewModel.Symbol,
                                                                                            0.0,
                                                                                            -1,
                                                                                            0.0,
                                                                                            useColorMask), 2.0);

            result.Add(new SvgSymbolViewModel(lightImageSource, viewModel.Symbol, 0.0, 0.0, 1.0, useColorMask));
            result.Add(new SvgSymbolViewModel(darkImageSource, viewModel.Symbol, 0.0, -1, 0.0, useColorMask));


            this.ColoredSymbolLB.ItemsSource = result;
        }

        private void SymbolResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as SymbolDetailsTemplateViewModel;
            var svgViewModel = this.BaseSymbolLB.SelectedItem as SvgSymbolViewModel;

            if (viewModel != null &&
                svgViewModel != null)
            {
                viewModel.SymbolUseColorMask = false;
                viewModel.SymbolHue = 0;
                viewModel.SymbolLightness = 0;
                viewModel.SymbolSaturation = 0;
                viewModel.SymbolScale = 1;

                LoadColoredSymbols(svgViewModel, false);
            }
        }

        private void ColorMaskCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            var svgViewModel = this.BaseSymbolLB.SelectedItem as SvgSymbolViewModel;

            if (svgViewModel != null)
                LoadColoredSymbols(svgViewModel, true);
        }

        private void ColorMaskCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            var svgViewModel = this.BaseSymbolLB.SelectedItem as SvgSymbolViewModel;

            if (svgViewModel != null)
                LoadColoredSymbols(svgViewModel, false);
        }
    }
}
