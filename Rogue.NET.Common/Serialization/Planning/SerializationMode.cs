namespace Rogue.NET.Common.Serialization.Planning
{
    /// <summary>
    /// Specifies mode of serialization per-object in the graph
    /// </summary>
    internal enum SerializationMode : byte
    {
        /// <summary>
        /// No constructor used to create object (ARRAYS, PRIMITIVES AND NULL REFERENCES)
        /// </summary>
        None = 0,

        /// <summary>
        /// Uses default constructor for creating objects
        /// </summary>
        Default = 1,

        /// <summary>
        /// Uses specified methods for creating objects:  parameterless ctor(), 
        /// GetPropertyDefinitions(PropertyPlanner), GetProperties(PropertyWriter), SetProperties(PropertyReader)
        /// </summary>
        Specified = 2
    }
}
