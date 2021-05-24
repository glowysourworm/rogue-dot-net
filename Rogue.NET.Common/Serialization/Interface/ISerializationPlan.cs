using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;

using System.Collections.Generic;

namespace Rogue.NET.Common.Serialization.Interface
{
    internal interface ISerializationPlan
    {
        IDictionary<HashedObjectInfo, SerializationNodeBase> UniqueReferenceDict { get; }

        SerializationNode RootNode { get; }
    }
}
