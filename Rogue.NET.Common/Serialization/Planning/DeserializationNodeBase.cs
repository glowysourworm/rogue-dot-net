using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Target;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal abstract class DeserializationNodeBase
    {
        /// <summary>
        /// Object being built for this node
        /// </summary>
        public DeserializationObjectBase NodeObject { get; private set; }

        /// <summary>
        /// Definition of the public (or custom) property represented by this node
        /// </summary>
        public PropertyDefinition Property { get; private set; }

        public DeserializationNodeBase(DeserializationObjectBase nodeObject, PropertyDefinition definition)
        {
            this.NodeObject = nodeObject;
            this.Property = definition;
        }

        public override string ToString()
        {
            return this.FormatToString();
        }
    }
}
