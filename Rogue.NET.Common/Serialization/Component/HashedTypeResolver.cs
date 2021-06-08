using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Serialization.Formatter;
using Rogue.NET.Common.Serialization.Target;
using Rogue.NET.Common.Serialization.Utility;

using System;

namespace Rogue.NET.Common.Serialization.Component
{
    internal class HashedTypeResolver
    {
        // TRACK HASHED TYPES
        SimpleDictionary<int, HashedType> _typeDict;

        internal HashedTypeResolver()
        {
            _typeDict = new SimpleDictionary<int, HashedType>();
        }

        internal SimpleDictionary<int, HashedType> GetResolvedTypes()
        {
            return _typeDict;
        }

        internal HashedType Resolve(object theObject, Type declaringType)
        {
            // PERFORMANCE: Try to prevent creating new hashed type
            var hashCode = HashedType.CalculateHashCode(declaringType, theObject.GetType());

            if (_typeDict.ContainsKey(hashCode))
                return _typeDict[hashCode];

            return Resolve(theObject, new HashedType(declaringType));
        }

        /// <summary>
        /// Calculate object hashed type from the object and from it's property type. VALIDATES TYPE!
        /// </summary>
        internal HashedType Resolve(object theObject, HashedType theObjectType)
        {
            var isPrimitive = FormatterFactory.IsPrimitiveSupported(theObjectType.GetImplementingType());

            // PRIMITIVE NULL
            if (isPrimitive && ReferenceEquals(theObject, null))
                return CreateDeclaringType(theObjectType);

            // NULL
            if (theObject == null)
                return CreateDeclaringType(theObjectType);

            // PRIMITIVE
            if (isPrimitive)
                return CreateDeclaringAndImplementingType(theObject, theObjectType);

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
                var hashedType = CreateDeclaringAndImplementingType(theObject, theObjectType);

                // INTERFACE (VALIDATING)
                if (theObjectType.GetDeclaringType().IsInterface)
                {
                    return hashedType;
                }
                // SUB CLASS (VALIDATING)
                else if (theObject.GetType().IsSubclassOf(theObjectType.GetDeclaringType()))
                {
                    return hashedType;
                }
                else
                    throw new RecursiveSerializerException(theObjectType, "Unhandled polymorphic object type:  " + theObjectType.DeclaringType);
            }
            // DECLARING TYPE == IMPLEMENTING TYPE
            else
            {
                return CreateDeclaringAndImplementingType(theObject, theObjectType);
            }
        }

        private HashedType CreateDeclaringType(HashedType hashedType)
        {
            var hashCode = hashedType.GetHashCode();

            if (!_typeDict.ContainsKey(hashCode))
                _typeDict.Add(hashedType.GetHashCode(), hashedType);

            return _typeDict[hashCode];
        }

        private HashedType CreateDeclaringAndImplementingType(object theObject, HashedType theObjectType)
        {
            // PERFORMANCE: Try to prevent creating new hashed type
            var hashCode = HashedType.CalculateHashCode(theObjectType.GetDeclaringType(), theObject.GetType());

            if (!_typeDict.ContainsKey(hashCode))
                _typeDict.Add(hashCode, new HashedType(theObjectType.GetDeclaringType(), theObject.GetType()));

            return _typeDict[hashCode];
        }
    }
}
