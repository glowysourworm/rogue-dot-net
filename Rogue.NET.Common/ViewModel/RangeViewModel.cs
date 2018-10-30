using System;
using System.ComponentModel;

// MUST LEAVE NAMESPACE Rogue.NET.Common for BinaryFormatter
namespace Rogue.NET.Common.ViewModel
{
    [Serializable]
    public class Range<T> : INotifyPropertyChanged where T : IComparable
    {
        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        T _low = default(T);
        T _high = default(T);
        T _lowlim = default(T);
        T _highlim = default(T);

        public T Low
        {
            get { return _low; }
            set
            {
                _low = value;
                OnPropertyChanged("Low");
            }
        }
        public T High
        {
            get { return _high; }
            set
            {
                _high = value;
                OnPropertyChanged("High");
            }
        }
        public T LowLimit
        {
            get { return _lowlim; }
            set
            {
                _lowlim = value;
                OnPropertyChanged("LowLimit");
            }
        }
        public T HighLimit
        {
            get { return _highlim; }
            set
            {
                _highlim = value;
                OnPropertyChanged("HighLimit");
            }
        }

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
        /// <summary>
        /// Gets Random value in interval
        /// </summary>
        /// <exception cref="InvalidCastException">If Can't cast template</exception>
        public T GetRandomValue(ref Random r)
        {
            double low, high;
            low = Convert.ToDouble(this.Low);
            high = Convert.ToDouble(this.High);
            Type t = typeof(T);
            double d = r.NextDouble();
            object o = System.Convert.ChangeType((low + ((high - low) * d)),typeof(T));
            return (T)o;
        }
        public T GetAverage()
        {
            double low, high;
            low = Convert.ToDouble(this.Low);
            high = Convert.ToDouble(this.High);
            object o = System.Convert.ChangeType(((low + high) / 2), typeof(T));
            return (T)o;
        }
        public bool Contains(T item)
        {
            if (item is IComparable<T>)
            {
                IComparable<T> comparable = item as IComparable<T>;
                int low = comparable.CompareTo(this.Low);
                int high = comparable.CompareTo(this.High);
                return (low >= 0) && (high <= 0);
            }
            return false;
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