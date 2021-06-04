using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Serialization.Formatter;
using Rogue.NET.Common.Serialization.Target;
using Rogue.NET.Common.Serialization.Utility;

namespace Rogue.NET.Common.Serialization.Component
{
    internal class ObjectInfoResolver
    {
        // TRACK HASHED TYPES
        SimpleDictionary<HashedType, HashedType> _typeDict;

        internal ObjectInfoResolver()
        {
            _typeDict = new SimpleDictionary<HashedType, HashedType>();
        }

        internal SimpleDictionary<HashedType, HashedType> GetResolvedTypes()
        {
            return _typeDict;
        }

        /// <summary>
        /// CALCULATES OBJECT INFO FROM PROPERTY + PROPERTY HASHED TYPE. Throw exceptions for circular 
        /// references for REFERENCE TYPES. VAILDATES THE HASHED TYPE AGAINST THE OBJECT
        /// </summary>
        internal ObjectInfo Resolve(object theObject, HashedType theObjectType)
        {
            var isPrimitive = FormatterFactory.IsPrimitiveSupported(theObjectType.GetImplementingType());

            // PRIMITIVE NULL
            if (isPrimitive && ReferenceEquals(theObject, null))
                return TrackAndReturn(new ObjectInfo(theObjectType));

            // NULL
            if (theObject == null)
                return TrackAndReturn(new ObjectInfo(theObjectType));

            // PRIMITIVE
            if (isPrimitive)
                return TrackAndReturn(new ObjectInfo(theObject, theObjectType));

            // *** NOTE:  Trying to work with MSFT Type... So, just using this to catch things we 
            //            might have missed.
            //

            // DECLARING TYPE != IMPLEMENTING TYPE
            if (!theObject.GetType().Equals(theObjectType.GetDeclaringType()))
            {
                // WHAT TO DO?!
                //
                // 1) Re-create HashedType with actual implementing type
                // 2) Validate that the DECLARING type is assignable from the IMPLEMENTING type
                //
                // NOTE*** COVER ALL INHERITANCE BASES (INTERFACE, SUB-CLASS, ETC...)
                //

                // Validate assignability
                if (!theObjectType.GetDeclaringType().IsAssignableFrom(theObject.GetType()))
                    throw new RecursiveSerializerException(theObjectType, "Invalid property definition:  property DECLARING type is NOT ASSIGNABLE from the object IMPLEMENTING type");

                // Re-create HASHED TYPE
                var hashedType = new HashedType(theObjectType.GetDeclaringType(), theObject.GetType());

                // INTERFACE
                if (theObjectType.GetDeclaringType().IsInterface)
                {
                    return TrackAndReturn(new ObjectInfo(theObject, hashedType));
                }
                // SUB CLASS
                else if (theObject.GetType().IsSubclassOf(theObjectType.GetDeclaringType()))
                {
                    return TrackAndReturn(new ObjectInfo(theObject, hashedType));
                }
                else
                    throw new RecursiveSerializerException(theObjectType, "Unhandled polymorphic object type:  " + theObjectType.DeclaringType);
            }
            // DECLARING TYPE == IMPLEMENTING TYPE
            else
            {
                return TrackAndReturn(new ObjectInfo(theObject, theObjectType));
            }
        }

        private ObjectInfo TrackAndReturn(ObjectInfo objectInfo)
        {
            if (!_typeDict.ContainsKey(objectInfo.Type))
                _typeDict.Add(objectInfo.Type, objectInfo.Type);

            return objectInfo;
        }
    }
}
