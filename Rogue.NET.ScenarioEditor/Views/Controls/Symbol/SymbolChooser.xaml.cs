using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;
using Rogue.NET.ScenarioEditor.Views.Controls.Symbol.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace Rogue.NET.ScenarioEditor.Views.Controls.Symbol
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public partial class SymbolChooser : UserControl
    {
        readonly IScenarioResourceService _scenarioResourceService;

        bool _loading = false;

        [ImportingConstructor]
        public SymbolChooser(IScenarioResourceService scenarioResourceService)
        {
            _scenarioResourceService = scenarioResourceService;

            InitializeComponent();

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
                var oldViewModel = e.OldValue as SymbolDetailsTemplateViewModel;
                var newViewModel = e.NewValue as SymbolDetailsTemplateViewModel;

                // Unhook old property change event
                if (oldViewModel != null)
                    oldViewModel.PropertyChanged -= OnViewModelPropertyChanged;

                if (newViewModel != null)
                {
                    newViewModel.PropertyChanged += OnViewModelPropertyChanged;

                    Initialize(newViewModel);
                    BringSelectionIntoView(newViewModel);
                }
            };
        }

        private void Initialize(SymbolDetailsTemplateViewModel viewModel)
        {
            if (viewModel.SymbolType != SymbolType.Symbol &&
                viewModel.SymbolType != SymbolType.OrientedSymbol)
                return;

            _loading = true;

            LoadBaseSymbols(viewModel.SymbolType);

            var baseSymbols = this.BaseSymbolLB.Items.Cast<SvgSymbolViewModel>();
            var selectedSymbol = baseSymbols.FirstOrDefault(x => x.Symbol == viewModel.Symbol);

            if (selectedSymbol != null)
                LoadColoredSymbols(selectedSymbol, viewModel.SymbolUseColorMask);

            _loading = false;
        }

        private void BringSelectionIntoView(SymbolDetailsTemplateViewModel viewModel)
        {
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

        private void LoadBaseSymbols(SymbolType type)
        {
            this.BaseSymbolLB.ItemsSource = _scenarioResourceService.GetResourceNames(type).Select(symbol =>
            {
                if (type == SymbolType.Symbol)
                {
                    var imageSource = _scenarioResourceService.GetImageSource(ScenarioImage.CreateSymbol(symbol, symbol, 0, 0, 0, false), 2.0);

                    // Input color mask indicator only. The HSL parameters are based at zero because they're effects
                    return SvgSymbolViewModel.CreateSymbol(imageSource, symbol, 0, 0, 0, false);
                }
                else // Oriented Symbol
                {
                    var imageSource = _scenarioResourceService.GetImageSource(ScenarioImage.CreateOrientedSymbol(symbol, symbol, 0, 0, 0, false), 2.0);

                    // Input color mask indicator only. The HSL parameters are based at zero because they're effects
                    return SvgSymbolViewModel.CreateOrientedSymbol(imageSource, symbol, 0, 0, 0, false);
                }
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

                switch (viewModel.SymbolType)
                {
                    default:
                    case SymbolType.Game:
                    case SymbolType.Smiley:
                    case SymbolType.Character:
                        throw new Exception("Unsupported symbol type SymbolChooser");
                    case SymbolType.Symbol:
                        {
                            // Create symbol type image
                            var scenarioImage = ScenarioImage.CreateSymbol(viewModel.Symbol, viewModel.Symbol, hue,
                                                                           viewModel.Saturation, viewModel.Lightness, useColorMask);

                            // Fetch image source
                            var imageSource = _scenarioResourceService.GetImageSource(scenarioImage, 2.0);

                            // Add to result
                            result.Add(SvgSymbolViewModel.CreateSymbol(imageSource, viewModel.Symbol, hue, viewModel.Saturation, viewModel.Lightness, viewModel.UseColorMask));
                        }
                        break;
                    case SymbolType.OrientedSymbol:
                        {
                            // Create symbol type image
                            var scenarioImage = ScenarioImage.CreateOrientedSymbol(viewModel.Symbol, viewModel.Symbol, hue,
                                                                                   viewModel.Saturation, viewModel.Lightness, useColorMask);

                            // Fetch image source
                            var imageSource = _scenarioResourceService.GetImageSource(scenarioImage, 2.0);

                            // Add to result
                            result.Add(SvgSymbolViewModel.CreateOrientedSymbol(imageSource, viewModel.Symbol, hue, viewModel.Saturation, viewModel.Lightness, viewModel.UseColorMask));
                        }
                        break;
                }


                cursor += increment;
            }

            switch (viewModel.SymbolType)
            {
                default:
                case SymbolType.Game:
                case SymbolType.Smiley:
                case SymbolType.Character:
                    throw new Exception("Unsupported symbol type SymbolChooser");
                case SymbolType.Symbol:
                    {
                        // Light / Dark
                        var lightImage = ScenarioImage.CreateSymbol(viewModel.Symbol, viewModel.Symbol, 0.0, 0.0, 1.0, useColorMask);
                        var darkImage = ScenarioImage.CreateSymbol(viewModel.Symbol, viewModel.Symbol, 0.0, -1, 0.0, useColorMask);

                        var lightImageSource = _scenarioResourceService.GetImageSource(lightImage, 2.0);
                        var darkImageSource = _scenarioResourceService.GetImageSource(darkImage, 2.0);

                        result.Add(SvgSymbolViewModel.CreateSymbol(lightImageSource, viewModel.Symbol, 0.0, 0.0, 1.0, useColorMask));
                        result.Add(SvgSymbolViewModel.CreateSymbol(darkImageSource, viewModel.Symbol, 0.0, -1, 0.0, useColorMask));
                    }
                    break;
                case SymbolType.OrientedSymbol:
                    {
                        // Light / Dark
                        var lightImage = ScenarioImage.CreateOrientedSymbol(viewModel.Symbol, viewModel.Symbol, 0.0, 0.0, 1.0, useColorMask);
                        var darkImage = ScenarioImage.CreateOrientedSymbol(viewModel.Symbol, viewModel.Symbol, 0.0, -1, 0.0, useColorMask);

                        var lightImageSource = _scenarioResourceService.GetImageSource(lightImage, 2.0);
                        var darkImageSource = _scenarioResourceService.GetImageSource(darkImage, 2.0);

                        result.Add(SvgSymbolViewModel.CreateOrientedSymbol(lightImageSource, viewModel.Symbol, 0.0, 0.0, 1.0, useColorMask));
                        result.Add(SvgSymbolViewModel.CreateOrientedSymbol(darkImageSource, viewModel.Symbol, 0.0, -1, 0.0, useColorMask));
                    }
                    break;
            }

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

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SymbolType")
            {
                Initialize((sender as SymbolDetailsTemplateViewModel));
            }
        }
    }
}
