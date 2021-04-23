using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Model;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;
using Rogue.NET.Core.Math.Algorithm;
using System.Threading;
using System.Windows;
using System.Collections.Concurrent;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    /// <summary>
    /// MULTI-THREADING POLICY:  Assign sub-sequence to new thread based on the (int) Thread.Name property.  This should generate one random number sequence per
    /// level.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRandomSequenceGenerator))]
    public class RandomSequenceGenerator : IRandomSequenceGenerator
    {
        // Create these by spawning new Random instances each thread - starting with the first seed and incrementing by
        // one each time.
        private readonly ConcurrentDictionary<int, Random> _randomSequences;
        private int _seed;

        [ImportingConstructor]
        public RandomSequenceGenerator()
        {
            if (Thread.CurrentThread !=
                Application.Current.Dispatcher.Thread)
                throw new Exception("IRandomSequenceGenerator must be created on the main thread");

            _randomSequences = new ConcurrentDictionary<int, Random>();
        }

        public void Reseed(int seed)
        {
            if (Thread.CurrentThread !=
                Application.Current.Dispatcher.Thread)
                throw new Exception("IRandomSequenceGenerator.Reseed(...) method must be called from the main thread");

            _seed = seed;

            _randomSequences.Clear();
            _randomSequences.TryAdd(0, new Random(seed));
        }

        private Random GetOrAdd()
        {
            // ***MULTI-THREADING POLICY (Stores the primary sequence as the 0th entry, level sequences after that)
            var index = 0;
            var levelNumber = 0;

            // Find existing sequence for this thread / or spawn a new one
            if (int.TryParse(Thread.CurrentThread.Name, out levelNumber))
                index = levelNumber;

            if (!_randomSequences.ContainsKey(index))
                _randomSequences.TryAdd(index, new Random(_seed + levelNumber));

            return _randomSequences[index];
        }

        public double Get()
        {
            return GetOrAdd().NextDouble();
        }

        public int Get(int inclusiveLowerBound, int exclusiveUpperBound)
        {
            return GetOrAdd().Next(inclusiveLowerBound, exclusiveUpperBound);
        }

        public double GetDouble(double inclusiveLowerBound, double exclusiveUpperBound)
        {
            var slope = exclusiveUpperBound - inclusiveLowerBound;
            var intercept = inclusiveLowerBound;

            return (slope * GetOrAdd().NextDouble()) + intercept;
        }

        public Compass GetRandomCardinalDirection()
        {
            var random = GetOrAdd().NextDouble();

            if (random < 0.25)
                return Compass.N;

            else if (random < 0.5)
                return Compass.E;

            else if (random < 0.75)
                return Compass.S;

            else
                return Compass.W;
        }

        public double GetGaussian(double mean, double standardDeviation)
        {
            return (mean + (GetNormal() * standardDeviation)).Clip(0, 3.5 * standardDeviation);
        }

        public int GetBinomialRandomInteger(int exclusiveUpperBound, int mean)
        {
            if (!mean.Between(0, exclusiveUpperBound, false))
                throw new ArgumentException("Trying to generate exponential with flatness parameter NOT in [0, 1] RandomSequenceGenerator");

            // The Binomial distribution is simulated using a weighted random draw with the specified distribution
            //
            // https://en.wikipedia.org/wiki/Binomial_distribution
            //
            // E[X] = np = (exclusiveUpperBound - 1) * (probability of success per trial) => p = mean / (exclusiveUpperBound - 1)
            //

            var domain = Enumerable.Range(0, exclusiveUpperBound);
            var binomialPdf = domain.Select(integer => GetBinomialPDF(integer, exclusiveUpperBound - 1, mean / (double)(exclusiveUpperBound - 1)))
                                    .ToList();

            return GetWeightedRandom(domain, integer =>
            {
                return binomialPdf[integer];
            });
        }

        public T GetRandomValue<T>(Range<T> range) where T : IComparable<T>
        {
            var low = Convert.ToDouble(range.Low);
            var high = Convert.ToDouble(range.High);

            return (T)Convert.ChangeType((low + ((high - low) * GetOrAdd().NextDouble())), typeof(T));
        }

        public T GetRandomElement<T>(IEnumerable<T> collection)
        {
            // NOTE*** Random.NextDouble() is [1, 0) (exclusive upper bound)
            return !collection.Any() ? default(T) : collection.ElementAt((int)(collection.Count() * GetOrAdd().NextDouble()));
        }

        public IEnumerable<T> GetDistinctRandomElements<T>(IEnumerable<T> collection, int count)
        {
            if (count > collection.Count())
                throw new ArgumentException("Trying to draw too many elements from the collection RandomSequenceGenerator.GetDistinctRandomElements");

            // Start with a full collection and remove elements randomly until the count is reached
            //
            var list = new List<T>(collection);

            while (list.Count > count)
            {
                // Draw random index from the list
                var randomIndex = Get(0, list.Count);

                // Remove at that index ~ O(n)
                list.RemoveAt(randomIndex);
            }

            return list;
        }

        /// <summary>
        /// Selects a weighted random element from the sequence using the supplied weight selector
        /// </summary>
        public T GetWeightedRandom<T>(IEnumerable<T> collection, Func<T, double> weightSelector)
        {
            if (collection.Count() == 0)
                return default(T);

            if (collection.Count() == 1)
                return collection.First();

            // Generate weigths for each item
            var weightedItems = collection.Select(x => new { Item = x, Weight = weightSelector(x) });

            // Draw random number scaled by the sum of weights
            var randomDraw = GetOrAdd().NextDouble() * weightedItems.Sum(x => x.Weight);

            // Figure out which item corresponds to the random draw - treating each 
            // like a "bucket" of size "weight"

            var cummulativeSum = 0D;

            foreach (var item in weightedItems)
            {
                // Add onto sum - searching for the "bucket" that the 
                // random draw fell into.
                cummulativeSum += item.Weight;

                // Found the "bucket"
                if (cummulativeSum >= randomDraw)
                    return item.Item;
            }

            throw new Exception("PickRandom<T> has an issue searching using CDF method");
        }

        public IEnumerable<T> Randomize<T>(IEnumerable<T> collection)
        {
            var list = new List<T>();
            var indices = new List<int>(Enumerable.Range(0, collection.Count()));

            while (indices.Count > 0)
            {
                // Fetch a random index
                var randomIndex = GetRandomElement(indices);

                // Add random element to the result
                list.Add(collection.ElementAt(randomIndex));

                // Remove used index from indices
                indices.Remove(randomIndex);
            }

            return list;
        }

        // https://www.alanzucconi.com/2015/09/16/how-to-sample-from-a-gaussian-distribution/
        private double GetNormal()
        {
            double v1, v2, R;

            do
            {
                // Generate U[-1, 1] Variables
                v1 = (2.0 * GetOrAdd().NextDouble()) - 1.0;
                v2 = (2.0 * GetOrAdd().NextDouble()) - 1.0;

                // Calculate R^2
                R = v1 * v1 + v2 * v2;

            // Reject points outside the unit circle (RARE)
            } while (R >= 1.0f || R == 0f);

            // Use inverse CDF methods to calculate the gaussian
            return v1 * System.Math.Sqrt((-2.0 * System.Math.Log(R)) / R);
        }

        // https://stats.stackexchange.com/questions/403201/wigner-semi-circle-distribution-random-numbers-generation
        public double GetWigner(double radius)
        {
            return System.Math.Abs(radius * System.Math.Sqrt(GetOrAdd().NextDouble()) * System.Math.Cos(System.Math.PI * GetOrAdd().NextDouble()));
        }

        // https://en.wikipedia.org/wiki/Triangular_distribution
        public double GetTriangle(double start, double peak, double end)
        {
            var cutoff = (peak - start) / (end - start);

            var uniform = GetOrAdd().NextDouble();

            if (uniform < cutoff)
                return start + System.Math.Sqrt(uniform * (end - start) * (peak - start));

            else
                return end - System.Math.Sqrt((1 - uniform) * (end - start) * (end - peak));
        }

        /// <summary>
        /// Generates Binomial(n, p) where n is the upper bound, and p is the probabiliy parameter (of success) for the
        /// given independent Bernoulli trials.
        /// </summary>
        private double GetBinomialPDF(int domain, int upperBound, double parameter)
        {
            var n = upperBound;
            var k = domain;
            var p = parameter;

            var factorialN = FactorialAlgorithm.Run(n);
            var factorialK = FactorialAlgorithm.Run(k);
            var factorialNK = FactorialAlgorithm.Run(n - k);

            return (factorialN / (double)(factorialK * factorialNK)) * System.Math.Pow(p, k) * System.Math.Pow(1 - p, n - k);
        }
    }
}
