using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rogue.NET.Common.Extension
{
    public static class TypeExtension
    {
        /// <summary>
        /// Creates an instance of the specified type using the default (parameterless) constructor
        /// </summary>
        public static T Construct<T>(this Type type)
        {
            var constructor = type.GetConstructor(new Type[] { });

            return constructor == null ? default(T) : (T)constructor.Invoke(new object[] { });
        }

        /// <summary>
        /// Creates an instance of the specified type using the default (parameterless) constructor
        /// </summary>
        public static object Construct(this Type type)
        {
            var constructor = type.GetConstructor(new Type[] { });

            return constructor == null ? null : constructor.Invoke(new object[] { });
        }
    }
}
