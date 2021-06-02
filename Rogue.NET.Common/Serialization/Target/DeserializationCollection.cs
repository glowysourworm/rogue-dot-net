using Rogue.NET.Common.Serialization.Planning;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Rogue.NET.Common.Serialization.Target
{
    internal class DeserializationCollection : DeserializationObjectBase
    {
        internal int Count { get { return _count; } }
        internal Type ElementType { get { return _elementType; } }
        internal CollectionInterfaceType InterfaceType { get { return _interfaceType; } }

        // Stored data from serialization
        CollectionInterfaceType _interfaceType;
        int _count;
        Type _elementType;

        PropertyInfo _dictionaryKeyInfo;
        PropertyInfo _dictionaryValueInfo;

        // ACTUAL COLLECTION
        IEnumerable _collection;

        IEnumerable<PropertyDefinition> _definitions;

        internal DeserializationCollection(HashedObjectReference reference,
                                           RecursiveSerializerMemberInfo memberInfo,
                                           IEnumerable<PropertyDefinition> definitions,
                                           Type elementType,
                                           int count,
                                           CollectionInterfaceType interfaceType) : base(reference, memberInfo)
        {
            _count = count;
            _interfaceType = interfaceType;
            _elementType = elementType;

            _definitions = definitions;

            // Call these during initialization
            if (_interfaceType == CollectionInterfaceType.IDictionary)
            {
                if (_elementType.GetGenericArguments().Length != 2)
                    throw new Exception("Invalid generic argument list for DeserializionCollection element type" + _elementType.Name);

                _dictionaryKeyInfo = _elementType.GetProperty("Key");
                _dictionaryValueInfo = _elementType.GetProperty("Value");
            }
        }

        internal override IEnumerable<PropertyDefinition> GetPropertyDefinitions()
        {
            return _definitions;
        }

        internal void FinalizeCollection(IEnumerable<HashedObjectInfo> resolvedChildren)
        {
            if (this.MemberInfo.Mode == SerializationMode.Specified)
                throw new Exception("Trying to call DeserializationCollection in SPECIFIED MODE");

            var elements = new ArrayList();

            foreach (var resolvedChild in resolvedChildren)
            {
                // VALIDATE ELEMENT TYPE
                if (!_elementType.IsAssignableFrom(resolvedChild.Type.GetImplementingType()))
                    throw new Exception("Invalid collection element type: " + resolvedChild.Type.DeclaringType);

                // Add the finished object to the elements
                elements.Add(resolvedChild.GetObject());
            }

            // PROBABLY OK, WATCH FOR DIFFERENT COLLECTION TYPES
            if (_interfaceType == CollectionInterfaceType.Array)
                Array.Copy(elements.ToArray(), _collection as Array, _count);

            foreach (var element in elements)
            {
                // Reflect KeyValuePair values
                if (_interfaceType == CollectionInterfaceType.IDictionary)
                {
                    var key = _dictionaryKeyInfo.GetValue(element);
                    var value = _dictionaryValueInfo.GetValue(element);

                    (_collection as IDictionary).Add(key, value);
                }

                switch (_interfaceType)
                {
                    case CollectionInterfaceType.IList:
                        (_collection as IList).Add(element);
                        break;
                    default:
                        throw new Exception("Unhandled collection interface type: DeserializationCollection.FinalizeCollection");
                }
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
                case SerializationMode.None:
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
                if (_interfaceType == CollectionInterfaceType.Array)
                    _collection = Array.CreateInstance(_elementType, _count);

                else
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
                if (_interfaceType == CollectionInterfaceType.Array)
                    throw new Exception("Trying to construct array type in SPECIFIED MODE:  " + this.Reference.Type.ToString());

                else
                    _collection = this.MemberInfo.SpecifiedConstructor.Invoke(new object[] { reader }) as IEnumerable;

                if (_collection == null)
                    throw new Exception("Constructor failed for collection of type:  " + this.Reference.Type.ToString());
            }
            catch (Exception ex)
            {
                throw new RecursiveSerializerException(this.Reference.Type, "Error constructing from specified constructor: " + this.MemberInfo.SpecifiedConstructor.Name, ex);
            }
        }

        protected override HashedObjectInfo ProvideResult()
        {
            return new HashedObjectInfo(_collection, _collection.GetType());
        }
    }
}
