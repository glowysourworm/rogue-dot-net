using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Rogue.NET.Core.Model.ScenarioConfiguration.Abstract
{
    [Serializable]
    public class Template : INotifyPropertyChanged
    {
        public Template()
        {
            this.Name = "New Template";
            this.Guid = System.Guid.NewGuid().ToString();
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        private string _name;
        private string _guid;

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        public string Guid
        {
            get { return _guid; }
            set
            {
                if (_guid != value)
                {
                    _guid = value;
                    OnPropertyChanged("Guid");
                }
            }
        }

        protected virtual void RaiseAndSetIfChanged<T>(ref T field, T value, [CallerMemberName] string memberName = "")
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
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }
    }
}
