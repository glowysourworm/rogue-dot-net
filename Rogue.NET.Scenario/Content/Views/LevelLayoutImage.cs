using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Extension.Prism.EventAggregator;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Character;
using Rogue.NET.Core.Model.Scenario.Character.Extension;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Abstract;
using Rogue.NET.Core.Processing.Event.Level;
using Rogue.NET.Core.Processing.Service.Cache.Interface;
using Rogue.NET.Core.Processing.Service.Interface;
using Rogue.NET.Core.Processing.Service.Rendering;
using Rogue.NET.Scenario.Content.ViewModel.LevelCanvas.Inteface;
using Rogue.NET.Scenario.Processing.Event.Content;
using Rogue.NET.Scenario.Processing.Service.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rogue.NET.Scenario.Content.Views
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export]
    public class LevelLayoutImage : Image
    {
        readonly IModelService _modelService;
        readonly IScenarioRenderingService _scenarioRenderingService;

        WriteableBitmap _rendering;
        bool _firstRendering = true;

        [ImportingConstructor]
        public LevelLayoutImage(IModelService modelService,
                                ILevelCanvasViewModel levelCanvasViewModel,
                                IScenarioUIService scenarioUIService,
                                IRogueEventAggregator eventAggregator,
                                IScenarioResourceService scenarioResourceService,
                                IScenarioBitmapSourceFactory scenarioBitmapSourceFactory,
                                IScenarioRenderingService scenarioRenderingService)
        {
            _modelService = modelService;
            _scenarioRenderingService = scenarioRenderingService;

            eventAggregator.GetEvent<LevelLoadedEvent>().Subscribe(() =>
            {
                // Reset the writeable bitmap
                _rendering = null;
                _firstRendering = true;

                RenderLayout();

            // SET PRIORITY TO EXECUTE BEFORE THE LEVEL IS INVALIDATED
            }, RogueEventPriority.Critical);

            eventAggregator.GetEvent<ZoomEvent>().Subscribe(eventData =>
            {
                RenderLayout();
            });

            // PRIMARY INVALIDATION EVENT
            levelCanvasViewModel.VisibilityUpdated += RenderLayout;
        }

        private void RenderLayout()
        {
            var layoutTemplate = _modelService.GetLayoutTemplate();
            var layoutGrid = _modelService.Level.Grid;

            // Create a collection for the terrain maps / terrain symbols
            var terrainMaps = layoutGrid.TerrainMaps
                                        .Join(layoutTemplate.TerrainLayers,
                                              leftTerrain => leftTerrain.Name,
                                              rightTerrain => rightTerrain.TerrainLayer.Name,
                                              (left, right) =>
                                              {
                                                  return new
                                                  {
                                                      Layer = right,
                                                      LayerMap = left,
                                                      Symbol = right.TerrainLayer.FillSymbolDetails,
                                                      Edge = right.TerrainLayer.EdgeSymbolDetails,
                                                      HasEdge = right.TerrainLayer.HasEdgeSymbol
                                                  };
                                              })

                                        // Below Ground -> Ground -> Above Ground
                                        .OrderBy(x => x.Layer.TerrainLayer.Layer)
                                        .Actualize();

            var renderingLayers = new List<RenderingLayer>();

            // Floor -> Walls -> Wall Lights
            renderingLayers.Add(new RenderingLayoutLayer(layoutGrid.WalkableMap, layoutTemplate.CellSymbol, TerrainLayer.BelowGround));
            renderingLayers.Add(new RenderingLayoutLayer(layoutGrid.WallMap, layoutTemplate.WallSymbol, TerrainLayer.Ground));
            renderingLayers.Add(new RenderingContentLayer(layoutGrid.Bounds.Width, 
                                                          layoutGrid.Bounds.Height,              
                                                          
                                                          // Wall Lights
                                                          (column, row) =>
                                                          {
                                                              var cell = layoutGrid[column, row];
                                                                
                                                              if (cell != null &&
                                                                  cell.IsWallLight)
                                                                  return layoutTemplate.WallLightSymbol;

                                                              return null;

                                                          }, TerrainLayer.Ground));

            // Terrain Layers
            renderingLayers.AddRange(terrainMaps.Select(map => new RenderingLayoutLayer(map.LayerMap, 
                                                                                        map.Symbol, 
                                                                                        map.Edge, 
                                                                                        map.HasEdge, 
                                                                                        map.Layer.TerrainLayer.Layer)));

            // Create Rendering! :)
            _rendering = _scenarioRenderingService.Render(new RenderingSpecification(renderingLayers,
            (column, row) => _modelService.Level.Movement.IsVisible(column, row),                           // Is Visibile Callback                
            (column, row) => _modelService.Level.Movement.WasVisible(column, row),                          // Was Visible Callback                
            (column, row) => _modelService.Level.Movement.GetEffectiveVision(column, row),                  // Effective Vision Callback                
            (column, row) => _modelService.Level.Grid[column, row].IsRevealed,                              // Revealed Callback                
            (column, row) => _modelService.Level.Grid[column, row].IsExplored,                              // Explored Callback                
            (column, row) => _modelService.Level.Grid[column, row].Lights,                                  // Lighting Callback
            _modelService.ZoomFactor));

            this.Source = _rendering;
        }
    }
}
