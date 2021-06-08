using Rogue.NET.Common.Serialization.Component;
using Rogue.NET.Common.Serialization.Planning;
using Rogue.NET.Common.Serialization.Utility;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializedCollectionNode : DeserializedObjectNode
    {
        internal List<DeserializedNodeBase> CollectionNodes { get; private set; }

        internal int Count { get { return _count; } }
        internal CollectionInterfaceType InterfaceType { get { return _interfaceType; } }
        internal HashedType ElementType { get { return _elementType; } }

        // Stored data from serialization
        CollectionInterfaceType _interfaceType;
        int _count;
        HashedType _elementType;

        // ACTUAL COLLECTION
        IEnumerable _collection;

        PropertySpecification _specification;

        internal DeserializedCollectionNode(PropertyDefinition definition,
                                            HashedType type,
                                            int referenceId,
                                            RecursiveSerializerMemberInfo memberInfo,
                                            PropertySpecification specification,
                                            HashedType elementType,
                                            int count,
                                            CollectionInterfaceType interfaceType) : base(definition, type, referenceId, memberInfo, specification)
        {
            _count = count;
            _interfaceType = interfaceType;
            _elementType = elementType;
            _specification = specification;

            this.CollectionNodes = new List<DeserializedNodeBase>(count);
        }

        internal override PropertySpecification GetPropertySpecification()
        {
            return _specification;
        }

        internal void FinalizeCollection(IList<PropertyResolvedInfo> resolvedChildren)
        {
            if (this.InterfaceType != CollectionInterfaceType.IList)
                throw new Exception("UNHANDLED INTERFACE TYPE DeserializationCollection.cs");

            if (this.MemberInfo.Mode == SerializationMode.Specified)
                throw new Exception("Trying to call DeserializationCollection in SPECIFIED MODE");

            for (int index = 0; index < _count; index++)
            {
                var element = resolvedChildren[index];

                // VALIDATE ELEMENT TYPE (NOTE*** ELEMENT TYPE IMPLEMENTING TYPE NOT TRACKED!)
                if (!_elementType.GetDeclaringType().IsAssignableFrom(element.ResolvedType.GetImplementingType()))
                    throw new Exception("Invalid collection element type: " + element.ResolvedType.DeclaringType);

                // ADD TO THE LIST
                (_collection as IList).Add(element.ResolvedObject);
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
                throw new RecursiveSerializerException(this.Type, "No property support for DEFAULT mode collections");

            // CONSTRUCT
            try
            {
                _collection = this.MemberInfo.ParameterlessConstructor.Invoke(new object[] { }) as IEnumerable;

                if (_collection == null)
                    throw new Exception("Constructor failed for collection of type:  " + this.Type.DeclaringType);
            }
            catch (Exception ex)
            {
                throw new RecursiveSerializerException(this.Type, "Error constructing from parameterless constructor", ex);
            }
        }

        private void ConstructSpecified(IEnumerable<PropertyResolvedInfo> resolvedProperties)
        {
            var reader = new PropertyReader(resolvedProperties);

            try
            {
                _collection = this.MemberInfo.SpecifiedConstructor.Invoke(new object[] { reader }) as IEnumerable;

                if (_collection == null)
                    throw new Exception("Constructor failed for collection of type:  " + this.Type.ToString());
            }
            catch (Exception ex)
            {
                throw new RecursiveSerializerException(this.Type, "Error constructing from specified constructor: " + this.MemberInfo.SpecifiedConstructor.Name, ex);
            }
        }

        public override int GetHashCode()
        {
            return this.ReferenceId;
        }

        public override bool Equals(object obj)
        {
            var node = obj as DeserializedCollectionNode;

            return this.GetHashCode() == node.GetHashCode();
        }

        protected override object ProvideResult()
        {
            return _collection;
        }
    }
}
