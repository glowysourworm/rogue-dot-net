using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Construction;
using Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

using static Rogue.NET.Core.Math.Algorithm.Interface.INoiseGenerator;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Finishing
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(ILightingFinisher))]
    public class LightingFinisher : ILightingFinisher
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;
        readonly ILightGenerator _lightGenerator;
        readonly INoiseGenerator _noiseGenerator;

        // Constants for creating space between wall lights
        const int WALL_LIGHT_SPACE_PARAMETER = 20;
        const int WALL_LIGHT_SPACE_MINIMUM = 5;

        // Lighting constants
        const double LIGHT_POWER_LAW = 0.75;
        const double LIGHT_FALLOFF_RADIUS = 2.0;
        const double LIGHT_PERLIN_FREQUENCY = 0.08;

        [ImportingConstructor]
        public LightingFinisher(ILightGenerator lightGenerator, INoiseGenerator noiseGenerator, IRandomSequenceGenerator randomSequenceGenerator)
        {
            _lightGenerator = lightGenerator;
            _noiseGenerator = noiseGenerator;
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public void CreateLighting(LayoutContainer container, LayoutTemplate template)
        {
            // Procedure
            //
            // - Create white light threshold for the level using the scenario configuration setting
            // - Create layers 1 and 2 if they're set (using RGB averages to add light color channels)
            // - Store the results as the cell's base lighting
            //

            // Create the white light threshold
            CreateLightThreshold(container.Grid, template);

            // Wall lights
            if (template.HasWallLights)
                CreateWallLighting(container.Grid, template);

            switch (template.AccentLighting.Type)
            {
                case TerrainAmbientLightingType.None:
                    break;
                case TerrainAmbientLightingType.PerlinNoiseLarge:
                    CreatePerlinNoiseLighting(container.Grid, template.AccentLighting);
                    break;
                case TerrainAmbientLightingType.WhiteNoise:
                    CreateWhiteNoiseLighting(container.Grid, template.AccentLighting);
                    break;
                default:
                    throw new Exception("Unhandled Terrain Ambient Lighting Type");
            }

            CreateTerrainLighting(container, template);
        }

        private void CreateLightThreshold(GridCellInfo[,] grid, LayoutTemplate template)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == null)
                        continue;

                    // Set a white light threshold (to simulate white light)
                    grid[i, j].AmbientLight = new Light(Light.White, ScaleIntensity(template.LightingThreshold));
                }
            }
        }

        private void CreateWhiteNoiseLighting(GridCellInfo[,] grid, LightAmbientTemplate template)
        {
            // Create the light for the room
            var light = _lightGenerator.GenerateLight(template.Light);

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == null)
                        continue;

                    if (_randomSequenceGenerator.Get() < template.FillRatio)
                    {
                        // Go ahead and adjust light intensity here since the light instance is only local
                        light.Intensity = ScaleIntensity(_randomSequenceGenerator.GetRandomValue(template.IntensityRange));

                        // Blend the current light value with the new light
                        grid[i, j].AccentLight = light;
                    }
                }
            }
        }

        private void CreatePerlinNoiseLighting(GridCellInfo[,] grid, LightAmbientTemplate template)
        {
            // Create the light for the room
            var light = _lightGenerator.GenerateLight(template.Light);

            _noiseGenerator.Run(NoiseType.PerlinNoise, grid.GetLength(0), grid.GetLength(1), LIGHT_PERLIN_FREQUENCY, (column, row, value) =>
            {
                if (grid[column, row] != null)
                {
                    // Scale the value from [-1, 1] -> [0, 1]
                    var scaledValue = (value * 0.5) + 0.5;

                    // Calculate the "from the peak" value
                    var peakMeasuredValue = (1 - scaledValue);

                    // Fill in values that fall into the fill ratio "bucket"
                    if (peakMeasuredValue <= template.FillRatio)
                    {
                        // Create a sub-scale to represent this "bucket" on [0,1]
                        var subScaledValue = peakMeasuredValue / template.FillRatio;

                        // Create light intensity value by scaling the sub-scaled value from the min -> max intensity
                        light.Intensity = ScaleIntensity((subScaledValue * (template.IntensityRange.High - template.IntensityRange.Low)) + template.IntensityRange.Low);

                        // Add light to the grid
                        grid[column, row].AccentLight = light;
                    }
                }

                return value;
            });
        }

        private void CreateWallLighting(GridCellInfo[,] grid, LayoutTemplate template)
        {
            // Create the light for the room
            var light = _lightGenerator.GenerateLight(template.WallLight);

            var installedWallLights = new List<GridLocation>();

            // Create a minimum radius that is a function of the fill ratio
            var minimumRadius = ((1 - template.WallLightFillRatio) * WALL_LIGHT_SPACE_PARAMETER) + WALL_LIGHT_SPACE_MINIMUM;

            // Collect the combined FOV for all wall lights
            var wallLightFOV = new Dictionary<GridCellInfo, List<DistanceLocation>>();

            // Combine color with existing lighting for the cell
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == null)
                        continue;

                    if (!grid[i, j].IsWall)
                        continue;

                    // Create the first wall light
                    if (!installedWallLights.Any(location => Metric.EuclideanDistance(grid[i, j].Location, location) <= minimumRadius))
                    {
                        installedWallLights.Add(grid[i, j].Location);

                        // Go ahead and adjust light intensity here since the light instance is only local
                        var wallLightIntensity = ScaleIntensity(template.WallLightIntensity);

                        // Blend in the resulting light 
                        grid[i, j].WallLight = new Light(light.Red, light.Green, light.Blue, wallLightIntensity);
                        grid[i, j].IsWallLight = true;

                        CreatePointSourceLighting(grid, i, j, grid[i, j].WallLight);
                    }
                }
            }
        }

        private void CreateTerrainLighting(LayoutContainer container, LayoutTemplate template)
        {
            foreach (var terrainLayer in template.TerrainLayers
                                 .Where(layer => layer.TerrainLayer.EmitsLight)
                                 .Select(layer => layer.TerrainLayer)
                                 .Actualize())
            {

                var light = _lightGenerator.GenerateLight(terrainLayer.EmittedLight);

                container.Grid.Iterate((column, row) =>
                {
                    var layerInfo = container.TerrainLayers.FirstOrDefault(layer => layer[column, row] != null &&
                                                                    layer.LayerName == terrainLayer.Name);

                    // Create point source light at this location
                    if (layerInfo != null)
                        CreatePointSourceLighting(container.Grid, column, row, light, true, layerInfo.LayerName);

                });
            }
        }

        private void CreatePointSourceLighting(GridCellInfo[,] grid, int column, int row, Light light, bool terrainLight = false, string terrainName = null)
        {
            // Add to field of view
            VisibilityCalculator.CalculateVisibility(grid, grid[column, row].Location, (columnCallback, rowCallback, isVisible) =>
            {
                if (isVisible)
                {
                    var cell = grid[columnCallback, rowCallback];
                    var distance = Metric.EuclideanDistance(column, row, columnCallback, rowCallback);

                    // Calculate 1 / r^a intensity (pseudo-power-law)
                    var intensity = distance > LIGHT_FALLOFF_RADIUS ? (light.Intensity / System.Math.Pow(distance - LIGHT_FALLOFF_RADIUS, LIGHT_POWER_LAW))
                                                                    : light.Intensity;

                    // Clip intensity to 1 for small distances
                    intensity = intensity.Clip(0, 1);

                    // var intensity = light.Intensity / System.Math.Pow(distance, LIGHT_POWER_LAW);

                    // Don't modify base lighting if the intensity is too low
                    if (intensity < ModelConstants.MinLightIntensity)
                        return;

                    var existingWallLight = grid[columnCallback, rowCallback].WallLight;

                    // Add contribution to the lighting
                    if (!terrainLight)
                    {
                        // Take the max of combined intensities for the cell
                        grid[columnCallback, rowCallback].WallLight.Intensity = System.Math.Max(intensity, existingWallLight.Intensity);
                    }

                    else
                    {
                        if (!cell.TerrainLights.ContainsKey(terrainName))
                            cell.TerrainLights.Add(terrainName, new Light(light, intensity));

                        else
                        {
                            // Take the max of combined intensities for the cell
                            cell.TerrainLights[terrainName].Intensity = System.Math.Max(intensity, cell.TerrainLights[terrainName].Intensity);
                        }
                    }
                }
            });
        }

        public void CreateDefaultLighting(GridCellInfo[,] grid)
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == null)
                        continue;

                    // Set a white light threshold (to simulate white light)
                    grid[i, j].AmbientLight = Light.White;
                }
            }
        }

        /// <summary>
        /// Takes [0,1] intensity and scales it linearly to the constraints
        /// </summary>
        private double ScaleIntensity(double unitIntensity)
        {
            return ((1 - ModelConstants.MinLightIntensity) * unitIntensity) + ModelConstants.MinLightIntensity;
        }
    }
}
