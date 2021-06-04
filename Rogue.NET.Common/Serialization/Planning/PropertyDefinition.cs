using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Target;

using System;
using System.Reflection;

namespace Rogue.NET.Common.Serialization.Planning
{
    /// <summary>
    /// Used for pre-planning Deserialization
    /// </summary>
    internal class PropertyDefinition
    {
        public static PropertyDefinition CollectionElement = new PropertyDefinition(null)
        {
            PropertyName = "NOT A PROPERTY",
            PropertyType = new HashedType(typeof(object))
        };

        public static PropertyDefinition Empty = new PropertyDefinition(null)
        {
            PropertyName = "EMPTY",
            PropertyType = new HashedType(typeof(object))
        };

        readonly PropertyInfo _reflectionInfo;

        /// <summary>
        /// Create PropertyDefinition from reflected property. Set to NULL for user defined properties
        /// </summary>
        public PropertyDefinition(PropertyInfo reflectionInfo)
        {
            _reflectionInfo = reflectionInfo;
        }

        public string PropertyName { get; set; }
        public HashedType PropertyType { get; set; }
        public bool IsUserDefined { get; set; }

        public PropertyInfo GetReflectedInfo()
        {
            if (_reflectionInfo == null)
                throw new Exception("Trying to retrieve NULL REFLECTION INFO from PropertyDefinition");

            return _reflectionInfo;
        }

        public override bool Equals(object obj)
        {
            var definition = obj as PropertyDefinition;

            return definition.PropertyName.Equals(this.PropertyName) &&
                   definition.PropertyType.Equals(this.PropertyType) &&
                   definition.IsUserDefined.Equals(this.IsUserDefined);
        }

        public override int GetHashCode()
        {
            return this.CreateHashCode(this.PropertyName, this.PropertyType, this.IsUserDefined);
        }

        public override string ToString()
        {
            return this.FormatToString();
        }
    }
}
