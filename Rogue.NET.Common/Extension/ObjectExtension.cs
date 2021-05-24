using Rogue.NET.Common.Utility;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

        public static bool ImplementsInterface<T>(this object value)
        {
            return value.GetType()
                        .GetInterfaces()
                        .Any(type => type.Equals(typeof(T)));
        }

        /// <summary>
        /// Returns a deep clone of the object using the Agile.Mapper
        /// </summary>
        public static T Clone<T>(this T value) where T : class
        {
            return MapperExtension.DeepClone(value);
        }

        public static T DeepCopy<T>(this T value)
        {
            return (T)BinarySerializer.BinaryCopy(value);
        }

        public static PropertyInfo GetPropertyInfo<T, V>(this T theObject, Expression<Func<T, V>> propertySelector)
        {
            var unaryExpression = propertySelector.Body as UnaryExpression;

            if (unaryExpression == null)
                throw new Exception("Invalid use of property selector ObjectExtension.GetPropertyInfo<T, V>");

            var memberInfo = unaryExpression.Operand as MemberExpression;

            if (memberInfo == null)
                throw new Exception("Invalid use of property selector ObjectExtension.GetPropertyInfo<T, V>");

            var propertyInfo = memberInfo.Member as PropertyInfo;

            if (propertyInfo == null)
                throw new Exception("Invalid use of property selector ObjectExtension.GetPropertyInfo<T, V>");

            return propertyInfo;
        }

        /// <summary>
        /// Implements standard method to calculate hash codes. Calls object.GetHashCode(..) from each
        /// object parameter. Uses these to create the final hash.
        /// </summary>
        public static int CreateHashCode(this object theObject, params object[] propertiesToHash)
        {
            var hash = 397;

            for (int index = 0; index < propertiesToHash.Length; index++)
            {
                hash = (hash * 397) ^ propertiesToHash[index].GetHashCode();
            }

            return hash;
        }

        /// <summary>
        /// Implements standard method to calculate hash codes to EXTEND hash code already calculated. 
        /// Calls object.GetHashCode(..) from each object parameter. Uses these to create the final hash.
        /// </summary>
        public static int ExtendHashCode(this int hashCode, params object[] propertiesToHash)
        {
            var hash = hashCode;

            for (int index = 0; index < propertiesToHash.Length; index++)
            {
                hash = (hash * 397) ^ propertiesToHash[index].GetHashCode();
            }

            return hash;
        }
    }
}
