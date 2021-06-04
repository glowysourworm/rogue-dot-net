using Rogue.NET.Common.Serialization.Component.Interface;

namespace Rogue.NET.Common.Serialization.Interface
{
    /// <summary>
    /// Provides custom serialization control for use with the RecursiveSerializer.
    /// </summary>
    public interface IRecursiveSerializable
    {
        /// <summary>
        /// Method used to store properties to the underlying serialization stream
        /// </summary>
        void GetProperties(IPropertyWriter writer);
    }
}
