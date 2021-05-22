using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Math.Geometry;
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

        public void CreateLighting(FinalizedLayoutContainer container, LayoutTemplate template)
        {
            // Procedure
            //
            // - Create white light threshold for the level using the scenario configuration setting
            // - Create layers 1 and 2 if they're set (using RGB averages to add light color channels)
            // - Store the results as the cell's base lighting
            //

            // Create the white light threshold
            CreateLightThreshold(container, template);

            // Wall lights
            if (template.HasWallLights)
                CreateWallLighting(container, template);

            switch (template.AccentLighting.Type)
            {
                case TerrainAmbientLightingType.None:
                    break;
                case TerrainAmbientLightingType.PerlinNoiseLarge:
                    CreatePerlinNoiseLighting(container, template.AccentLighting);
                    break;
                case TerrainAmbientLightingType.WhiteNoise:
                    CreateWhiteNoiseLighting(container, template.AccentLighting);
                    break;
                default:
                    throw new Exception("Unhandled Terrain Ambient Lighting Type");
            }

            CreateTerrainLighting(container, template);
        }

        private void CreateLightThreshold(FinalizedLayoutContainer container, LayoutTemplate template)
        {
            container.Grid.Iterate((column, row) =>
            {
                if (container.Grid[column, row] == null)
                    return;

                var intensity = ScaleIntensity(template.LightingThreshold);

                intensity = intensity.Round(1);

                // Set a white light threshold (to simulate white light)
                container.Grid[column, row].AmbientLight = new Light(Light.White, intensity);
            });

        }

        private void CreateWhiteNoiseLighting(FinalizedLayoutContainer container, LightAmbientTemplate template)
        {
            // Create the light for the room
            var light = _lightGenerator.GenerateLight(template.Light);

            container.Grid.Iterate((column, row) =>
            {
                if (container.Grid[column, row] == null)
                    return;

                if (_randomSequenceGenerator.Get() < template.FillRatio)
                {
                    // Go ahead and adjust light intensity here since the light instance is only local
                    var intensity = ScaleIntensity(_randomSequenceGenerator.GetRandomValue(template.IntensityRange));

                    intensity = intensity.Round(1);

                    // Blend the current light value with the new light
                    container.Grid[column, row].AccentLight = new Light(light, intensity);
                }
            });
        }

        private void CreatePerlinNoiseLighting(FinalizedLayoutContainer container, LightAmbientTemplate template)
        {
            // Create the light for the room
            var light = _lightGenerator.GenerateLight(template.Light);

            _noiseGenerator.Run(NoiseType.PerlinNoise, container.Grid.GetLength(0), container.Grid.GetLength(1), LIGHT_PERLIN_FREQUENCY, (column, row, value) =>
            {
                if (container.Grid[column, row] != null)
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
                        var intensity = ScaleIntensity((subScaledValue * (template.IntensityRange.High - template.IntensityRange.Low)) + template.IntensityRange.Low);

                        intensity = intensity.Round(1);

                        // Add light to the grid
                        container.Grid[column, row].AccentLight = new Light(light, intensity);
                    }
                }

                return value;
            });
        }

        private void CreateWallLighting(FinalizedLayoutContainer container, LayoutTemplate template)
        {
            // Create the light for the room
            var light = _lightGenerator.GenerateLight(template.WallLight);

            var installedWallLights = new List<GridLocation>();

            // Create a minimum radius that is a function of the fill ratio
            var minimumRadius = ((1 - template.WallLightFillRatio) * WALL_LIGHT_SPACE_PARAMETER) + WALL_LIGHT_SPACE_MINIMUM;

            // Collect the combined FOV for all wall lights
            var wallLightFOV = new Dictionary<GridCellInfo, List<DistanceLocation>>();

            // Combine color with existing lighting for the cell
            container.Grid.Iterate((column, row) =>
            {
                var cell = container.Grid[column, row];

                if (cell == null)
                    return;

                if (!cell.IsWall)
                    return;

                // Create the first wall light
                if (!installedWallLights.Any(location => Metric.ForceDistance(cell.Location, location, Metric.MetricType.Euclidean) <= minimumRadius))
                {
                    installedWallLights.Add(cell.Location);

                    // Go ahead and adjust light intensity here since the light instance is only local
                    var wallLightIntensity = ScaleIntensity(template.WallLightIntensity).Round(1);

                    // Blend in the resulting light 
                    cell.WallLight = new Light(light.Red, light.Green, light.Blue, wallLightIntensity);
                    cell.IsWallLight = true;

                    CreatePointSourceLighting(container, column, row, cell.WallLight);
                }
            });
        }

        private void CreateTerrainLighting(FinalizedLayoutContainer container, LayoutTemplate template)
        {
            foreach (var terrainLayer in template.TerrainLayers
                                                 .Where(layer => layer.TerrainLayer.EmitsLight)
                                                 .Select(layer => layer.TerrainLayer)
                                                 .Actualize())
            {

                var light = _lightGenerator.GenerateLight(terrainLayer.EmittedLight);
                var terrainRegions = container.TerrainDict[terrainLayer];

                container.Grid.Iterate((column, row) =>
                {
                    // Create point source light at this location
                    if (terrainRegions.Any(region => region[column, row] != null))
                        CreatePointSourceLighting(container, column, row, light, true, terrainLayer.Name);
                });
            }
        }

        private void CreatePointSourceLighting(FinalizedLayoutContainer container, int column, int row, Light light, bool terrainLight = false, string terrainName = null)
        {
            // Add to field of view
            VisibilityCalculator.CalculateVisibility(container.Grid[column, row].Location, ModelConstants.MaxVisibileRadiusNPC,
                (column, row) =>
                {
                    if (container.Bounds.Contains(column, row))
                        return container.Grid[column, row]?.Location;

                    return null;
                },
                (column, row) =>
                {
                    if (container.Bounds.Contains(column, row))
                        return container.Grid[column, row] == null ||
                               container.Grid[column, row].IsWall ||
                               container.Grid[column, row].IsDoor;

                    return false;
                },
            (column, row, isVisible) =>
            {
                if (isVisible)
                {
                    var cell = container.Grid[column, row];
                    var distance = Metric.ForceDistance(column, row, column, row, Metric.MetricType.Euclidean);

                    // Calculate 1 / r^a intensity (pseudo-power-law)
                    var intensity = distance > LIGHT_FALLOFF_RADIUS ? (light.Intensity / System.Math.Pow(distance - LIGHT_FALLOFF_RADIUS, LIGHT_POWER_LAW))
                                                                    : light.Intensity;

                    // Clip intensity to 1 for small distances
                    intensity = intensity.Clip(0, 1).Round(1);

                    // var intensity = light.Intensity / System.Math.Pow(distance, LIGHT_POWER_LAW);

                    // Don't modify base lighting if the intensity is too low
                    if (intensity < ModelConstants.MinLightIntensity)
                        return;

                    var existingWallLight = cell.WallLight;

                    // Add contribution to the lighting
                    if (!terrainLight)
                    {
                        // Take the max of combined intensities for the cell
                        if (existingWallLight != null &&
                            existingWallLight != Light.None)
                            cell.WallLight.Intensity = System.Math.Max(intensity, existingWallLight.Intensity);

                        else
                            cell.WallLight = new Light(light, intensity);
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
