using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization
{
    public class PropertyWriter
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
        /// Clears current property context and begins for a new object to store data
        /// </summary>
        internal void ClearContext()
        {
            _properties.Clear();
        }

        /// <summary>
        /// Returns entire list of properties
        /// </summary>
        internal IEnumerable<PropertyStorageInfo> GetResult()
        {
            return _properties.Values;
        }

        internal IEnumerable<PropertyStorageInfo> GetPropertiesReflection(object graphObject)
        {
            var propertyInfos = RecursiveSerializerStore.GetOrderedProperties(graphObject.GetType());

            return propertyInfos.Select(info => new PropertyStorageInfo()
            {
                PropertyName = info.Name,
                PropertyType = info.PropertyType,
                PropertyValue = info.GetValue(graphObject)
            });
        }
    }
}
