using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Model;
using Rogue.NET.Common.Extension;
using Rogue.NET.Core.Model.Enums;

namespace Rogue.NET.Core.Processing.Model.Generator
{
    [PartCreationPolicy(CreationPolicy.Shared)]
    [Export(typeof(IRandomSequenceGenerator))]
    public class RandomSequenceGenerator : IRandomSequenceGenerator
    {
        private Random _random;

        [ImportingConstructor]
        public RandomSequenceGenerator()
        {
            _random = new Random(1);
        }

        public void Reseed(int seed)
        {
            _random = new Random(seed);
        }

        public double Get()
        {
            return _random.NextDouble();
        }

        public int Get(int inclusiveLowerBound, int exclusiveUpperBound)
        {
            return _random.Next(inclusiveLowerBound, exclusiveUpperBound);
        }

        public double GetDouble(double inclusiveLowerBound, double exclusiveUpperBound)
        {
            var slope = exclusiveUpperBound - inclusiveLowerBound;
            var intercept = inclusiveLowerBound;

            return (slope * _random.NextDouble()) + intercept;
        }

        public Compass GetRandomCardinalDirection()
        {
            var random = _random.NextDouble();

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

        public double GetExponential(double rate)
        {
            if (rate <= 0)
                throw new ArgumentException("Trying to generate exponential with rate <= 0 RandomSequenceGenerator");

            return -1 * System.Math.Log(_random.NextDouble()) / rate;
        }

        public T GetRandomValue<T>(Range<T> range) where T : IComparable<T>
        {
            var low = Convert.ToDouble(range.Low);
            var high = Convert.ToDouble(range.High);

            return (T)Convert.ChangeType((low + ((high - low) * _random.NextDouble())), typeof(T));
        }

        public T GetRandomElement<T>(IEnumerable<T> collection)
        {
            // NOTE*** Random.NextDouble() is [1, 0) (exclusive upper bound)
            return !collection.Any() ? default(T) : collection.ElementAt((int)(collection.Count() * _random.NextDouble()));
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
            var randomDraw = _random.NextDouble() * weightedItems.Sum(x => x.Weight);

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

        // https://www.alanzucconi.com/2015/09/16/how-to-sample-from-a-gaussian-distribution/
        private double GetNormal()
        {
            double v1, v2, R;

            do
            {
                // Generate U[-1, 1] Variables
                v1 = (2.0 * _random.NextDouble()) - 1.0;
                v2 = (2.0 * _random.NextDouble()) - 1.0;

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
            return System.Math.Abs(radius * System.Math.Sqrt(_random.NextDouble()) * System.Math.Cos(System.Math.PI * _random.NextDouble()));
        }

        // https://en.wikipedia.org/wiki/Triangular_distribution
        public double GetTriangle(double start, double peak, double end)
        {
            var cutoff = (peak - start) / (end - start);

            var uniform = _random.NextDouble();

            if (uniform < cutoff)
                return start + System.Math.Sqrt(uniform * (end - start) * (peak - start));

            else
                return end - System.Math.Sqrt((1 - uniform) * (end - start) * (end - peak));
        }
    }
}
