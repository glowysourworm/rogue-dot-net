using Rogue.NET.ScenarioEditor.Utility.Undo;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rogue.NET.ScenarioEditor.ViewModel.ScenarioConfiguration
{
    public class RangeViewModel<T> : INotifyPropertyChanged, INotifyPropertyChanging, IComparable where T : IComparable
    {
        T _low = default(T);
        T _high = default(T);

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

        public RangeViewModel() { }
        public RangeViewModel(RangeViewModel<T> copy)
        {
            this.Low = copy.Low;
            this.High = copy.High;
        }
        public RangeViewModel(T low, T high)
        {
            this.Low = low;
            this.High = high;
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
        public override string ToString()
        {
            return "From " + this.Low + " To " + this.High;
        }

        public int CompareTo(object obj)
        {
            var rangeViewModel = obj as RangeViewModel<T>;
            if (rangeViewModel == null)
                return 0;

            return rangeViewModel.Low.CompareTo(this.Low);
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