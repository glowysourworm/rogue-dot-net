using System;

namespace Rogue.NET.Common.Serialization
{
    internal class PropertyStorageInfo
    {
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public object PropertyValue { get; set; }
    }
}
