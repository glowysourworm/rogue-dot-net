using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializationObject : DeserializationObjectBase
    {
        // Object being constructed
        private object _defaultObject;

        internal DeserializationObject(HashedObjectReference reference, RecursiveSerializerMemberInfo memberInfo) : base(reference, memberInfo)
        {
            Construct();
        }

        protected override void Construct()
        {
            try
            {
                _defaultObject = this.MemberInfo.ParameterlessConstructor.Invoke(new object[] { });
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error trying to construct object of type {0}. Must have a parameterless constructor",
                                                  this.Reference.Type.TypeName), ex);
            }
        }

        protected override IEnumerable<PropertyDefinition> GetPropertyDefinitions(PropertyPlanner planner)
        {
            // No method Get
            if (this.MemberInfo.PlanningMethod == null)
                return planner.GetDefaultProperties(this.Reference.Type.Resolve());

            // CLEAR CURRENT CONTEXT
            planner.ClearContext();

            // CALL OBJECT'S GetPropertyDefinitions METHOD
            try
            {
                this.MemberInfo.PlanningMethod.Invoke(_defaultObject, new object[] { planner });
            }
            catch (Exception innerException)
            {
                throw new Exception("Error trying to read properties from " + this.Reference.Type.TypeName, innerException);
            }

            return planner.GetResult();
        }

        protected override void WriteProperties(PropertyReader reader)
        {
            // Set using reflection
            if (this.MemberInfo.SetMethod == null)
            {
                foreach (var property in reader.Properties)
                {
                    // Get property info for THIS type
                    var propertyInfo = this.Reference.Type.Resolve().GetProperty(property.PropertyName);

                    // Set property VALUE on our _defaultObject
                    propertyInfo.SetValue(_defaultObject, property.ResolvedInfo.TheObject);
                }
            }

            else
            {
                // CALL OBJECT'S SetProperties METHOD
                try
                {
                    this.MemberInfo.SetMethod.Invoke(_defaultObject, new object[] { reader });
                }
                catch (Exception innerException)
                {
                    throw new Exception("Error trying to set properties from " + this.Reference.Type.TypeName, innerException);
                }
            }
        }

        protected override HashedObjectInfo ResolveImpl()
        {
            return new HashedObjectInfo(_defaultObject, _defaultObject.GetType());
        }
    }
}
