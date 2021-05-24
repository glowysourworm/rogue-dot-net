
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rogue.NET.Common.Serialization
{
    /// <summary>
    /// Stores property data to an underlying record
    /// </summary>
    public class PropertyReader
    {
        // Keep track of property types to avoid extra reflection calls
        static Dictionary<Type, IEnumerable<PropertyInfo>> _propertyDict;

        // Actual object contextual properties
        Dictionary<string, PropertyStorageInfo> _properties;

        static PropertyReader()
        {
            _propertyDict = new Dictionary<Type, IEnumerable<PropertyInfo>>();
        }
        public PropertyReader()
        {
            _properties = new Dictionary<string, PropertyStorageInfo>();
        }

        public void Read<T>(string propertyName, T property)
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
            var propertyInfos = GetPropertyInfos(graphObject);

            return propertyInfos.Select(info => new PropertyStorageInfo()
            {
                PropertyName = info.Name,
                PropertyType = info.PropertyType,
                PropertyValue = info.GetValue(graphObject)
            });
        }

        /// <summary>
        /// NOTE*** ORDERED TO KEEP SERIALIZATION CONSISTENT!!!
        /// </summary>
        private IEnumerable<PropertyInfo> GetPropertyInfos(object graphObject)
        {
            if (!_propertyDict.ContainsKey(graphObject.GetType()))
            {
                // ORDER BY PROPERTY NAME
                _propertyDict.Add(graphObject.GetType(), graphObject.GetType()
                                                                    .GetProperties()
                                                                    .OrderBy(property => property.Name));
            }

            return _propertyDict[graphObject.GetType()];
        }
    }
}
