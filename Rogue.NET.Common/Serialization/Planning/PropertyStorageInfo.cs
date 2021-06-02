using Rogue.NET.Common.Extension;

using System;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class PropertyStorageInfo
    {
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public object PropertyValue { get; set; }

        public override string ToString()
        {
            return this.FormatToString();
        }
    }
}
