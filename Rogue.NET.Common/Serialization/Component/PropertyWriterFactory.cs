using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Target;
using Rogue.NET.Common.Serialization.Utility;

using System;

namespace Rogue.NET.Common.Serialization.Component
{
    internal static class PropertyWriterFactory
    {
        /// <summary>
        /// Creates a PropertyWriter and resolves properties according to the serialization object. Returned writer can then
        /// be used to query for the property specification and property resolved info collection.
        /// </summary>
        internal static PropertyWriter CreateAndResolve(ObjectInfoResolver resolver, SerializationObjectBase objectBase)
        {
            if (objectBase is SerializationNullObject ||
                objectBase is SerializationNullPrimitive ||
                objectBase is SerializationPrimitive ||
                objectBase is SerializationReference)
                throw new Exception("Trying to resolve properties using PropertyWriter for non-reference serialization object:  PropertyWriterFactory.cs");

            var writer = new PropertyWriter(resolver, objectBase.ObjectInfo.Type);

            // DEFAULT MODE -> NO SUPPORT FOR COLLECTIONS!
            if (objectBase.Mode == SerializationMode.Default)
            {
                // Initialization complete for collections
                if (objectBase is SerializationCollection)
                    return writer;

                // Initialize writer using reflection
                writer.ReflectProperties(objectBase.ObjectInfo);
            }

            // SPECIFIED MODE
            else
            {
                try
                {
                    objectBase.MemberInfo.GetMethod.Invoke(objectBase.ObjectInfo.GetObject(), new object[] { writer });
                }
                catch (Exception innerException)
                {
                    throw new RecursiveSerializerException(objectBase.ObjectInfo.Type, "Error trying to read properties", innerException);
                }
            }

            return writer;
        }
    }
}
