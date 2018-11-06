using Rogue.NET.ScenarioEditor.Utility.Undo;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration
{
    public class RangeViewModel<T> : INotifyPropertyChanged, INotifyPropertyChanging where T : IComparable
    {
        T _low = default(T);
        T _high = default(T);
        T _lowlim = default(T);
        T _highlim = default(T);

        public T Low
        {
            get { return _low; }
            set { this.RaiseAndSetIfChanged(ref _low, value); }
        }
        public T High
        {
            get { return _high; }
            set { this.RaiseAndSetIfChanged(ref _high, value); }
        }
        public T LowLimit
        {
            get { return _lowlim; }
            set { this.RaiseAndSetIfChanged(ref _lowlim, value); }
        }
        public T HighLimit
        {
            get { return _highlim; }
            set { this.RaiseAndSetIfChanged(ref _highlim, value); }
        }

        public RangeViewModel() { }
        public RangeViewModel(RangeViewModel<T> copy)
        {
            this.Low = copy.Low;
            this.High = copy.High;
            this.LowLimit = copy.LowLimit;
            this.HighLimit = copy.HighLimit;
        }
        public RangeViewModel(T low, T high)
        {
            this.LowLimit = low;
            this.HighLimit = high;
            this.Low = low;
            this.High = high;
        }
        public RangeViewModel(T lowlim, T low, T high, T highlim)
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

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected virtual void RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string memberName = "")
        {
            var changed = false;
            if (field == null)
                changed = value != null;
            else
                changed = !field.Equals(value);

            if (changed)
            {
                // Use the Id to relate the two events
                var eventArgs = new UndoPropertyChangingEventArgs(memberName);
                if (PropertyChanging != null)
                    PropertyChanging(this, eventArgs);

                field = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new UndoPropertyChangedEventArgs(eventArgs.Id, memberName));
            }
        }
    }
}