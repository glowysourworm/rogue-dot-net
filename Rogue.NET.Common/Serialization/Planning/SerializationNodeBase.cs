using Rogue.NET.Common.Serialization.Target;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal abstract class SerializationNodeBase
    {
        public SerializationObjectBase NodeObject { get; private set; }

        public SerializationNodeBase(SerializationObjectBase nodeObject)
        {
            this.NodeObject = nodeObject;
        }
    }
}
