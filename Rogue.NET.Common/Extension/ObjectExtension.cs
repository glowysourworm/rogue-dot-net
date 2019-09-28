using Rogue.NET.Common.Utility;
using System.Linq;
using System.Text;

namespace Rogue.NET.Common.Extension
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Returns the First or Default attribute of the supplied type for the supplied object
        /// </summary>
        public static T GetAttribute<T>(this object value) where T : System.Attribute
        {
            var attributes = value.GetType().GetCustomAttributes(typeof(T), true);

            return attributes.Any() ? (T)attributes.First() : default(T);
        }

        /// <summary>
        /// Returns a deep clone of the object using the Agile.Mapper
        /// </summary>
        public static T DeepCopy<T>(this T value) where T : class
        {
            return value.DeepClone();
        }

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
