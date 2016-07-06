using Microsoft.Practices.Prism.Events;
using Rogue.NET.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Rogue.NET.Model.Scenario
{
    [Serializable]
    public class BehaviorDetails : NamedObject
    {
        bool _secondaryBehavior = false;
        public Behavior CurrentBehavior
        {
            get
            {
                if (this.SecondaryReason == SecondaryBehaviorInvokeReason.SecondaryNotInvoked)
                    return this.PrimaryBehavior;

                else
                    return _secondaryBehavior ? this.SecondaryBehavior : this.PrimaryBehavior; 
            }
        }
        public Behavior PrimaryBehavior { get; set; }
        public Behavior SecondaryBehavior { get; set; }
        public SecondaryBehaviorInvokeReason SecondaryReason { get; set; }
        public double SecondaryProbability { get; set; }

        public BehaviorDetails() { }
        public BehaviorDetails(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            var properties = typeof(BehaviorDetails).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
            {
                var val = info.GetValue(property.Name, property.PropertyType);
                property.SetValue(this, val);
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            var properties = typeof(BehaviorDetails).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(z => z.GetSetMethod() != null);
            foreach (var property in properties)
                info.AddValue(property.Name, property.GetValue(this));
        }

        public void SetPrimaryInvoked()
        {
            if (this.SecondaryReason == SecondaryBehaviorInvokeReason.PrimaryInvoked)
                _secondaryBehavior = true;
        }
        public void RollRandomBehavior(Random r)
        {
            if (this.SecondaryReason == SecondaryBehaviorInvokeReason.Random && r.NextDouble() < this.SecondaryProbability)
                _secondaryBehavior = !_secondaryBehavior;

        }
    }
}
