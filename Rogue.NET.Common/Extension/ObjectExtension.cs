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
    }
}
