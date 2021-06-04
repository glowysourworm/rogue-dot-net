namespace Rogue.NET.Common.Serialization.Planning
{
    /// <summary>
    /// Specifies mode of serialization per-object in the graph
    /// </summary>
    public enum SerializationMode : byte
    {
        /// <summary>
        /// Uses default constructor for creating objects
        /// </summary>
        Default = 0,

        /// <summary>
        /// Uses specified methods for creating objects:  parameterless ctor(), 
        /// GetPropertyDefinitions(PropertyPlanner), GetProperties(PropertyWriter), SetProperties(PropertyReader)
        /// </summary>
        Specified = 1
    }
}
