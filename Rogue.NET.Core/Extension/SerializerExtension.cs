using Rogue.NET.Common.Utility;
using Rogue.NET.Core.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Core.Extension
{
    public static class SerializerExtension
    {
        /// <summary>
        /// Creates a unqiue Hex fingerprint of object public property values. This can be used as a hash key; but
        /// is only unique up to the values of set public properties for the object type.
        /// </summary>
        public static string ToFingerprint(this object source)
        {
            // Serialize list of public property values
            var properties = source.GetType().GetProperties();
            var propertyValues = properties.OrderBy(property => property.Name)
                                           .Select(property => property.GetValue(source))
                                           .ToList();

            var buffer = BinarySerializer.Serialize(propertyValues);

            return buffer.Aggregate(new StringBuilder(), (current, next) => current.Append(next.ToString("X2")))
                         .ToString();
        }
    }
}
