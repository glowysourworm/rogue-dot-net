using Rogue.NET.Core.Model;
using Rogue.NET.Core.Model.Enums;
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
        /// Generates a Wigner semi-circle distribution on [0,R] by mapping points from [-R, 0] to
        /// [0, R]. 
        /// </summary>
        double GetWigner(double radius);

        /// <summary>
        /// Generates a triangle distribution on [start, end] - with peak of (end - start) / 2 (at "peak"). To simplify this
        /// to a linear distribution - just set the peak to be at the start or end.
        /// </summary>
        double GetTriangle(double start, double peak, double end);

        /// <summary>
        /// Generates Binomially distributed random integer on [0, exclusive upper bound) with specified mean
        /// </summary>
        public int GetBinomialRandomInteger(int exclusiveUpperBound, int mean);

        /// <summary>
        /// Gets random value from a Range<T> object
        /// </summary>
        T GetRandomValue<T>(Range<T> range) where T : IComparable<T>;

        /// <summary>
        /// Gets a random element from the collection
        /// </summary>
        T GetRandomElement<T>(IEnumerable<T> collection);

        /// <summary>
        /// Returns unique (up to reference comparison) random elements from the collection
        /// </summary>
        IEnumerable<T> GetDistinctRandomElements<T>(IEnumerable<T> collection, int count);

        /// <summary>
        /// Gets a random element from the collection with the supplied weight selector
        /// </summary>
        T GetWeightedRandom<T>(IEnumerable<T> collection, Func<T, double> weightSelector);

        /// <summary>
        /// Shuffles a collection and returns a new one with randomly placed elements
        /// </summary>
        IEnumerable<T> Randomize<T>(IEnumerable<T> collection);

        /// <summary>
        /// Gets random N, S, E, or W direction
        /// </summary>
        Compass GetRandomCardinalDirection();
    }
}
