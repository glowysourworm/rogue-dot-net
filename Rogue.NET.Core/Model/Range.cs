using System;

namespace Rogue.NET.Core.Model
{
    [Serializable]
    public class Range<T> where T : IComparable<T>
    {
        public T Low { get; set; }
        public T High { get; set; }
        public T LowLimit { get; set; }
        public T HighLimit { get; set; }

        public Range() { }
        public Range(Range<T> copy)
        {
            this.Low = copy.Low;
            this.High = copy.High;
            this.LowLimit = copy.LowLimit;
            this.HighLimit = copy.HighLimit;
        }
        public Range(T low, T high)
        {
            this.LowLimit = low;
            this.HighLimit = high;
            this.Low = low;
            this.High = high;
        }
        public Range(T lowlim, T low, T high, T highlim)
        {
            this.LowLimit = lowlim;
            this.Low = low;
            this.High = high;
            this.HighLimit = highlim;
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
        public bool Validate()
        {
            if (this.LowLimit.CompareTo(this.HighLimit) > 1)
                return false;

            if (this.Low.CompareTo(this.LowLimit) < 0)
                return false;

            if (this.High.CompareTo(this.HighLimit) > 0)
                return false;

            if (this.Low.CompareTo(this.High) > 0)
                return false;

            return true;
        }
        public bool ValidateLow()
        {
            if (this.Low.CompareTo(this.LowLimit) < 0)
                return false;

            if (this.Low.CompareTo(this.High) > 0)
                return false;

            return true;
        }
        public bool ValidateHigh()
        {
            if (this.High.CompareTo(this.HighLimit) > 0)
                return false;

            if (this.Low.CompareTo(this.High) > 0)
                return false;

            return true;
        }
        public override string ToString()
        {
            return "From " + this.Low + " To " + this.High;
        }
    }
}