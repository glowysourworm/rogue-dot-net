using System;
using System.Linq;

namespace Rogue.NET.Common.Extension
{
    public static class EnumExtension
    {
        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var member = value.GetType()
                              .GetMember(value.ToString())
                              .FirstOrDefault();

            if (member == null)
                throw new Exception("No Member Defined for Enum Type");

            var attributes = member.GetCustomAttributes(typeof(T), true);

            return attributes.Any() ? (T)attributes.First() : default(T);
        }
    }
}
