using System;

namespace Rogue.NET.Core.Model
{
    [Serializable]
    public class Range<T> where T : IComparable<T>
    {
        public T Low { get; set; }
        public T High { get; set; }

        public Range() { }
        public Range(Range<T> copy)
        {
            this.Low = copy.Low;
            this.High = copy.High;
        }
        public Range(T low, T high)
        {
            this.Low = low;
            this.High = high;
        }

        public T GetAverage()
        {
            double low, high;
            low = Convert.ToDouble(this.Low);
            high = Convert.ToDouble(this.High);
            return (T)System.Convert.ChangeType(((low + high) / 2), typeof(T)); ;
        }
        public bool Contains(T item)
        {
            IComparable<T> comparable = item as IComparable<T>;
            int low = comparable.CompareTo(this.Low);
            int high = comparable.CompareTo(this.High);
            return (low >= 0) && (high <= 0);
        }
        /// <summary>
        /// returns true if either the High or Low values are set away from zero (default(T))
        /// </summary>
        /// <returns></returns>
        public bool IsSet()
        {
            return this.High.CompareTo(default(T)) != 0 || this.Low.CompareTo(default(T)) != 0;
        }
        public override string ToString()
        {
            return "From " + this.Low + " To " + this.High;
        }
    }
}