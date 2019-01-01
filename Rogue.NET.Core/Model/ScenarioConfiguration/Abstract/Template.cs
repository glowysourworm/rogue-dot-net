using System;
using System.ComponentModel;

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
