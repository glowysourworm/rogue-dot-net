using Rogue.NET.Common.Extension;

using System;

namespace Rogue.NET.Common.Serialization.Planning
{
    /// <summary>
    /// Used for pre-planning Deserialization
    /// </summary>
    internal class PropertyDefinition
    {
        public static PropertyDefinition CollectionElement = new PropertyDefinition()
        {
            PropertyName = "NOT A PROPERTY",
            PropertyType = typeof(object)
        };

        public static PropertyDefinition Empty = new PropertyDefinition()
        {
            PropertyName = "EMPTY",
            PropertyType = typeof(object)
        };

        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public bool IsUserDefined { get; set; }

        public override string ToString()
        {
            return this.FormatToString();
        }
    }
}
