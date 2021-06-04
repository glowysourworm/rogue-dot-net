using Rogue.NET.Common.Collection;
using Rogue.NET.Common.Extension;
using Rogue.NET.Common.Serialization.Target;

using System.Collections.Generic;
using System.Linq;

namespace Rogue.NET.Common.Serialization.Planning
{
    internal class PropertySpecification
    {
        /// <summary>
        /// Type of object used to create the specification. THIS IS INCLUDED IN THE
        /// HASH CODE! There could be multiple specifications PER TYPE. 
        /// </summary>
        internal HashedType ObjectType { get; private set; }
        internal IEnumerable<PropertyDefinition> Definitions { get; private set; }

        // KEEP HASH FOR PERFORMANCE WHEN RESOLVING
        SimpleDictionary<int, PropertyDefinition> _definitionHash;

        // CACHE HASH CODE FOR PERFORMANCE
        int _calculatedHashCode;

        internal PropertySpecification(HashedType objectType, IEnumerable<PropertyDefinition> definitions)
        {
            this.ObjectType = objectType;
            this.Definitions = definitions;

            // Calculate to save time iterating later
            _definitionHash = definitions.ToSimpleDictionary(definition => definition.CreateHashCode(definition.PropertyName, definition.PropertyType),
                                                             definition => definition);
            _calculatedHashCode = default(int);
        }

        internal PropertyDefinition GetHashedDefinition(string propertyName, HashedType propertyType)
        {
            var hashCode = this.CreateHashCode(propertyName, propertyType);

            if (!_definitionHash.ContainsKey(hashCode))
                throw new System.Exception("Missing hash code for property type" + propertyType.ToString());

            return _definitionHash[hashCode];
        }

        public override bool Equals(object obj)
        {
            var specification = obj as PropertySpecification;

            return this.GetHashCode() == specification.GetHashCode();
        }

        public override int GetHashCode()
        {
            if (_calculatedHashCode == default(int))
            {
                var hashCode = this.CreateHashCode(this.ObjectType);

                foreach (var definition in this.Definitions)
                    hashCode = hashCode.ExtendHashCode(definition);

                _calculatedHashCode = hashCode;
            }

            return _calculatedHashCode;
        }

        public override string ToString()
        {
            return string.Format("{ ObjectType={0}, Definitions[{1}] }", this.ObjectType, this.Definitions.Count());
        }
    }
}
