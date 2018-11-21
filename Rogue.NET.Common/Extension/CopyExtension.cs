using ExpressMapper;
using KellermanSoftware.CompareNetObjects;
using System;

namespace Rogue.NET.Common.Extension
{
    public static class CopyExtension
    {
        public static TDest Map<TSource, TDest>(this TSource source)
        {
            return Mapper.Map<TSource, TDest>(source);
        }

        public static T Copy<T>(this T source)
        {
            var constructor = typeof(T).GetConstructor(new Type[] { });
            var instance = constructor.Invoke(new object[] { });

            return (T)Mapper.Map(source, instance, typeof(T), typeof(T));
        }

        public static bool DeepEquals(this object source, object dest)
        {
            var logic = new CompareLogic(new ComparisonConfig()
            {
                MaxStructDepth = 5
            });

            return logic.Compare(source, dest).AreEqual;
        }
    }
}
