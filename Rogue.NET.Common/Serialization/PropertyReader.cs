
using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization
{
    /// <summary>
    /// Reads properties (from serialization backend) for the invoker
    /// </summary>
    public class PropertyReader
    {
        readonly IDictionary<string, PropertyResolvedInfo> _properties;

        internal PropertyReader(IEnumerable<PropertyResolvedInfo> properties)
        {
            _properties = properties.ToDictionary(property => property.PropertyName, property => property);
        }

        internal IEnumerable<PropertyResolvedInfo> Properties { get { return _properties.Values; } }

        public T Read<T>(string propertyName)
        {
            if (_properties.ContainsKey(propertyName))
                throw new ArgumentException("Property not present in the underlying stream:  " + propertyName);

            if (!_properties[propertyName].PropertyType.Equals(typeof(T)))
                throw new ArgumentException("Requested property type is invalid:  " + propertyName);

            return (T)_properties[propertyName].ResolvedInfo.TheObject;
        }
    }
}
