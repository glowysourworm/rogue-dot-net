using Rogue.NET.Common.Serialization.Component;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Utility;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializationCollection : DeserializationObjectBase
    {
        internal int Count { get { return _count; } }
        internal CollectionInterfaceType InterfaceType { get { return _interfaceType; } }

        // Stored data from serialization
        CollectionInterfaceType _interfaceType;
        int _count;
        int[] _elementTypeHashCodes;

        // ACTUAL COLLECTION
        IEnumerable _collection;

        PropertySpecification _specification;

        internal DeserializationCollection(ObjectReference reference,
                                           RecursiveSerializerMemberInfo memberInfo,
                                           PropertySpecification specification,
                                           int[] elementTypeHashCodes,
                                           int count,
                                           CollectionInterfaceType interfaceType) : base(reference, memberInfo)
        {
            _count = count;
            _interfaceType = interfaceType;
            _elementTypeHashCodes = elementTypeHashCodes;
            _specification = specification;
        }

        internal override PropertySpecification GetPropertySpecification()
        {
            return _specification;
        }

        /// <summary>
        /// FOR PERFORMANCE
        /// </summary>
        internal int GetElementTypeHashCode(int index)
        {
            return _elementTypeHashCodes[index];
        }

        internal void FinalizeCollection(IList<ObjectInfo> resolvedChildren, Func<int, HashedType> hashCodeResolver)
        {
            if (this.InterfaceType != CollectionInterfaceType.IList)
                throw new Exception("UNHANDLED INTERFACE TYPE DeserializationCollection.cs");

            if (this.MemberInfo.Mode == SerializationMode.Specified)
                throw new Exception("Trying to call DeserializationCollection in SPECIFIED MODE");

            for (int index = 0; index < _count; index++)
            {
                var element = resolvedChildren[index];
                var elementType = hashCodeResolver(_elementTypeHashCodes[index]);

                // VALIDATE ELEMENT TYPE
                if (!element.Type.GetDeclaringType().IsAssignableFrom(elementType.GetImplementingType()))
                    throw new Exception("Invalid collection element type: " + element.Type.DeclaringType);

                // ADD TO THE LIST
                (_collection as IList).Add(element.GetObject());
            }
        }

        internal override void Construct(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            switch (this.MemberInfo.Mode)
            {
                case SerializationMode.Default:
                    ConstructDefault(resolvedProperties);
                    break;
                case SerializationMode.Specified:
                    ConstructSpecified(resolvedProperties);
                    break;
                default:
                    throw new Exception("Unhandled SerializationMode type:  DeserializationCollection.cs");
            }
        }

        private void ConstructDefault(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            // NO PROPERTY SUPPORT FOR DEFAULT MODE
            if (resolvedProperties.Any())
                throw new RecursiveSerializerException(this.Reference.Type, "No property support for DEFAULT mode collections");

            // CONSTRUCT
            try
            {
                _collection = this.MemberInfo.ParameterlessConstructor.Invoke(new object[] { }) as IEnumerable;

                if (_collection == null)
                    throw new Exception("Constructor failed for collection of type:  " + this.Reference.Type.DeclaringType);
            }
            catch (Exception ex)
            {
                throw new RecursiveSerializerException(this.Reference.Type, "Error constructing from parameterless constructor", ex);
            }
        }

        private void ConstructSpecified(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            var reader = new PropertyReader(resolvedProperties);

            try
            {
                _collection = this.MemberInfo.SpecifiedConstructor.Invoke(new object[] { reader }) as IEnumerable;

                if (_collection == null)
                    throw new Exception("Constructor failed for collection of type:  " + this.Reference.Type.ToString());
            }
            catch (Exception ex)
            {
                throw new RecursiveSerializerException(this.Reference.Type, "Error constructing from specified constructor: " + this.MemberInfo.SpecifiedConstructor.Name, ex);
            }
        }

        protected override ObjectInfo ProvideResult()
        {
            return new ObjectInfo(_collection, new HashedType(this.Reference.Type.GetDeclaringType(), _collection.GetType()));
        }
    }
}
