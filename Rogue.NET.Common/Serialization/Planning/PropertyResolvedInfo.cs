using Rogue.NET.Common.Serialization.Target;

using System;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class PropertyResolvedInfo
    {
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
        public HashedObjectInfo ResolvedInfo { get; set; }
    }
}
