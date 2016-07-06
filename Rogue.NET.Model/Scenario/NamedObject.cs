using Microsoft.Practices.Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;

namespace Rogue.NET.Model.Scenario
{
    [Serializable]
    public abstract class NamedObject : FrameworkElement, INotifyPropertyChanged, ISerializable
    {
        string _id = System.Guid.NewGuid().ToString();

        public string RogueName { get; set; }
        public string Id { get { return _id; } }

        public NamedObject() { }
        public NamedObject(SerializationInfo info, StreamingContext context)
        {
            this.RogueName = info.GetString("RogueName");
            _id = info.GetString("Id");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("RogueName", this.RogueName);
            info.AddValue("Id", this.Id);
        }
    }
}
