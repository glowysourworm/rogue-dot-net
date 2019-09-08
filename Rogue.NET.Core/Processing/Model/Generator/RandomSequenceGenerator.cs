using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rogue.NET.Core.Processing.Model.Generator.Interface;
using Rogue.NET.Core.Model;

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

        public int CalculateGenerationNumber(double generationNumber)
        {
            var truncatedNumber = (int)generationNumber;

            var truncatedRemainder = generationNumber - truncatedNumber;

            if (truncatedRemainder > 0 && truncatedRemainder > _random.NextDouble())
                truncatedNumber++;

            return truncatedNumber;
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
    }
}
