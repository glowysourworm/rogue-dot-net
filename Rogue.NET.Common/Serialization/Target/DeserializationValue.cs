using Rogue.NET.Common.Serialization.Component;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Utility;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializationValue : DeserializationObjectBase
    {
        // Object being constructed
        private object _defaultObject;

        // Property definitions
        PropertySpecification _specification;

        internal DeserializationValue(ObjectReference reference, RecursiveSerializerMemberInfo memberInfo, PropertySpecification specification) : base(reference, memberInfo)
        {
            _specification = specification;
        }

        internal override PropertySpecification GetPropertySpecification()
        {
            return _specification;
        }

        internal override void Construct(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            switch (this.MemberInfo.Mode)
            {
                case SerializationMode.Default:
                    ConstructDefault(resolvedProperties);
                    break;
                case SerializationMode.Specified:
                    ConstructSpecified(resolvedProperties);
                    break;
                default:
                    throw new Exception("Unhandled SerializationMode type:  DeserializationValue.cs");
            }
        }

        private void ConstructDefault(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            // CONSTRUCT
            try
            {
                _defaultObject = this.MemberInfo.ParameterlessConstructor.Invoke(new object[] { });
            }
            catch (Exception ex)
            {
                throw new RecursiveSerializerException(this.Reference.Type, "Error constructing from parameterless constructor", ex);
            }

            // SET PROPERTIES
            try
            {
                foreach (var property in resolvedProperties)
                {
                    if (property.IsUserDefined)
                        throw new RecursiveSerializerException(property.ResolvedInfo.Type, "Trying to set user defined property using DEFAULT mode");

                    // LOCATE PROPERTY INFO
                    var propertyDefinition = _specification.GetHashedDefinition(property.PropertyName, property.ResolvedInfo.Type);

                    // SAFE TO CALL GetPropertyInfo() and GetObject() 
                    propertyDefinition.GetReflectedInfo()
                                      .SetValue(_defaultObject, property.ResolvedInfo.GetObject());
                }
            }
            catch (Exception ex)
            {
                throw new RecursiveSerializerException(this.Reference.Type, "Error constructing object from properties", ex);
            }
        }

        private void ConstructSpecified(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            var reader = new PropertyReader(resolvedProperties);

            try
            {
                _defaultObject = this.MemberInfo.SpecifiedConstructor.Invoke(new object[] { reader });
            }
            catch (Exception ex)
            {
                throw new RecursiveSerializerException(this.Reference.Type, "Error constructing from specified constructor", ex);
            }
        }

        protected override ObjectInfo ProvideResult()
        {
            return new ObjectInfo(_defaultObject, new HashedType(this.Reference.Type.GetDeclaringType(), _defaultObject.GetType()));
        }
    }
}
