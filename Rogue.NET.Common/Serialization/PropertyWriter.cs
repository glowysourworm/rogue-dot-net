using Rogue.NET.Common.Serialization.Interface;
using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization
{
    public class PropertyWriter : IPropertyWriter
    {
        // Actual object contextual properties
        Dictionary<string, PropertyStorageInfo> _properties;

        internal PropertyWriter()
        {
            _properties = new Dictionary<string, PropertyStorageInfo>();
        }

        public void Write<T>(string propertyName, T property)
        {
            if (_properties.ContainsKey(propertyName))
                throw new ArgumentException("Property with the same name is already added to the reader:  " + propertyName);

            _properties.Add(propertyName, new PropertyStorageInfo()
            {
                PropertyName = propertyName,
                PropertyType = typeof(T),
                PropertyValue = property
            });
        }

        /// <summary>
        /// Returns entire list of properties
        /// </summary>
        internal IEnumerable<PropertyStorageInfo> GetResult()
        {
            return _properties.Values;
        }

        internal IEnumerable<PropertyStorageInfo> GetPropertiesReflection(Type type, object graphObject)
        {
            if (type != graphObject.GetType())
                throw new Exception("Type of object not equal to the implementation type:  " + type.FullName);

            var propertyInfos = RecursiveSerializerStore.GetOrderedProperties(type);

            _properties = propertyInfos.Select(info => new PropertyStorageInfo()
            {
                PropertyName = info.Name,
                PropertyType = info.PropertyType,
                PropertyValue = info.GetValue(graphObject)
            }).ToDictionary(info => info.PropertyName, info => info);

            return _properties.Values;
        }
    }
}
