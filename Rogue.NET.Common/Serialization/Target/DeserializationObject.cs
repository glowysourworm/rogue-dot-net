using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializationObject : DeserializationObjectBase
    {
        // Object being constructed
        private object _defaultObject;

        // Property definitions
        private IEnumerable<PropertyDefinition> _definitions;

        internal DeserializationObject(HashedObjectReference reference, RecursiveSerializerMemberInfo memberInfo, IEnumerable<PropertyDefinition> definitions) : base(reference, memberInfo)
        {
            _definitions = definitions;
        }

        internal override IEnumerable<PropertyDefinition> GetPropertyDefinitions()
        {
            return _definitions;
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
                case SerializationMode.None:
                default:
                    throw new Exception("Unhandled SerializationMode type:  DeserializationObject.cs");
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
                var allProperties = RecursiveSerializerStore.GetOrderedProperties(this.Reference.Type.GetImplementingType());

                foreach (var property in resolvedProperties)
                {
                    // LOCATE PROPERTY INFO
                    var propertyInfo = allProperties.FirstOrDefault(info => info.Name == property.PropertyName && 
                                                                            info.PropertyType.IsAssignableFrom(property.PropertyType));

                    if (propertyInfo == null)
                        throw new Exception("Error locating property info:  " + propertyInfo.Name);

                    // SAFE TO CALL GetObject() 
                    propertyInfo.SetValue(_defaultObject, property.ResolvedInfo.GetObject());
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

        protected override HashedObjectInfo ProvideResult()
        {
            return new HashedObjectInfo(_defaultObject, _defaultObject.GetType());
        }
    }
}
