using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Model.Scenario
{
    [Serializable]
    public class AttackAttribute : ScenarioObject
    {
        double _attack = 0;
        double _resistance = 0;
        int _weakness = 0;

        public double Attack 
        {
            get { return _attack; }
            set
            {
                if (_attack != value)
                {
                    _attack = value;
                    OnPropertyChanged("Attack");
                }
            }
        }
        public double Resistance
        {
            get { return _resistance; }
            set
            {
                if (_resistance != value)
                {
                    _resistance = value;
                    OnPropertyChanged("Resistance");
                }
            }
        }
        public int Weakness
        {
            get { return _weakness; }
            set
            {
                if (_weakness != value)
                {
                    _weakness = value;
                    OnPropertyChanged("Weakness");
                }
            }
        }
        public AttackAttribute()
        {
        }
        public AttackAttribute(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var properties = typeof(AttackAttribute).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }
        }

        public override string ToString()
        {
            return this.RogueName + " " + _attack + "|" + _resistance + "|" + _weakness;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(AttackAttribute).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var property in properties)
                info.AddValue(property.Name, property.GetValue(this));
        }
    }
}
