using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Component.Interface;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;
using Rogue.NET.Common.Serialization.Utility;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Component
{
    public class PropertyWriter : IPropertyWriter
    {
        // Actual object contextual properties
        SimpleDictionary<string, PropertyResolvedInfo> _properties;

        // Object type for this writer
        HashedType _objectType;

        // RESOLVER FOR OBJECT INFO + HASHED TYPE RULES!
        readonly ObjectInfoResolver _resolver;

        internal PropertyWriter(ObjectInfoResolver resolver, HashedType objectType)
        {
            _objectType = objectType;
            _properties = new SimpleDictionary<string, PropertyResolvedInfo>();
            _resolver = resolver;
        }

        public void Write<T>(string propertyName, T property)
        {
            if (_properties.ContainsKey(propertyName))
                throw new ArgumentException("Property with the same name is already added to the reader:  " + propertyName);

            // Create the HASH TYPE from the template "T" and the implementing property type
            var hashedType = new HashedType(typeof(T), property.GetType());

            _properties.Add(propertyName, new PropertyResolvedInfo(null)
            {
                PropertyName = propertyName,
                ResolvedInfo = ResolveObjectInfo(property, hashedType),
                IsUserDefined = true
            });
        }

        /// <summary>
        /// Returns entire list of properties
        /// </summary>
        internal IEnumerable<PropertyResolvedInfo> GetProperties()
        {
            return _properties.Values;
        }

        internal PropertySpecification GetPropertySpecification()
        {
            // BUILD SPECIFICATION
            return new PropertySpecification(_objectType,
                _properties.Values
                           .Select(info =>
                           {
                               // NULL for user-defined properties
                               return new PropertyDefinition(info.IsUserDefined ? null : info.GetReflectedInfo())
                               {
                                   IsUserDefined = info.IsUserDefined,
                                   PropertyName = info.PropertyName,
                                   PropertyType = info.ResolvedInfo.Type
                               };
                           }));
        }

        internal void ReflectProperties(ObjectInfo objectInfo)
        {
            if (objectInfo.GetObject() == null)
                throw new RecursiveSerializerException(objectInfo.Type, "Trying to reflect properties on a null object");

            // FETCH SPECIFICATION (BASED ON HASHED TYPE ONLY!)
            var specification = RecursiveSerializerStore.GetOrderedProperties(objectInfo.Type);

            // RESOLVE USING REFLECTION
            _properties = specification.Definitions.Select(definition => new PropertyResolvedInfo(definition.GetReflectedInfo())
            {
                PropertyName = definition.PropertyName,
                ResolvedInfo = ResolveObjectInfo(definition.GetReflectedInfo()
                                                           .GetValue(objectInfo.GetObject()),   // Use Reflection
                                                 definition.PropertyType),
                IsUserDefined = false

            }).ToSimpleDictionary(info => info.PropertyName, info => info);
        }

        private ObjectInfo ResolveObjectInfo(object theObject, HashedType theObjectType)
        {
            // Catch "Backend" exception to hide from user 
            try
            {
                // Validate the object info
                return _resolver.Resolve(theObject, theObjectType);
            }
            catch (Exception innerException)
            {
                throw new RecursiveSerializerException(theObjectType, "Error writing property for type " + theObjectType.DeclaringType, innerException);
            }
        }
    }
}
