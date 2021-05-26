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

        // Stored data from serialization
        CollectionInterfaceType _interfaceType;
        int _count;
        Type _elementType;

        PropertyInfo _dictionaryKeyInfo;
        PropertyInfo _dictionaryValueInfo;

        // ACTUAL COLLECTION
        IEnumerable _collection;

        internal DeserializationCollection(HashedObjectReference reference,
                                         RecursiveSerializerMemberInfo memberInfo,
                                         Type elementType,
                                         int count,
                                         CollectionInterfaceType interfaceType) : base(reference, memberInfo)
        {
            _count = count;
            _interfaceType = interfaceType;
            _elementType = elementType;

            Construct();

            // Call these during initialization
            if (_interfaceType == CollectionInterfaceType.IDictionary)
            {
                if (_elementType.GetGenericArguments().Length != 2)
                    throw new Exception("Invalid generic argument list for DeserializionCollection element type" + _elementType.Name);

                _dictionaryKeyInfo = _elementType.GetProperty("Key");
                _dictionaryValueInfo = _elementType.GetProperty("Value");
            }
        }

        internal void FinalizeCollection(IEnumerable<DeserializationObjectBase> resolvedChildren)
        {           
            var elements = new ArrayList();

            foreach (var resolvedChild in resolvedChildren)
            {
                // VALIDATE ELEMENT TYPE
                if (!resolvedChild.Reference.Type.Resolve().Equals(_elementType))
                    throw new Exception("Invalid collection element type: " + resolvedChild.Reference.Type.TypeName);

                // CALL RESOLVE() TO DETACH HashedObjectInfo
                var objectInfo = resolvedChild.Resolve();

                // Add the finished object to the elements
                elements.Add(objectInfo.TheObject);
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

        // FOR COLLECTIONS - NO PROPERTIES ARE SUPPORTED UNLESS THE COLLECTION USES SERIALIZATIONMODE.SPECIFIED
        protected override IEnumerable<PropertyDefinition> GetPropertyDefinitions(PropertyPlanner planner)
        {
            // DEFAULT MODE - NO PROPERTY SUPPORT
            if (this.MemberInfo.PlanningMethod == null)
                return new PropertyDefinition[] { };

            // CLEAR CURRENT CONTEXT
            planner.ClearContext();

            // CALL OBJECT'S GetPropertyDefinitions METHOD
            try
            {
                this.MemberInfo.PlanningMethod.Invoke(_collection, new object[] { planner });
            }
            catch (Exception innerException)
            {
                throw new Exception("Error trying to read properties from " + this.Reference.Type.TypeName, innerException);
            }

            return planner.GetResult();
        }

        protected override void Construct()
        {
            try
            {
                if (_interfaceType == CollectionInterfaceType.Array)
                    _collection = Array.CreateInstance(_elementType, _count);

                else
                    _collection = this.MemberInfo.ParameterlessConstructor.Invoke(new object[] { }) as IEnumerable;

                if (_collection == null)
                    throw new Exception("Constructor failed for collection of type:  " + this.Reference.Type.TypeName);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error trying to construct object of type {0}. Must have a parameterless constructor",
                                                  this.Reference.Type.TypeName), ex);
            }
        }

        protected override HashedObjectInfo ResolveImpl()
        {
            return new HashedObjectInfo(_collection, _collection.GetType());
        }

        protected override void WriteProperties(PropertyReader reader)
        {
            // DEFAULT MODE - NO PROPERTY SUPPORT
            if (this.MemberInfo.SetMethod == null && reader.Properties.Any())
            {
                throw new Exception("Trying read reflected or custom properties in DEFAULT MODE for collection:  " + this.Reference.Type.TypeName);
            }

            else
            {
                // CALL OBJECT'S SetProperties METHOD
                try
                {
                    this.MemberInfo.SetMethod.Invoke(_collection, new object[] { reader });
                }
                catch (Exception innerException)
                {
                    throw new Exception("Error trying to set properties from " + this.Reference.Type.TypeName, innerException);
                }
            }
        }
    }
}
