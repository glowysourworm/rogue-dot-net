using System.Linq;

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
    }
}
