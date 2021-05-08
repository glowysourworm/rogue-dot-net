using AgileObjects.AgileMapper;
using NClone;

namespace Rogue.NET.Common.Extension
{
    public static class MapperExtension
    {
        /// <summary>
        /// Creates a new object from a different source type using name comparison
        /// </summary>
        public static TDest Map<TSource, TDest>(this TSource source)
        {
            return Mapper.Map<TSource>(source).ToANew<TDest>();
        }

        /// <summary>
        /// Updates a given object from a sourc object using property name resolution
        /// </summary>
        public static TDest MapOnto<TSource, TDest>(this TSource source, TDest dest)
        {
            return Mapper.Map<TSource>(source).Over<TDest>(dest);
        }

        /// <summary>
        /// Creates a copy using NClone
        /// </summary>
        public static T DeepClone<T>(this T source)
        {
            return Clone.ObjectGraph<T>(source);
        }
    }
}
