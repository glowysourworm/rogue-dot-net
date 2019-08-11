using Rogue.NET.Core.Model.Generator.Interface;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Rogue.NET.Core.Model.Generator
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
    }
}
