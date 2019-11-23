namespace Rogue.NET.Core.Math.Algorithm.Interface
{
    public interface INoiseGenerator
    {
        /// <summary>
        /// Specifies the type of noise to generate
        /// </summary>
        public enum NoiseType
        {
            /// <summary>
            /// Creates U[0,1] white noise - using the frequency parameter as a threshold
            /// </summary>
            WhiteNoise,

            /// <summary>
            /// Creates Perlin noise using the frequency parameter as a ratio of the total map size to create
            /// the noise mesh of random vectors
            /// </summary>
            PerlinNoise
        }

        /// <summary>
        /// Delegate for a method that is used for processing a grid based on the 2D noise array. Allows for filtering
        /// of the values.
        /// </summary>
        /// <param name="column">The corresponding column index (FROM 0) for the underlying grid</param>
        /// <param name="row">The corresponding row index (FROM 0) for the underlying grid</param>
        /// <param name="value">The unfiltered, normalized result for the noise function</param>
        delegate double PostProcessingCallback(int column, int row, double value);

        /// <summary>
        /// Generates a noise map given the input parameters (See Initialize(...)). The callback is used to consume
        /// the noise map value during iteration.
        /// </summary>
        /// <param name="width">Width of the noise grid</param>
        /// <param name="height">Height of the noise grid</param>
        /// <param name="frequency">Frequency parameter for the noise</param>
        double[,] Run(NoiseType type, int width, int height, double frequency, PostProcessingCallback callback);
    }
}
