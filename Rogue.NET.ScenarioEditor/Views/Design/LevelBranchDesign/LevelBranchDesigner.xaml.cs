﻿using Rogue.NET.Common.Extension;
using Rogue.NET.Common.ViewModel;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.ScenarioEditor.Service.Interface;
using Rogue.NET.ScenarioEditor.Utility;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Design;
using Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration.Layout;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Rogue.NET.ScenarioEditor.Views.Design.LevelBranchDesign
{
    [Export]
    public partial class LevelBranchDesigner : UserControl
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly IPreviewRenderingService _previewRenderingService;
        readonly ILayoutGenerator _layoutGenerator;

        // TODO: Move to injection
        readonly ScenarioConfigurationMapper _scenarioConfigurationMapper;

        // Layout Preview Symbols
        readonly SymbolDetailsTemplate FullNoTerrainSupportLayerSymbol;
        readonly SymbolDetailsTemplate ConnectionLayerSymbol;
        readonly SymbolDetailsTemplate WalkableLayerSymbol;
        readonly SymbolDetailsTemplate PlacementLayerSymbol;
        readonly SymbolDetailsTemplate RoomLayerSymbol;
        readonly SymbolDetailsTemplate CorridorLayerSymbol;
        readonly SymbolDetailsTemplate WallLayerSymbol;
        readonly SymbolDetailsTemplate TerrainSupportLayerSymbol;

        // LAST PREVIEW
        LayoutTemplate _previewLayoutTemplate;
        LayoutGrid _previewLayoutGrid;
        bool _initialized = false;

        // TODO: REMOVE THIS
        public class TerrainSelectionViewModel : NotifyViewModel
        {
            string _name;
            bool _isSelected;

            public string Name
            {
                get { return _name; }
                set { this.RaiseAndSetIfChanged(ref _name, value); }
            }

            public bool IsSelected
            {
                get { return _isSelected; }
                set { this.RaiseAndSetIfChanged(ref _isSelected, value); }
            }
        }

        [ImportingConstructor]
        public LevelBranchDesigner(IRandomSequenceGenerator randomSequenceGenerator,
               IPreviewRenderingService previewRenderingService,
               ILayoutGenerator layoutGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
            _previewRenderingService = previewRenderingService;
            _layoutGenerator = layoutGenerator;

            _scenarioConfigurationMapper = new ScenarioConfigurationMapper();

            InitializeComponent();

            // TODO: CREATE SOME STATIC METHODS FOR THESE RESOURCES
            this.FullNoTerrainSupportLayerSymbol = CreateRectangleSymbol(0 * 2 * Math.PI / 8.0);
            this.ConnectionLayerSymbol = CreateRectangleSymbol(1 * 2 * Math.PI / 8.0);
            this.WalkableLayerSymbol = CreateRectangleSymbol(2 * 2 * Math.PI / 8.0);
            this.PlacementLayerSymbol = CreateRectangleSymbol(3 * 2 * Math.PI / 8.0);
            this.RoomLayerSymbol = CreateRectangleSymbol(4 * 2 * Math.PI / 8.0);
            this.CorridorLayerSymbol = CreateRectangleSymbol(5 * 2 * Math.PI / 8.0);
            this.WallLayerSymbol = CreateRectangleSymbol(6 * 2 * Math.PI / 8.0);
            this.TerrainSupportLayerSymbol = CreateRectangleSymbol(7 * 2 * Math.PI / 8.0);

            this.DataContextChanged += (sender, args) =>
            {
                if (args.NewValue is LevelBranchTemplateViewModel)
                {
                    Initialize(args.NewValue as LevelBranchTemplateViewModel);

                    _initialized = true;
                }
                else
                    _initialized = _previewLayoutTemplate == null || _previewLayoutGrid == null;
            };
        }

        // Stores randomly chosen layout template data
        private void Initialize(LevelBranchTemplateViewModel viewModel)
        {
            // Get the layout generation view model
            var layoutGenerationViewModel = _randomSequenceGenerator.GetWeightedRandom(viewModel.Layouts, layoutGeneration => layoutGeneration.GenerationWeight);

            // Map layout template view model -> layout template
            _previewLayoutTemplate = _scenarioConfigurationMapper.MapObject<LayoutTemplateViewModel, LayoutTemplate>(layoutGenerationViewModel.Asset, true);

            // Create Layout and store
            _previewLayoutGrid = _layoutGenerator.CreateLayout(_previewLayoutTemplate);

            // Setup terrain layer box
            this.PreviewTerrainLayerLB.ItemsSource = _previewLayoutTemplate.TerrainLayers.Select(x => new TerrainSelectionViewModel()
            {
                IsSelected = true,
                Name = x.TerrainLayer.Name
            });

            // Initialize Rendering (AS IF FROM UI)
            RenderFromUI();
        }

        private void OnPreviewChanged(object sender, RoutedEventArgs args)
        {
            RenderFromUI();
        }

        private void RenderFromUI()
        {
            if (!_initialized)
                return;

            // Preview Stack Visibility
            var layoutMode = (this.LayoutRB.IsChecked == true);

            if (layoutMode)
            {
                this.LayoutLayerContainer.Visibility = Visibility.Visible;
                this.PreviewLayerContainer.Visibility = Visibility.Collapsed;

                RenderLayoutMode(this.FullNoTerrainSupportLayerCB.IsChecked.GetValueOrDefault(),
                                 this.WalkableLayerCB.IsChecked.GetValueOrDefault(),
                                 this.PlacementLayerCB.IsChecked.GetValueOrDefault(),
                                 this.RoomLayerCB.IsChecked.GetValueOrDefault(),
                                 this.ConnectionRoomLayerCB.IsChecked.GetValueOrDefault(),
                                 this.CorridorLayerCB.IsChecked.GetValueOrDefault(),
                                 this.WallLayerCB.IsChecked.GetValueOrDefault(),
                                 this.TerrainSupportLayerCB.IsChecked.GetValueOrDefault());
            }
            else
            {
                this.LayoutLayerContainer.Visibility = Visibility.Collapsed;
                this.PreviewLayerContainer.Visibility = Visibility.Visible;

                // Gather the terrain layers that are selected
                var terrainLayers = this.PreviewTerrainLayerLB
                                          .Items
                                          .Cast<TerrainSelectionViewModel>()
                                          .Where(x => x.IsSelected)
                                          .Join(_previewLayoutTemplate.TerrainLayers,
                                                viewModel => viewModel.Name,
                                                layer => layer.TerrainLayer.Name,
                                                (viewModel, layer) => layer.TerrainLayer)
                                          .Actualize();

                RenderPreviewMode(this.PreviewWalkableLayerCB.IsChecked.GetValueOrDefault(),
                                  this.PreviewWallLayerCB.IsChecked.GetValueOrDefault(),
                                  terrainLayers);
            }
        }

        private void RenderLayoutMode(bool fullNTSLayer,
                                      bool walkableLayer,
                                      bool placementLayer,
                                      bool roomLayer,
                                      bool connectionRoomLayer,
                                      bool corridorLayer,
                                      bool wallLayer,
                                      bool terrainSupportLayer)
        {
            if (!_initialized)
                return;

            var layerImages = new List<WriteableBitmap>();

            if (fullNTSLayer)
                layerImages.Add(_previewRenderingService.RenderLayoutLayer(_previewLayoutGrid, LayoutGrid.LayoutLayer.FullNoTerrainSupport, this.FullNoTerrainSupportLayerSymbol, false));

            if (walkableLayer)
                layerImages.Add(_previewRenderingService.RenderLayoutLayer(_previewLayoutGrid, LayoutGrid.LayoutLayer.Walkable, this.WalkableLayerSymbol, false));

            if (placementLayer)
                layerImages.Add(_previewRenderingService.RenderLayoutLayer(_previewLayoutGrid, LayoutGrid.LayoutLayer.Placement, this.PlacementLayerSymbol, false));

            if (roomLayer)
                layerImages.Add(_previewRenderingService.RenderLayoutLayer(_previewLayoutGrid, LayoutGrid.LayoutLayer.Room, this.RoomLayerSymbol, false));

            if (connectionRoomLayer)
                layerImages.Add(_previewRenderingService.RenderLayoutLayer(_previewLayoutGrid, LayoutGrid.LayoutLayer.ConnectionRoom, this.ConnectionLayerSymbol, false));

            if (corridorLayer)
                layerImages.Add(_previewRenderingService.RenderLayoutLayer(_previewLayoutGrid, LayoutGrid.LayoutLayer.Corridor, this.CorridorLayerSymbol, false));

            if (wallLayer)
                layerImages.Add(_previewRenderingService.RenderLayoutLayer(_previewLayoutGrid, LayoutGrid.LayoutLayer.Wall, this.WallLayerSymbol, false));

            if (terrainSupportLayer)
                layerImages.Add(_previewRenderingService.RenderLayoutLayer(_previewLayoutGrid, LayoutGrid.LayoutLayer.TerrainSupport, this.TerrainSupportLayerSymbol, false));

            if (layerImages.Count == 0)
            {
                this.PreviewImage.Source = null;
                return;
            }

            // MANUALLY SETTING TRANSLUCENSE
            if (layerImages.Count > 1)
            {
                layerImages = layerImages.Select((image, index) =>
                {
                    // SKIP THE FIRST
                    if (index == 0)
                        return image;

                    var translucentImage = new WriteableBitmap(image);

                    // Lower the opacity
                    translucentImage.BlitRender(image, true, 0.5f);

                    return translucentImage;

                }).ToList();
            }

            // Combine layers
            var finalRendering = layerImages.Aggregate((image1, image2) =>
            {
                return _previewRenderingService.CombineRendering(image1, image2, WriteableBitmapExtensions.BlendMode.Alpha);
            });

            this.PreviewImage.Source = finalRendering;
        }

        private void RenderPreviewMode(bool walkableLayer,
                                       bool wallLayer,
                                       IEnumerable<TerrainLayerTemplate> terrainLayers)
        {
            if (!_initialized)
                return;

            var layerImages = new List<WriteableBitmap>();

            // Walkable
            if (walkableLayer)
                layerImages.Add(_previewRenderingService.RenderLayoutLayer(_previewLayoutGrid, LayoutGrid.LayoutLayer.Walkable, _previewLayoutTemplate.CellSymbol, true));

            if (wallLayer)
            {
                // Walls
                layerImages.Add(_previewRenderingService.RenderLayoutLayer(_previewLayoutGrid, LayoutGrid.LayoutLayer.Wall, _previewLayoutTemplate.WallSymbol, true));

                // Wall Lights
                layerImages.Add(_previewRenderingService.RenderWallLightLayer(_previewLayoutGrid, _previewLayoutTemplate.WallLightSymbol, true));
            }

            // Terrain
            foreach (var layer in terrainLayers)
            {
                // VERIFY THAT LAYER WAS GENERATED
                if (_previewLayoutGrid.TerrainMaps.Any(map => map.Name == layer.Name))
                    layerImages.Add(_previewRenderingService.RenderTerrainLayer(_previewLayoutGrid, layer, true));
            }

            if (layerImages.Count == 0)
            {
                this.PreviewImage.Source = null;
                return;
            }

            // Combine layers
            var finalRendering = layerImages.Aggregate((image1, image2) =>
            {
                return _previewRenderingService.CombineRendering(image1, image2, WriteableBitmapExtensions.BlendMode.Alpha);
            });

            this.PreviewImage.Source = finalRendering;
        }

        private SymbolDetailsTemplate CreateRectangleSymbol(double hue)
        {
            return new SymbolDetailsTemplate()
            {
                SymbolType = SymbolType.Terrain,
                SymbolSize = CharacterSymbolSize.Large,
                SymbolEffectType = CharacterSymbolEffectType.HslShift,
                SymbolHue = hue,

                // TODO: REMOVE THIS AND CREATE SOME WAY USING THE RESOURCE SERVICE
                SymbolPath = "Wall"
            };
        }
    }
}
