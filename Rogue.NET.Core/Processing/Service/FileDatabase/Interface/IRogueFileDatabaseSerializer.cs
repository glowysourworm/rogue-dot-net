namespace Rogue.NET.Core.Processing.Service.Interface
{
    /// <summary>
    /// Component that is responsible for the actual data serialization of individual obejct records
    /// in the rogue file database.
    /// </summary>
    public interface IRogueFileDatabaseSerializer
    {
        /// <summary>
        /// Fetches a record from the directory representing type T. The record will store all the data
        /// for type V serialized to file using the ISerializable interface.
        /// </summary>
        /// <typeparam name="V">Type for this record</typeparam>
        /// <param name="recordName">UNIQUE record name for this entry</param>
        public V Fetch<V>(string recordName);

        /// <summary>
        /// Adds / Updates a(n) New / Existing record for this entry of type V.
        /// </summary>
        /// <typeparam name="V">Type for this record</typeparam>
        /// <param name="recordName">UNIQUE record name for this entry</param>
        /// <param name="value">Value data to store / update</param>
        public void AddOrUpdate<V>(string recordName, V value);
    }
}
