using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Abstract;

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
                var baseSymbol = e.AddedItems.Count > 0 ? (SymbolDetailsTemplateViewModel)e.AddedItems[0] : null;

                if (baseSymbol != null &&
                    viewModel != null)
                    LoadColoredSymbols(baseSymbol, viewModel.SymbolEffectType == CharacterSymbolEffectType.HslShiftColorMask);
            };

            this.ColoredSymbolLB.SelectionChanged += (sender, e) =>
            {
                var viewModel = this.DataContext as SymbolDetailsTemplateViewModel;
                var item = e.AddedItems.Count > 0 ? (SymbolDetailsTemplateViewModel)e.AddedItems[0] : null;

                if (item != null &&
                    viewModel != null &&
                   !_loading)
                {
                    // Copy public properties over
                    item.MapOnto(viewModel);
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
            if (viewModel.SymbolType == SymbolType.Smiley ||
                viewModel.SymbolType == SymbolType.Character)
                return;

            _loading = true;

            LoadBaseSymbols(viewModel.SymbolType);

            var baseSymbols = this.BaseSymbolLB.Items.Cast<SymbolDetailsTemplateViewModel>();
            var selectedSymbol = baseSymbols.FirstOrDefault(x => x.SymbolPath == viewModel.SymbolPath);

            if (selectedSymbol != null)
                LoadColoredSymbols(selectedSymbol, viewModel.SymbolEffectType == CharacterSymbolEffectType.HslShiftColorMask);

            _loading = false;
        }

        private void BringSelectionIntoView(SymbolDetailsTemplateViewModel viewModel)
        {
            this.BaseSymbolLB.SelectedItem = this.BaseSymbolLB
                                                 .Items
                                                 .Cast<SymbolDetailsTemplateViewModel>()
                                                 .FirstOrDefault(x => x.SymbolType == viewModel.SymbolType &&
                                                                      x.SymbolPath == viewModel.SymbolPath);

            this.ColoredSymbolLB.SelectedItem = this.ColoredSymbolLB
                                                    .Items
                                                    .Cast<SymbolDetailsTemplateViewModel>()
                                                    .FirstOrDefault(x => x.SymbolType == viewModel.SymbolType &&
                                                                         x.SymbolPath == viewModel.SymbolPath &&
                                                                         x.SymbolHue == viewModel.SymbolHue &&
                                                                         x.SymbolSaturation == viewModel.SymbolSaturation &&
                                                                         x.SymbolLightness == viewModel.SymbolLightness &&
                                                                         x.SymbolEffectType == viewModel.SymbolEffectType);

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
                return new SymbolDetailsTemplateViewModel()
                {
                    SymbolPath = symbol,
                    SymbolType = type
                };
            });
        }
        private void LoadColoredSymbols(SymbolDetailsTemplateViewModel viewModel, bool useColorMask)
        {
            var result = new List<SymbolDetailsTemplateViewModel>();
            var increment = Math.PI / 4.0D;
            var cursor = 0.0D;

            // Hue Shifts
            while (cursor <= (Math.PI * 2))
            {
                var hue = (viewModel.SymbolHue + cursor);

                switch (viewModel.SymbolType)
                {
                    default:
                    case SymbolType.Smiley:
                    case SymbolType.Character:
                        throw new Exception("Unsupported symbol type SymbolChooser");

                    case SymbolType.Game:
                    case SymbolType.Symbol:
                    case SymbolType.OrientedSymbol:
                    case SymbolType.Terrain:
                        {
                            // Copy the view model properties
                            var copy = viewModel.DeepCopy();

                            if (!useColorMask)
                                copy.SymbolEffectType = CharacterSymbolEffectType.HslShift;
                            else
                                copy.SymbolEffectType = CharacterSymbolEffectType.HslShiftColorMask;

                            copy.SymbolHue = hue;

                            // Add to result
                            result.Add(copy);
                        }
                        break;
                }


                cursor += increment;
            }

            switch (viewModel.SymbolType)
            {
                default:
                case SymbolType.Smiley:
                case SymbolType.Character:
                    throw new Exception("Unsupported symbol type SymbolChooser");
                case SymbolType.Game:
                case SymbolType.Symbol:
                case SymbolType.OrientedSymbol:
                case SymbolType.Terrain:
                    {
                        // Light / Dark
                        var lightImage = viewModel.DeepCopy();
                        var darkImage = viewModel.DeepCopy();

                        if (!useColorMask)
                        {
                            lightImage.SymbolEffectType = CharacterSymbolEffectType.HslShift;
                            darkImage.SymbolEffectType = CharacterSymbolEffectType.HslShift;
                        }
                        else
                        {
                            lightImage.SymbolEffectType = CharacterSymbolEffectType.HslShiftColorMask;
                            darkImage.SymbolEffectType = CharacterSymbolEffectType.HslShiftColorMask;
                        }

                        result.Add(lightImage);
                        result.Add(darkImage);
                    }
                    break;
            }

            this.ColoredSymbolLB.ItemsSource = result;
        }

        private void SymbolResetButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.DataContext as SymbolDetailsTemplateViewModel;
            var originalViewModel = this.BaseSymbolLB.SelectedItem as SymbolDetailsTemplateViewModel;

            if (viewModel != null &&
                originalViewModel != null)
            {
                // Map public properties onto the view model
                originalViewModel.MapOnto(viewModel);

                // Reset symbol previews
                LoadColoredSymbols(originalViewModel, false);
            }
        }

        private void ColorMaskCheckBox_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.BaseSymbolLB.SelectedItem as SymbolDetailsTemplateViewModel;

            if (viewModel != null)
                LoadColoredSymbols(viewModel, true);
        }

        private void ColorMaskCheckBox_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = this.BaseSymbolLB.SelectedItem as SymbolDetailsTemplateViewModel;

            if (viewModel != null)
                LoadColoredSymbols(viewModel, false);
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
