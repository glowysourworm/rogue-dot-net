using Rogue.NET.Common.Serialization.Component;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Utility;

using System;
using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializedObjectNode : DeserializedNodeBase
    {
        internal List<DeserializedNodeBase> SubNodes { get; private set; }

        /// <summary>
        /// ID FROM SERIALIZATION PROCEDURE
        /// </summary>
        internal int ReferenceId { get; private set; }

        // Property definitions
        PropertySpecification _specification;

        // Object being constructed
        private object _defaultObject;

        internal DeserializedObjectNode(PropertyDefinition definition, HashedType type, int referenceId, RecursiveSerializerMemberInfo memberInfo, PropertySpecification specification) : base(definition, type, memberInfo)
        {
            this.ReferenceId = referenceId;
            this.SubNodes = new List<DeserializedNodeBase>();

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
                throw new RecursiveSerializerException(this.Type, "Error constructing from parameterless constructor", ex);
            }

            // SET PROPERTIES
            try
            {
                foreach (var property in resolvedProperties)
                {
                    if (property.IsUserDefined)
                        throw new RecursiveSerializerException(property.ResolvedType, "Trying to set user defined property using DEFAULT mode");

                    // LOCATE PROPERTY INFO
                    var propertyDefinition = _specification.GetHashedDefinition(property.PropertyName, property.ResolvedType);

                    // SAFE TO CALL GetPropertyInfo() and GetObject() 
                    propertyDefinition.GetReflectedInfo()
                                      .SetValue(_defaultObject, property.ResolvedObject);
                }
            }
            catch (Exception ex)
            {
                throw new RecursiveSerializerException(this.Type, "Error constructing object from properties", ex);
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
                throw new RecursiveSerializerException(this.Type, "Error constructing from specified constructor", ex);
            }
        }

        public override bool Equals(object obj)
        {
            var node = obj as DeserializedObjectNode;

            return this.ReferenceId == node.ReferenceId;
        }
        public override int GetHashCode()
        {
            return this.ReferenceId;
        }

        protected override object ProvideResult()
        {
            return _defaultObject;
        }
    }
}
