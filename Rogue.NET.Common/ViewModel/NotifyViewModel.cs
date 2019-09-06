using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Rogue.NET.Common.ViewModel
{
    public abstract class NotifyViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        protected void OnPropertyChanged<T>(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression != null)
                OnPropertyChanged(memberExpression.Member.Name);

            else
                throw new Exception("Invalid Member Expression OnPropertyChanged<T>");
        }

        /// <summary>
        /// Raised INotifyPropertyChanged event if there's a change to the property. Returns true if there was
        /// a change
        /// </summary>
        protected virtual bool RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string memberName = "")
        {
            var changed = false;
            if (field == null)
                changed = value != null;
            else
                changed = !field.Equals(value);

            if (changed)
            {
                field = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(memberName));
            }

            return changed;
        }
    }
}
