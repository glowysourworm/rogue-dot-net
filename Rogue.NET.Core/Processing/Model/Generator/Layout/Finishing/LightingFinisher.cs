using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.Scenario.Dynamic.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm.Interface;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
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
        readonly IVisibilityCalculator _visibilityCalculator;
        readonly ILightGenerator _lightGenerator;
        readonly INoiseGenerator _noiseGenerator;

        // Constants for creating space between wall lights
        const int WALL_LIGHT_SPACE_PARAMETER = 20;
        const int WALL_LIGHT_SPACE_MINIMUM = 5;

        // Lighting constants
        const double LIGHT_INTENSITY_THRESHOLD = 0.35;
        const double LIGHT_POWER_LAW = 0.75;
        const double LIGHT_FALLOFF_RADIUS = 2.0;
        const double LIGHT_PERLIN_FREQUENCY = 0.08;

        [ImportingConstructor]
        public LightingFinisher(ILightGenerator lightGenerator, INoiseGenerator noiseGenerator, IVisibilityCalculator visibilityCalculator, IRandomSequenceGenerator randomSequenceGenerator)
        {
            _lightGenerator = lightGenerator;
            _noiseGenerator = noiseGenerator;
            _visibilityCalculator = visibilityCalculator;
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public void CreateLighting(GridCellInfo[,] grid, IEnumerable<Region<GridCellInfo>> roomRegions, LayoutTemplate template)
        {
            // Procedure
            //
            // - Create white light threshold for the level using the scenario configuration setting
            // - Create layers 1 and 2 if they're set (using RGB averages to add light color channels)
            // - Store the results as the cell's base lighting
            //

            // Create the white light threshold
            CreateLightThreshold(grid, template);

            switch (template.LightingAmbient1.Type)
            {
                case TerrainAmbientLightingType.None:
                    break;
                case TerrainAmbientLightingType.PerlinNoiseLarge:
                    CreatePerlinNoiseLighting(grid, template.LightingAmbient1);
                    break;
                case TerrainAmbientLightingType.WhiteNoise:
                    CreateWhiteNoiseLighting(grid, template.LightingAmbient1);
                    break;
                case TerrainAmbientLightingType.WallLighting:
                    CreateWallLighting(grid, template.LightingAmbient1);
                    break;
                default:
                    throw new Exception("Unhandled Terrain Ambient Lighting Type");
            }

            switch (template.LightingAmbient2.Type)
            {
                case TerrainAmbientLightingType.None:
                    break;
                case TerrainAmbientLightingType.PerlinNoiseLarge:
                    CreatePerlinNoiseLighting(grid, template.LightingAmbient2);
                    break;
                case TerrainAmbientLightingType.WhiteNoise:
                    CreateWhiteNoiseLighting(grid, template.LightingAmbient2);
                    break;
                case TerrainAmbientLightingType.WallLighting:
                    CreateWallLighting(grid, template.LightingAmbient2);
                    break;
                default:
                    throw new Exception("Unhandled Terrain Ambient Lighting Type");
            }
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
                    grid[i, j].BaseLight = new Light(0xFF, 0xFF, 0xFF, ScaleIntensity(template.LightingThreshold));
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
                        grid[i, j].BaseLight = ColorFilter.AddLight(grid[i, j].BaseLight, light);
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
                        grid[column, row].BaseLight = ColorFilter.AddLight(grid[column, row].BaseLight, light);
                    }
                }

                return value;
            });
        }

        private void CreateWallLighting(GridCellInfo[,] grid, LightAmbientTemplate template)
        {
            // Create the light for the room
            var light = _lightGenerator.GenerateLight(template.Light);

            var installedWallLights = new List<GridLocation>();

            // Create a minimum radius that is a function of the fill ratio
            var minimumRadius = ((1 - template.FillRatio) * WALL_LIGHT_SPACE_PARAMETER) + WALL_LIGHT_SPACE_MINIMUM;

            // Collect the combined FOV for all wall lights
            var wallLightFOV = new Dictionary<GridCellInfo, IEnumerable<DistanceLocation>>();

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
                        var wallLightIntensity = ScaleIntensity(_randomSequenceGenerator.GetRandomValue(template.IntensityRange));

                        // Blend in the resulting light 
                        grid[i, j].WallLight = new Light(light.Red, light.Green, light.Blue, wallLightIntensity);
                        grid[i, j].IsWallLight = true;
                        grid[i, j].BaseLight = ColorFilter.AddLight(grid[i, j].BaseLight,
                                                                    new Light(light.Red, light.Green, light.Blue, wallLightIntensity));

                        // Add to field of view
                        wallLightFOV.Add(grid[i, j], _visibilityCalculator.CalculateVisibility(grid, grid[i, j].Location));
                    }
                }
            }

            // Add base lighting contributions for the wall lights
            foreach (var entry in wallLightFOV)
            {
                foreach (var cell in entry.Value)
                {
                    // Calculate 1 / r^a intensity (pseudo-power-law)
                    var wallLight = entry.Key.WallLight;
                    var intensity = cell.EuclideanDistance > LIGHT_FALLOFF_RADIUS ? (light.Intensity / System.Math.Pow(cell.EuclideanDistance - LIGHT_FALLOFF_RADIUS, LIGHT_POWER_LAW))
                                                                                  : light.Intensity;

                    // Don't modify base lighting if the intensity is too low
                    if (intensity < LIGHT_INTENSITY_THRESHOLD)
                        continue;

                    // Add contribution to the effective lighting
                    grid[cell.Location.Column, cell.Location.Row].BaseLight = ColorFilter.AddLight(grid[cell.Location.Column, cell.Location.Row].BaseLight,
                                                                                                   new Light(wallLight.Red, wallLight.Green, wallLight.Blue, intensity));
                }
            }
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
                    grid[i, j].BaseLight = new Light(0xFF, 0xFF, 0xFF, 1);
                }
            }
        }

        /// <summary>
        /// Takes [0,1] intensity and scales it linearly to the constraints
        /// </summary>
        private double ScaleIntensity(double unitIntensity)
        {
            return ((1 - LIGHT_INTENSITY_THRESHOLD) * unitIntensity) + LIGHT_INTENSITY_THRESHOLD;
        }
    }
}
