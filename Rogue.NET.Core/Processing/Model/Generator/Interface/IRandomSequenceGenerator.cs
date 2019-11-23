using Rogue.NET.Core.Model;
using System;
using System.Collections.Generic;

namespace Rogue.NET.Core.Processing.Model.Generator.Interface
{
    /// <summary>
    /// Component to provide singleton random sequence
    /// </summary>
    public interface IRandomSequenceGenerator
    {
        /// <summary>
        /// Creates new sequence with specified seed
        /// </summary>
        void Reseed(int seed);

        /// <summary>
        /// Generates a U[0,1) random number
        /// </summary>
        double Get();

        /// <summary>
        /// Generates U[low, high) random integer
        /// </summary>
        int Get(int inclusiveLowerBound, int exclusiveUpperBound);

        /// <summary>
        /// Generates U[low, high) random double
        /// </summary>
        double GetDouble(double inclusiveLowerBound, double exclusiveUpperBound);

        /// <summary>
        /// Generates Gaussian distributed random number with the provided mean and std. deviation 
        /// using the Marsaglia polar method. Clips anything outside 3.5 std deviations.
        /// </summary>
        double GetGaussian(double mean, double standardDeviation);

        /// <summary>
        /// Gets random value from a Range<T> object
        /// </summary>
        T GetRandomValue<T>(Range<T> range) where T : IComparable<T>;

        /// <summary>
        /// Gets a random element from the collection
        /// </summary>
        T GetRandomElement<T>(IEnumerable<T> collection);

        /// <summary>
        /// Gets a random element from the collection with the supplied weight selector
        /// </summary>
        T GetWeightedRandom<T>(IEnumerable<T> collection, Func<T, double> weightSelector);
    }
}
