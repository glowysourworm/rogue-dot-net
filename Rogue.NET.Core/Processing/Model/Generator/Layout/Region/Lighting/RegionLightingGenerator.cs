using Microsoft.Practices.ServiceLocation;
using Rogue.NET.Core.Math.Algorithm;
using Rogue.NET.Core.Media.SymbolEffect.Utility;
using Rogue.NET.Core.Model.Scenario.Content.Layout;
using Rogue.NET.Core.Model.ScenarioConfiguration.Layout;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using RegionModel = Rogue.NET.Core.Model.Scenario.Content.Layout.Region;

namespace Rogue.NET.Core.Processing.Model.Generator.Layout.Region.Lighting
{
    public static class RegionLightingGenerator
    {
        readonly static IRandomSequenceGenerator _randomSequenceGenerator;

        static RegionLightingGenerator()
        {
            _randomSequenceGenerator = ServiceLocator.Current.GetInstance<IRandomSequenceGenerator>();
        }

        public static void CreateLightedRoom(GridCellInfo[,] grid, RegionModel region, LayoutTemplate template)
        {
            // Convert the lighting color from the template
            var light = ColorFilter.Convert(template.LightingColor);

            // Create the color with the lighting intensity
            var color = Color.FromArgb((byte)(int)(256.0 * template.LightingIntensity), light.R, light.G, light.B);

            // Combine color with existing lighting for the cell
            foreach (var cell in region.Cells)
                grid[cell.Column, cell.Row].Lighting = ColorFilter.AddLightingEffect(grid[cell.Column, cell.Row].Lighting, color);
        }

        public static void CreateWhiteNoiseLighting(GridCellInfo[,] grid, LayoutTemplate template)
        {
            // Convert the lighting color from the template
            var light = ColorFilter.Convert(template.LightingColor);

            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j] == null)
                        continue;

                    if (_randomSequenceGenerator.Get() < template.LightingRatio)
                        grid[i, j].Lighting = Color.FromArgb((byte)(int)(255.0 * _randomSequenceGenerator.Get()), light.R, light.G, light.B); ;
                }
            }
        }

        public static void CreatePerlinNoiseLighting(GridCellInfo[,] grid, LayoutTemplate template)
        {
            // Convert the lighting color from the template
            var light = ColorFilter.Convert(template.LightingColor);

            NoiseGenerator.GeneratePerlinNoise(grid.GetLength(0), grid.GetLength(1), template.LightingRatio * 0.1, (column, row, value) =>
            {
                if (grid[column, row] != null)
                {
                    // Scale the value from [-1, 1] -> [0, 1]
                    var scaledValue = (value * 0.5) + 0.5;

                    // Create the color with the lighting intensity
                    grid[column, row].Lighting = Color.FromArgb((byte)(int)(255.0 * template.LightingIntensity * scaledValue), light.R, light.G, light.B);
                }

                return value;
            });
        }
    }
}
