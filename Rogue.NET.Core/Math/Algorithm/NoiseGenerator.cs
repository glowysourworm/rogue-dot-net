using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Math.Algorithm.Interface;
using Rogue.NET.Core.Math.Geometry;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using System;
using System.ComponentModel.Composition;
using static Rogue.NET.Core.Math.Algorithm.Interface.INoiseGenerator;

namespace Rogue.NET.Core.Math.Algorithm
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(INoiseGenerator))]
    public class NoiseGenerator : INoiseGenerator
    {
        readonly IRandomSequenceGenerator _randomSequenceGenerator;

        private const double PERLIN_NOISE_LOW_FREQUENCY = 0.06;
        private const double PERLIN_NOISE_HIGH_FREQUENCY = 0.5;

        [ImportingConstructor]
        public NoiseGenerator(IRandomSequenceGenerator randomSequenceGenerator)
        {
            _randomSequenceGenerator = randomSequenceGenerator;
        }

        public void Run(NoiseType type, int width, int height, double frequency, PostProcessingCallback callback)
        {
            switch (type)
            {
                case NoiseType.WhiteNoise:
                    GenerateWhiteNoise(width, height, frequency, callback);
                    break;
                case NoiseType.PerlinNoise:
                    GeneratePerlinNoise(width, height, frequency, callback);
                    break;
                default:
                    throw new Exception("Unhandled Noise Type:  NoiseGenerator");
            }
        }

        /// <summary>
        /// Returns white noise 2D array with [0, 1] values
        /// </summary>
        private void GenerateWhiteNoise(int width, int height, double frequency, PostProcessingCallback callback)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (_randomSequenceGenerator.Get() < frequency)
                        callback(i, j, _randomSequenceGenerator.Get());
                }
            }
        }

        /// <summary>
        /// Returns Perlin noise map using the specified [0,1] frequency. A frequency of 0 uses a (nearly) single mesh
        /// square to compute the noise map. A frequency value of 1 uses a minimum of 2x2 mesh size to compute the
        /// noise values.
        /// </summary>
        /// <param name="frequency">A [0, 1] value used to specify the relative noise erradicity.</param>
        /// <returns>Returns a Perlin noise map which has the characteristic behavior of smoothness and feature size according to the mesh cell size.</returns>
        public void GeneratePerlinNoise(int width, int height, double frequency, PostProcessingCallback callback)
        {
            // Scale the frequency input
            var scaledFrequency = (frequency * (PERLIN_NOISE_HIGH_FREQUENCY - PERLIN_NOISE_LOW_FREQUENCY)) + PERLIN_NOISE_LOW_FREQUENCY;

            // Initialize the output map
            var map = new double[width, height];

            // Calculate the mesh:  Allow the mesh to fall outside the grid boundary; but only compute
            //                      the mesh values that have an effect on the grid cells.
            //

            // Mesh Cell Width:  Creates a mesh cell width between 2.0 and the width of the grid
            var meshCellWidth = (int)(1 / scaledFrequency).Clip(2, width);

            // Mesh Cell Height:  [2.0, height]
            var meshCellHeight = (int)(1 / scaledFrequency).Clip(2, height);

            // Generate mesh that hangs over the edges of the grid (AT LEAST BY ONE MESH CELL)
            //
            var meshWidth = ((int)(width / (double)meshCellWidth)).LowLimit(width + 1);
            var meshHeight = (int)(height / (double)meshCellHeight).LowLimit(height + 1);

            // Create mesh - using points to describe random unit vectors
            var mesh = new Vector[meshWidth, meshHeight];

            // Initialize the mesh
            for (int i = 0; i < meshWidth; i++)
            {
                // Create random vector 
                for (int j = 0; j < meshHeight; j++)
                    mesh[i, j] = Vector.Create(_randomSequenceGenerator.GetDouble(0, System.Math.PI * 2), 1.0);
            }

            var maxValue = double.MinValue;
            var minValue = double.MaxValue;

            // Iterate the map - creating interpolated values
            //
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    // Calculate the mesh cell coordinates for the given map cell
                    //
                    var meshColumn = (int)((double)i / (double)meshCellWidth);
                    var meshRow = (int)((double)j / (double)meshCellHeight);

                    // Calculate the actual mesh locations with respect to the grid
                    // it overlays.
                    //
                    var meshX0 = meshColumn * (double)meshCellWidth;
                    var meshX1 = (meshColumn + 1) * meshCellWidth;
                    var meshY0 = meshRow * (double)meshCellHeight;
                    var meshY1 = (meshRow + 1) * meshCellHeight;

                    // Compute Distance Vectors for each point from the mesh cell vertex locations
                    //
                    var vectorX0Y0 = new Vector(i - meshX0, j - meshY0);
                    var vectorX1Y0 = new Vector(i - meshX1, j - meshY0);
                    var vectorX0Y1 = new Vector(i - meshX0, j - meshY1);
                    var vectorX1Y1 = new Vector(i - meshX1, j - meshY1);

                    // Compute Dot Products for each vector with the corresponding mesh vector
                    //
                    var dotX0Y0 = vectorX0Y0.Dot(mesh[meshColumn, meshRow]);
                    var dotX1Y0 = vectorX1Y0.Dot(mesh[meshColumn + 1, meshRow]);
                    var dotX0Y1 = vectorX0Y1.Dot(mesh[meshColumn, meshRow + 1]);
                    var dotX1Y1 = vectorX1Y1.Dot(mesh[meshColumn + 1, meshRow + 1]);

                    // Calculate influence weights for each of the mesh corners by interpolating the dot products
                    // with respect to the normalized distance (from each corner). An easing function is applied 
                    // as a weight to specify "how far left, or how far right" the coordinate is. (similarly in the Y-direction)
                    //
                    // https://mzucker.github.io/html/perlin-noise-math-faq.html
                    //
                    // (Better notes here)
                    //
                    // https://rmarcus.info/blog/2018/03/04/perlin-noise.html
                    //

                    // Calculate normalized coordinates for the point relative to the mesh cell
                    //
                    var normalizedX = (i - meshX0) / ((double)(meshX1 - meshX0));
                    var normalizedY = (j - meshY0) / ((double)(meshY1 - meshY0));
                    var easeX = PerlinFade(1 - normalizedX);
                    var easeY = PerlinFade(1 - normalizedY);
                    var weightA = Interpolate(dotX0Y0, dotX1Y0, easeX);
                    var weightB = Interpolate(dotX0Y1, dotX1Y1, easeX);
                    var weight = Interpolate(weightA, weightB, easeY);

                    map[i, j] = weight;

                    // Store the extreme points to normalize the output
                    //
                    if (weight > maxValue)
                        maxValue = weight;

                    if (weight < minValue)
                        minValue = weight;
                }
            }

            // Create normalization constants:  The equation y(x) = (x - B)(2 / (B - A)) + 1  maps [A, B] -> [-1, 1]
            //                                  So, given A := minValue and B  := maxValue, the output will be [-1, 1]
            //
            var slope = 2.0 / (maxValue - minValue);
            var intercept = 1 - ((2.0 * maxValue) / (maxValue - minValue));

            // Normalize the output and apply post processing callback
            //
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    // Normalize the map value
                    map[i, j] = (slope * map[i, j]) + intercept;

                    // Apply post processing callback
                    //
                    callback(i, j, map[i, j]);
                }
            }
        }

        private double PerlinEase(double unitValue)
        {
            if (unitValue > 1.0 ||
                unitValue < 0.0)
                throw new Exception("Trying to generate Perlin easing value out of bounds");

            return (3.0 * System.Math.Pow(unitValue, 2)) - (2 * System.Math.Pow(unitValue, 3));
        }

        private double PerlinFade(double unitValue)
        {
            if (unitValue > 1.0 ||
                unitValue < 0.0)
                throw new Exception("Trying to generate Perlin easing value out of bounds");

            // https://rmarcus.info/blog/2018/03/04/perlin-noise.html
            //
            return (6 * System.Math.Pow(unitValue, 5)) - (15 * System.Math.Pow(unitValue, 4)) + (10 * System.Math.Pow(unitValue, 3));
        }

        private double Interpolate(double value1, double value2, double weight)
        {
            if (weight < 0 || weight > 1)
                throw new Exception("Improper interpolation weight NoiseGenerator");

            return (weight * value1) + ((1 - weight) * value2);
        }
    }
}
