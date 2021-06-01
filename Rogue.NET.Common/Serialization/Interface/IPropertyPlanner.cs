using System;

namespace Rogue.NET.Common.Serialization.Interface
{
    public interface IPropertyPlanner
    {
        void Define<T>(string propertyName);

        void Define(string propertyName, Type propertyType);
    }
}
