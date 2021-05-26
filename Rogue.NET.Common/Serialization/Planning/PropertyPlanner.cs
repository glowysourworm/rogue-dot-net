using System;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Planning
{
    /// <summary>
    /// Stores property definitions for Deserialization planning phase
    /// </summary>
    public class PropertyPlanner
    {
        // Actual object contextual properties
        Dictionary<string, PropertyDefinition> _properties;

        public PropertyPlanner()
        {
            _properties = new Dictionary<string, PropertyDefinition>();
        }

        /// <summary>
        /// Allows pre-defining properties for Deserialization planning
        /// </summary>
        public void Define(string propertyName, Type propertyType)
        {
            if (_properties.ContainsKey(propertyName))
                throw new ArgumentException("Property with the same name is already added to the planner:  " + propertyName);

            _properties.Add(propertyName, new PropertyDefinition()
            {
                PropertyName = propertyName,
                PropertyType = propertyType
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
        internal IEnumerable<PropertyDefinition> GetResult()
        {
            return _properties.Values;
        }

        /// <summary>
        /// Returns reflected public properties
        /// </summary>
        internal IEnumerable<PropertyDefinition> GetDefaultProperties(Type type)
        {
            var propertyInfos = RecursiveSerializerStore.GetOrderedProperties(type);

            return propertyInfos.Select(info => new PropertyDefinition()
            {
                PropertyName = info.Name,
                PropertyType = info.PropertyType
            });
        }
    }
}
