using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Algorithm.Interface;
using Rogue.NET.Core.Processing.Model.Extension;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System.Collections.Generic;
using System.Linq;
using RegionModel = Rogue.NET.Core.Model.Scenario.Content.Layout.Region;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Lighting
{
    public static class RegionLightingGenerator
    {
        readonly static IRandomSequenceGenerator _randomSequenceGenerator;
        readonly static ILightGenerator _lightGenerator;

        static RegionLightingGenerator()
        {
            _randomSequenceGenerator = ServiceLocator.Current.GetInstance<IRandomSequenceGenerator>();
            _lightGenerator = ServiceLocator.Current.GetInstance<ILightGenerator>();
        }

        public static void CreateLightThreshold(GridCellInfo[,] grid, LayoutTemplate template)
        {
            if (template.LightingThreshold <= 0)
                return;

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == null)
                        continue;

                    // Set a white light threshold (to simulate white light)
                    grid[i, j].BaseLight = new Light(0xFF, 0xFF, 0xFF, template.LightingThreshold);
                }
            }
        }

        public static void CreateLightedRooms(GridCellInfo[,] grid, IEnumerable<RegionModel> regions, LightAmbientTemplate template)
        {
            // Create the light for the room
            var light = _lightGenerator.GenerateLight(template.Light);

            foreach (var region in regions)
            {
                if (_randomSequenceGenerator.Get() < template.FillRatio)
                {
                    // Create random intensity within the parameters
                    light.Intensity = _randomSequenceGenerator.GetRandomValue(template.IntensityRange);

                    // Combine color with existing lighting for the cell
                    foreach (var cell in region.Cells)
                        grid[cell.Column, cell.Row].BaseLight = ColorFilter.AddLight(grid[cell.Column, cell.Row].BaseLight, light);
                }
            }
        }

        public static void CreateWhiteNoiseLighting(GridCellInfo[,] grid, LightAmbientTemplate template)
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
                        light.Intensity = _randomSequenceGenerator.GetRandomValue(template.IntensityRange);

                        // Blend the current light value with the new light
                        grid[i, j].BaseLight = ColorFilter.AddLight(grid[i, j].BaseLight, light);
                    }
                }
            }
        }

        public static void CreatePerlinNoiseLighting(GridCellInfo[,] grid, LightAmbientTemplate template)
        {
            // Create the light for the room
            var light = _lightGenerator.GenerateLight(template.Light);

            // Create a frequency for the perlin noise
            var frequency = template.Type == TerrainAmbientLightingType.PerlinNoiseSmall ? 0.5 : 0.08;

            NoiseGenerator.GeneratePerlinNoise(grid.GetLength(0), grid.GetLength(1), frequency, (column, row, value) =>
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
                        light.Intensity = (subScaledValue * (template.IntensityRange.High - template.IntensityRange.Low)) + template.IntensityRange.Low;

                        // Add light to the grid
                        grid[column, row].BaseLight = ColorFilter.AddLight(grid[column, row].BaseLight, light);
                    }
                }

                return value;
            });
        }

        public static void CreateWallLighting(GridCellInfo[,] grid, LightAmbientTemplate template)
        {
            // Create the light for the room
            var light = _lightGenerator.GenerateLight(template.Light);

            // TODO:TERRAIN - FIND A WAY TO PUT THESE EVERY "X" CELLS AROUND THE EDGE (WHERE "X" IS CALCULATED BASED ON THE FILL RATIO)

            // Combine color with existing lighting for the cell
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == null)
                        continue;

                    if (!grid[i, j].IsWall)
                        continue;

                    if (_randomSequenceGenerator.Get() < template.FillRatio)
                    {
                        // Go ahead and adjust light intensity here since the light instance is only local
                        var wallLightIntensity = _randomSequenceGenerator.GetRandomValue(template.IntensityRange);

                        // Blend in the resulting light 
                        grid[i, j].WallLight = new Light(light.Red, light.Green, light.Blue, wallLightIntensity);
                        grid[i, j].IsWallLight = true;
                    }
                }
            }
        }
    }
}
